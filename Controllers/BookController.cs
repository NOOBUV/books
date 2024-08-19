using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using books.Models;
using books.Data;

namespace books.Controllers;

public class BookController : Controller {
    private ApplicationDbContext _db;

    public BookController(ApplicationDbContext db) {
        _db = db;

    }

    public IActionResult Index() {
        var books = _db.Books.ToList();
        return View(books);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(BooksEntity book)
    {
        if (ModelState.IsValid)
        {
            _db.Books.Add(book);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(book);
    }

    public IActionResult Edit(int id)
    {
        var book = _db.Books.Find(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    public IActionResult Edit(BooksEntity book)
    {
        if (ModelState.IsValid)
        {
            _db.Books.Update(book);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(book);
    }

    public IActionResult Delete(int id)
    {
        var book = _db.Books.Find(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var book = _db.Books.Find(id);
        if (book != null)
        {
            _db.Books.Remove(book);
            _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }
}