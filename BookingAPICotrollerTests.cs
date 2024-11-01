using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarRent.Controllers;
using CarRent.DTOs;
using CarRent.Models;
using CarRent.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CarRentTests
{
    public class BookingAPIControllerTests
    {
        private readonly Mock<IBookingRepository> _mockRepo;
        private readonly BookingAPIController _controller;

        public BookingAPIControllerTests()
        {
            _mockRepo = new Mock<IBookingRepository>();
            _controller = new BookingAPIController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllBookings_ReturnsOkResult_WithListOfBookings()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking { Id = 1, CarId = 1, UserId = "user1" } };
            _mockRepo.Setup(repo => repo.GetAllBookingsAsync()).ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetAllBookings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnBookings = Assert.IsType<List<Booking>>(okResult.Value);
            Assert.Single(returnBookings);
        }

        [Fact]
        public async Task GetBookingById_ExistingId_ReturnsOkResult_WithBooking()
        {
            // Arrange
            var booking = new Booking { Id = 1, CarId = 1, UserId = "user1" };
            _mockRepo.Setup(repo => repo.GetBookingByIdAsync(1)).ReturnsAsync(booking);

            // Act
            var result = await _controller.GetBookingById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnBooking = Assert.IsType<Booking>(okResult.Value);
            Assert.Equal(1, returnBooking.Id);
        }

        [Fact]
        public async Task GetBookingById_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetBookingByIdAsync(It.IsAny<int>())).ReturnsAsync((Booking)null);

            // Act
            var result = await _controller.GetBookingById(99);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddBooking_ValidBooking_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var bookingDto = new BookingDto
            {
                UserId = "user1",
                CarId = 1,
                TotalPrice = 100,
                IsCancelled = false,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(2)
            };

            var booking = new Booking
            {
                Id = 1,
                UserId = "user1",
                CarId = 1,
                TotalPrice = 100,
                IsCancelled = false,
                StartDate = bookingDto.StartDate,
                EndDate = bookingDto.EndDate
            };

            _mockRepo.Setup(repo => repo.AddBookingAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddBooking(bookingDto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetBookingById", createdAtResult.ActionName);
        }

        [Fact]
        public async Task UpdateBooking_ValidBooking_ReturnsNoContentResult()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                UserId = "user1",
                CarId = 1,
                TotalPrice = 100,
                IsCancelled = false,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(2)
            };

            _mockRepo.Setup(repo => repo.UpdateBookingAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateBooking(1, booking);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBooking_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.DeleteBookingAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteBooking(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        

    }
}
