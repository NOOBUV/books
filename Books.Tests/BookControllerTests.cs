using System.Collections.Generic;
using System.Linq;
using books.Controllers;
using books.Data;
using books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Books.Tests
{
    public class BookControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<ApplicationDbContext>(options);
            _controller = new BookController(_mockContext.Object);
        }

        [Fact]
        public void Index_ReturnsAViewResult_WithAListOfBooks()
        {
            // Arrange
            var books = new List<BooksEntity>
            {
                new BooksEntity { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "1234567890" },
                new BooksEntity { Id = 2, Title = "Book 2", Author = "Author 2", ISBN = "0987654321" }
            };
            _mockContext.Setup(db => db.Books).ReturnsDbSet(books);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BooksEntity>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        // Additional tests for Create, Edit, Delete actions...
    }

    public static class DbSetMockingExtensions
    {
        public static DbSet<T> ReturnsDbSet<T>(this Mock<ApplicationDbContext> context, IEnumerable<T> entities) where T : class
        {
            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.AsQueryable().Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.AsQueryable().Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.AsQueryable().ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.AsQueryable().GetEnumerator());
            context.Setup(c => c.Set<T>()).Returns(dbSet.Object);
            return dbSet.Object;
        }
    }
}
