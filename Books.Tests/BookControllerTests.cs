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

        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var book = new BooksEntity { Title = "New Book", Author = "New Author", ISBN = "1234567890" };

            // Act
            var result = _controller.Create(book);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Create_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");
            var book = new BooksEntity();

            // Act
            var result = _controller.Create(book);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(book, viewResult.Model);
        }

        [Fact]
        public void Edit_ValidId_ReturnsViewResultWithBook()
        {
            // Arrange
            var book = new BooksEntity { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "1234567890" };
            _mockContext.Setup(db => db.Books.Find(1)).Returns(book);

            // Act
            var result = _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BooksEntity>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public void Edit_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockContext.Setup(db => db.Books.Find(1)).Returns((BooksEntity)null);

            // Act
            var result = _controller.Edit(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var book = new BooksEntity { Id = 1, Title = "Updated Book", Author = "Updated Author", ISBN = "1234567890" };

            // Act
            var result = _controller.Edit(book);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Edit_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");
            var book = new BooksEntity();

            // Act
            var result = _controller.Edit(book);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(book, viewResult.Model);
        }

        [Fact]
        public void Delete_ValidId_ReturnsViewResultWithBook()
        {
            // Arrange
            var book = new BooksEntity { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "1234567890" };
            _mockContext.Setup(db => db.Books.Find(1)).Returns(book);

            // Act
            var result = _controller.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BooksEntity>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public void Delete_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockContext.Setup(db => db.Books.Find(1)).Returns((BooksEntity)null);

            // Act
            var result = _controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            // Arrange
            var book = new BooksEntity { Id = 1, Title = "Book 1", Author = "Author 1", ISBN = "1234567890" };
            _mockContext.Setup(db => db.Books.Find(1)).Returns(book);

            // Act
            var result = _controller.DeleteConfirmed(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
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