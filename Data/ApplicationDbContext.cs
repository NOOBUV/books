namespace books.Data;
using Microsoft.EntityFrameworkCore;
using books.Models;

public class ApplicationDbContext: DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) {

    }

    public DbSet<BooksEntity> Books {get;}
}