using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.BusinessLogic.Services;
using ServerApp.DataAccess;
using ServerApp.DataAccess.Repositories;

namespace ServerApp.Tests.IntegrationTests;

public class ProfilesServiceIntTests
{
    private readonly ProfilesService _service;
    private readonly MainDbContext _context;
    private readonly UserManager<UserProfileEntity> _users;

    public ProfilesServiceIntTests()
    {
        // Setup db
        var options = new DbContextOptionsBuilder<MainDbContext>()
            //.UseInMemoryDatabase($"TestDb").Options;
            .UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}").Options;
        _context = new MainDbContext(options);

        // Setup Identity services
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddIdentity<UserProfileEntity, IdentityRole>()
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();
        services.AddSingleton(_context); // EF context for Identity
        var serviceProvider = services.BuildServiceProvider();

        // Init service
        var images = new ProfileImageRepository(_context);
        _users = serviceProvider.GetRequiredService<UserManager<UserProfileEntity>>();

        _service = new ProfilesService(images, _users);

        // Reset DB for each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private async Task<List<UserProfileEntity>> SeedUsers()
    {
        var users = new List<UserProfileEntity>
        {
            new() { UserName = "user1", Email = "user1@mail.com", EmailConfirmed = true },
            new() { UserName = "user2", Email = "user2@mail.com", EmailConfirmed = true },
            new() { UserName = "user3", Email = "user3@mail.com", EmailConfirmed = true }
        };
        foreach (var user in users)
        {
            if (await _users.FindByNameAsync(user.UserName!) != null)
                continue;
            var result = await _users.CreateAsync(user, "!String1");
            if (!result.Succeeded)
            {
                var error = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user: {error}");
            }
        }
        return users;
    }

    [Fact]
    public async Task UpdateProfileTest()
    {
        // Arrange
        var users = await SeedUsers();
        var user = users[0];
        var newUsername = "new-username";
        var newDisplayName = "Display Name";
        var newBio = "New Bio";

        // Act
        var result = await _service.Update(user.UserName ?? "", newUsername, newDisplayName, newBio);

        // Assert
        Console.WriteLine(result.Error);
        Assert.True(result.Success);
        var updatedUser = await _users.FindByNameAsync(newUsername ?? "");
        Assert.NotNull(updatedUser);
        Assert.Equal(user.Id, updatedUser.Id);
        Assert.Equal(user.DisplayName, updatedUser.DisplayName);
        Assert.Equal(user.Bio, updatedUser.Bio);
        Assert.Equal(user.CreationDate, updatedUser.CreationDate);
    }

    [Fact]
    public async Task UpdateNonExistingUserTest()
    {
        // Arrange
        var newUsername = "new-username";
        var newDisplayName = "Display Name";
        var newBio = "New Bio";

        // Act
        var result = await _service
            .Update("non-existing", newUsername, newDisplayName, newBio);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task UpdateButUsernameIsTakenTest()
    {
        // Arrange
        var users = await SeedUsers();
        var user = users[0];
        var newUsername = "user2";
        var newDisplayName = "Display Name";
        var newBio = "New Bio";

        // Act
        var result = await _service
            .Update(user.UserName ?? "", newUsername, newDisplayName, newBio);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData("invalid name", "Some displayname", "some bio", true)]
    [InlineData("InvalidName", "Some displayname", "some bio", true)]
    [InlineData("new-username", "", "some bio", false)]
    [InlineData("user1", "", "some bio", false)]
    [InlineData("new-username", null, "some bio", false)]
    [InlineData("new-username", "some displayname", "", false)]
    public async Task UpdateUserInputValidationTest(string username, string displayName, string bio, bool shouldFail)
    {
        // Arrange
        var users = await SeedUsers();
        var user = users[0];

        // Act
        var result = await _service.Update(user.UserName ?? "", username, displayName, bio);

        // Assert
        if (shouldFail) Assert.True(result.IsFailed);
        else Assert.True(result.Success);
    }
}