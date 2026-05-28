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

// Allow CORS. We are currently hard-coding the localhost address.
// It works for now, but we should probably develop a more robust solution.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Build the application.
var app = builder.Build();

// Also for CORS usage.
app.UseCors("AllowFrontend");

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

// Expose the Program class. Although such a class is not created in this file,
// due to the ASP.NET workflow, the compiler actually automatically creates such
// a class in the background. Therefore, we use 'partial' here, as we are
// 'attaching' to it. By declaring the Program class here, we expose it to our
// testing framework, which is needed for running integration tests.
public partial class Program { }