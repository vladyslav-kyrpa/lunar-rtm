using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServerApp.BusinessLogic.Models;
using ServerApp.BusinessLogic.Services;
using ServerApp.DataAccess;
using ServerApp.DataAccess.Entities;
using ServerApp.DataAccess.Repositories;

namespace ServerApp.Tests.IntegrationTests;

public class ChatsServiceIntTests
{
    private readonly ChatsServices _service;
    private readonly MainDbContext _context;
    private readonly UserManager<UserProfileEntity> _users;

    public ChatsServiceIntTests()
    {
        // Setup db
        var options = new DbContextOptionsBuilder<MainDbContext>()
            //.UseInMemoryDatabase("TestDb").Options;
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
        var images = new ChatImageRepository(_context);
        var chats = new ChatRepository(_context);
        var messages = new MessageRepository(_context);
        _users = serviceProvider.GetRequiredService<UserManager<UserProfileEntity>>();

        _service = new ChatsServices(images, messages, chats, _users);

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

    private async Task<List<ChatEntity>> SeedChats()
    {
        var users = await SeedUsers();
        var chats = new List<ChatEntity>
        {
            new ChatEntity() { Id = Guid.NewGuid(), Title = "Chat1", Description = "1", Owner = users[0], OwnerId = users[0].Id,
                MemberIDs = { users[0].Id, users[1].Id }, Type = 1, CreationDate = DateTime.UtcNow
            },
            new ChatEntity() { Id = Guid.NewGuid(), Title = "Chat2", Description = "1", Owner = users[0],  OwnerId = users[0].Id,
                MemberIDs = { users[0].Id, users[2].Id }, Type = 1, CreationDate = DateTime.UtcNow
            }
        };

        _context.AddRange(chats);
        await _context.SaveChangesAsync();

        return _context.Chats.AsNoTracking().ToList();
    }

    [Fact]
    public async Task CreateChatTest()
    {
        // Arrange
        var users = await SeedUsers();
        var title = "test-chat";
        var description = "test";
        var owner = users[1];
        var type = ChatType.Polylogue;

        // Act
        var result = await _service.Create(title, description, owner.UserName ?? "", type);

        // Assert
        Assert.True(result.Success);

        var id = Guid.Parse(result.Value);
        var chat = await _context.Chats
            .Include(c => c.Owner)
            .FirstAsync(c => c.Id == id);

        Assert.Equal(title, chat.Title);
        Assert.Equal(description, chat.Description);
        Assert.Equal(owner.Id, chat.Owner.Id);
        Assert.Equal((int)type, chat.Type);
        Assert.Contains(owner.Id, chat.MemberIDs);
    }

    [Fact]
    public async Task CreateChatWithUnknownOwner()
    {
        // Act
        var result = await _service.Create("title", "desc", "random-user", ChatType.Polylogue);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData(101, 1, true)]
    [InlineData(0, 1, true)]
    [InlineData(50, 1, false)]
    [InlineData(1, 501, true)]
    [InlineData(1, 0, true)]
    [InlineData(1, 50, false)]
    public async Task CreateChatInputValidationTest(int titleLength, int descriptionLength, bool shouldFail)
    {
        var users = await SeedUsers();
        var title = new string('a', titleLength);
        var description = new string('a', descriptionLength);
        var owner = users[1];

        // Act
        var result = await _service.Create(title, description, owner.UserName ?? "", ChatType.Polylogue);
        if (shouldFail) Assert.True(result.IsFailed);
        else Assert.True(result.Success);
    }

    [Fact]
    public async Task UpdateChatTest()
    {
        // Arrange
        var chats = await SeedChats();
        var title = "new-title";
        var description = "new description";

        // Act
        var result = await _service.Update(chats[0].Id.ToString(), title, description);

        // Assert
        Assert.True(result.Success);
        var chat = await _context.Chats.FirstAsync(c => c.Id == chats[0].Id);
        Assert.Equal(chat.Title, title);
        Assert.Equal(chat.Description, description);
    }

    [Theory]
    [InlineData(101, 1, true)]
    [InlineData(0, 1, true)]
    [InlineData(50, 1, false)]
    [InlineData(1, 501, true)]
    [InlineData(1, 0, true)]
    [InlineData(1, 50, false)]
    public async Task UpdateChatInputValidationTest(int titleLength, int descriptionLength, bool shouldFail)
    {
        // Arrange
        var chats = await SeedChats();
        var title = new string('a', titleLength);
        var description = new string('a', descriptionLength);

        // Act
        var result = await _service.Update(chats[0].Id.ToString(), title, description);

        // Assert
        if (shouldFail) Assert.True(result.IsFailed);
        else Assert.True(result.Success);
    }

    [Fact]
    public async Task AddMemberTest()
    {
        // Arrange
        var chats = await SeedChats();
        var chat = chats[1].Id.ToString();
        var user = "user3";

        // Act
        var result = await _service.AddMember(user, chat);

        // Assert
        Assert.True(result.Success);
        var updatedChat = await _context.Chats
            .SingleAsync(c => c.Id == Guid.Parse(chat));
        var userEntity = await _users.FindByNameAsync(user);
        Assert.NotNull(userEntity);
        Assert.Contains(userEntity.Id, updatedChat.MemberIDs);
    }

    [Fact]
    public async Task AddNonExistingMemberTest()
    {
        // Arrange
        var chats = await SeedChats();
        var chat = chats[1].Id.ToString();
        var user = "random";

        // Act
        var result = await _service.AddMember(user, chat);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task AddMemberToNonExistingChatTest()
    {
        // Arrange
        var chat = "random";
        var user = "user1";

        // Act
        var result = await _service.AddMember(user, chat);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task RemoveMemberTest()
    {
        // Arrange
        var chats = await SeedChats();
        var chat = chats[1].Id.ToString();
        var user = "user3";

        // Act
        var result = await _service.RemoveMember(user, chat);

        // Assert
        Assert.True(result.Success);
        var updatedChat = await _context.Chats
            .SingleAsync(c => c.Id == Guid.Parse(chat));
        var userEntity = await _users.FindByNameAsync(user);
        Assert.NotNull(userEntity);
        Assert.DoesNotContain(userEntity.Id, updatedChat.MemberIDs);
    }

    [Fact]
    public async Task RemoveNonExistingMemberTest()
    {
        // Arrange
        var chats = await SeedChats();
        var chat = chats[1].Id.ToString();
        var user = "random";

        // Act
        var result = await _service.AddMember(user, chat);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task RemoveMemberToNonExistingChatTest()
    {
        // Arrange
        var chat = "random";
        var user = "user1";

        // Act
        var result = await _service.AddMember(user, chat);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task RemoveChatTest()
    {
        // Arrange
        var chats = await SeedChats();
        var id = chats[0].Id.ToString();

        // Act
        var result = await _service.Detele(id);

        // Arrange
        Assert.True(result.Success);
        var removedChat = await _context.Chats
            .FirstOrDefaultAsync(c => c.Id == chats[0].Id);
        Assert.Null(removedChat);
    }

    [Fact]
    public async Task RemoveNonExistingChatTest()
    {
        // Arrange
        var id = Guid.NewGuid(); 

        // Act
        var result = await _service.Detele(id.ToString());

        // Arrange
        Assert.True(result.IsFailed);
    }
}