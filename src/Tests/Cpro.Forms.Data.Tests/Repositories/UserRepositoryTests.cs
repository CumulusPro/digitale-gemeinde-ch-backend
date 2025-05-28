using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models.User;
using Cpro.Forms.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cpro.Forms.Data.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly SqlContext _dbContext;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SqlContext(options);
        _userRepository = new UserRepository(_dbContext);
    }

    [Fact]
    public async Task CreateUserAsync_ValidUser_SavesSuccessfully()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var result = await _userRepository.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john.doe@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingId_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "jane.doe@example.com",
            FirstName = "Jane"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(user).State = EntityState.Detached;

        // Act
        var result = await _userRepository.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jane.doe@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "search.me@example.com",
            FirstName = "Search"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(user).State = EntityState.Detached;

        // Act
        var result = await _userRepository.GetUserByEmailAsync("search.me@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Search", result.FirstName);
    }

    [Fact]
    public async Task UpdateUserAsync_ValidUser_UpdatesSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "update@example.com",
            FirstName = "Before"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(user).State = EntityState.Detached;

        user.FirstName = "After";

        // Act
        var result = await _userRepository.UpdateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("After", result.FirstName);
    }

    [Fact]
    public async Task SearchUsers_WithMatchingCriteria_ReturnsResults()
    {
        // Arrange
        _dbContext.Users.AddRange(
            new User
            {
                Id = Guid.NewGuid(),
                Email = "alice@example.com",
                FirstName = "Alice"
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "bob@example.com",
                FirstName = "Bob"
            }
        );
        await _dbContext.SaveChangesAsync();

        var searchRequest = new UserSearchRequest
        {
            Email = "example.com",
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _userRepository.SearchUsers(searchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Data, u => Assert.Contains("example.com", u.Email));
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistentUser_ReturnsNull()
    {
        // Act
        var result = await _userRepository.GetUserByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}
