using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Service.Models.User;
using Cpro.Forms.Service.Services;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Data.Models.User.User { Id = userId };
        var expectedResponse = new UserResponse { Id = userId };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(expectedResponse);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _userRepositoryMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(user), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_CreatesUser_WhenValidInput()
    {
        // Arrange
        var request = new UserRequest();
        var user = new Data.Models.User.User();
        var expectedResponse = new UserResponse();

        _mapperMock.Setup(x => x.Map<Data.Models.User.User>(request))
            .Returns(user);
        _userRepositoryMock.Setup(x => x.CreateUserAsync(user))
            .ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserResponse>(user))
            .Returns(expectedResponse);

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mapperMock.Verify(x => x.Map<Data.Models.User.User>(request), Times.Once);
        _userRepositoryMock.Verify(x => x.CreateUserAsync(user), Times.Once);
        _mapperMock.Verify(x => x.Map<UserResponse>(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatesUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new UpdateUserRequest();
        var existingUser = new Data.Models.User.User { Id = userId };
        var updatedUser = new Data.Models.User.User { Id = userId };
        var expectedResponse = new UserResponse { Id = userId };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mapperMock.Setup(x => x.Map(request, existingUser))
            .Returns(updatedUser);
        _userRepositoryMock.Setup(x => x.UpdateUserAsync(updatedUser))
            .ReturnsAsync(updatedUser);
        _mapperMock.Setup(x => x.Map<UserResponse>(updatedUser))
            .Returns(expectedResponse);

        // Act
        var result = await _userService.UpdateUserAsync(userId, request);

        // Assert
        _userRepositoryMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_DeletesUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Data.Models.User.User { Id = userId };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _userRepositoryMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.DeleteUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task AssignUserRoleAsync_AssignsRole_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = 1;
        string email = "test@test.com";
        var user = new Data.Models.User.User { Id = userId, Role = Data.Models.User.Role.Admin };
        var updated = new Data.Models.User.User { Id = userId, Role = Data.Models.User.Role.Designer };

        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(email))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.UpdateUserAsync(user))
            .ReturnsAsync(updated);

        // Act
        await _userService.AssignUserRoleAsync(userId, Models.Enums.Role.Designer, email);

        // Assert
        _userRepositoryMock.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
        _userRepositoryMock.Verify(x => x.GetUserByEmailAsync(email), Times.Once);
        _userRepositoryMock.Verify(x => x.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task SearchUsers_ReturnsPagingResponse_WhenUsersFound()
    {
        // Arrange
        var searchRequest = new UserSearchRequest();
        var expectedResponse = new PagingResponse<UserResponse>();

        _mapperMock.Setup(x => x.Map<Data.Models.User.UserSearchRequest>(searchRequest))
            .Returns(new Data.Models.User.UserSearchRequest());
        _userRepositoryMock.Setup(x => x.SearchUsers(It.IsAny<Data.Models.User.UserSearchRequest>()))
            .ReturnsAsync(new PagingResponse<Data.Models.User.User>());
        _mapperMock.Setup(x => x.Map<PagingResponse<UserResponse>>(It.IsAny<PagingResponse<Data.Models.User.User>>()))
            .Returns(expectedResponse);

        // Act
        var result = await _userService.SearchUsers(searchRequest);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mapperMock.Verify(x => x.Map<Data.Models.User.UserSearchRequest>(searchRequest), Times.Once);
        _userRepositoryMock.Verify(x => x.SearchUsers(It.IsAny<Data.Models.User.UserSearchRequest>()), Times.Once);
        _mapperMock.Verify(x => x.Map<PagingResponse<UserResponse>>(It.IsAny<PagingResponse<Data.Models.User.User>>()), Times.Once);
    }
}