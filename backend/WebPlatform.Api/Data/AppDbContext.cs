using Microsoft.EntityFrameworkCore;
using WebPlatform.Api.Models;

namespace WebPlatform.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

    public DbSet<Book> Books { get; set; }
}