using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.BusinessLogic.Services;
using ServerApp.DataAccess;

namespace ServerApp.Tests.IntegrationTests;

public class AuthServiceIntTests
{
    private readonly AuthService _service;
    private readonly MainDbContext _context;
    private readonly SignInManager<UserProfileEntity> _signInManager;

    public AuthServiceIntTests()
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
        services.AddHttpContextAccessor(); // required by SignInManager

        services.AddAuthentication();
        services.AddScoped<IUserClaimsPrincipalFactory<UserProfileEntity>, UserClaimsPrincipalFactory<UserProfileEntity, IdentityRole>>();

        var serviceProvider = services.BuildServiceProvider();

        // Init service
        _signInManager = serviceProvider.GetRequiredService<SignInManager<UserProfileEntity>>();
        _service = new AuthService(_signInManager);

        // Reset DB for each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    private async Task<UserProfileEntity> SeedUser()
    {
        var user = new UserProfileEntity()
        {
            UserName = "user1",
            Email = "user1@mail.com",
            EmailConfirmed = true
        };

        if (await _signInManager.UserManager.FindByNameAsync(user.UserName!) != null)
            throw new Exception("User already exists");
        var result = await _signInManager.UserManager.CreateAsync(user, "!String1");
        if (!result.Succeeded)
        {
            var error = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user: {error}");
        }
        return user;
    }

    [Fact]
    public async Task RegisterUserTest()
    {
        // Arrange
        var username = "test-user";
        var displayName = "Test User";
        var email = "testuser@mail.com";
        var password = "Valid1!";

        // Act
        await _service.RegisterUser(username, password, displayName, email);

        // Assert
        var user = await _signInManager.UserManager.FindByNameAsync(username);
        Assert.NotNull(user);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Equal(email, user.Email);
        var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        Assert.True(passwordCheck.Succeeded);
    }

    [Fact]
    public async Task RegisterWithExistingName()
    {
        // Arrange
        var user = await SeedUser();
        var email = "user@mail.com";
        var displayName = "display-name";
        var password = "Password!1";

        // Act
        var result = await _service.RegisterUser(user.UserName ?? "", password, displayName, email);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData("valid-user", "Valid1!", "valid", "valid@mail.com", true)]
    [InlineData("valid-user", "Valid1!", null, "valid@mail.com", true)]
    [InlineData("invalid username", "Valid1!", "valid", "valid@mail.com", false)]
    [InlineData("valid-user", "invalid-password", "valid", "valid@mail.com", false)]
    [InlineData("valid-user", "Valid1!", "valid", "invalid-email", false)]
    public async Task RegisterInputValidationTest(string username, string password, string? displayName, string email, bool shouldPass)
    {
        // Act
        var result = await _service.RegisterUser(username, password, displayName, email);

        // Assert 
        if (shouldPass) Assert.True(result.Success);
        else Assert.True(result.IsFailed);
    }
}