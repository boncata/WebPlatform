using Microsoft.EntityFrameworkCore;
using WebPlatform.Api.Data;
using WebPlatform.Api.Services;


var builder = WebApplication.CreateBuilder(args);

// Add the controllers.
builder.Services.AddControllers();

// Add support for Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the database context. This uses Entity Framework.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Only use Swagger in development environment.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map the controllers.
app.MapControllers();

// Use secure channel. Currently disabled, while I learn.
// Enable this when feelint more confident: app.UseHttpsRedirection();

// Run the application.
app.Run();

