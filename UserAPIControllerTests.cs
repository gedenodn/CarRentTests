using Moq;
using CarRent.Controllers;
using CarRent.Repositories;
using CarRent.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace CarRentTests
{
    public class UserAPIControllerTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserAPIController _controller;

        public UserAPIControllerTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _controller = new UserAPIController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetUsers_ReturnsOkResult_WithListOfUsers()
        {
            _mockRepo.Setup(repo => repo.GetAllUsersAsync())
                     .ReturnsAsync(new List<UserDto>
                     {
                         new UserDto { Id = "1", UserName = "User1", Email = "user1@example.com" },
                         new UserDto { Id = "2", UserName = "User2", Email = "user2@example.com" }
                     });

            var result = await _controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUsers = Assert.IsType<List<UserDto>>(okResult.Value);
            Assert.Equal(2, returnUsers.Count);
        }

        [Fact]
        public async Task GetUserById_UserExists_ReturnsOkResult_WithUser()
        {
            var userId = "1";
            _mockRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                     .ReturnsAsync(new UserDto { Id = userId, UserName = "User1", Email = "user1@example.com" });

            var result = await _controller.GetUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var user = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, user.Id);
        }

        [Fact]
        public async Task GetUserById_UserDoesNotExist_ReturnsNotFound()
        {
            var userId = "2";
            _mockRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                     .ReturnsAsync((UserDto)null);

            var result = await _controller.GetUserById(userId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateUser_ValidUser_ReturnsCreatedAtAction()
        {
            var userDto = new UserDto { Id = "3", UserName = "User3", Email = "user3@example.com" };
            _mockRepo.Setup(repo => repo.AddUserAsync(userDto)).Returns(Task.CompletedTask);

            var result = await _controller.CreateUser(userDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnUser = Assert.IsType<UserDto>(createdAtActionResult.Value);
            Assert.Equal(userDto.Id, returnUser.Id);
        }

        [Fact]
        public async Task UpdateUser_ValidUser_ReturnsNoContent()
        {
            var userId = "1";
            var userDto = new UserDto { Id = userId, UserName = "UpdatedUser", Email = "updateduser@example.com" };
            _mockRepo.Setup(repo => repo.UpdateUserAsync(userDto)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateUser(userId, userDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_UserExists_ReturnsNoContent()
        {
            var userId = "1";
            _mockRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                     .ReturnsAsync(new UserDto { Id = userId });
            _mockRepo.Setup(repo => repo.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

            var result = await _controller.DeleteUser(userId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_UserDoesNotExist_ReturnsNotFound()
        {
            var userId = "2";
            _mockRepo.Setup(repo => repo.GetUserByIdAsync(userId))
                     .ReturnsAsync((UserDto)null);

            var result = await _controller.DeleteUser(userId);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
