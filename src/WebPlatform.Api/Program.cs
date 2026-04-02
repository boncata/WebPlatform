using WebPlatform.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add the controllers.
builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddScoped<IBookService, BookService>();

// Here we should probably add swagger as well. It stopped working
// after we moved to controllers from simple direct api definitions.

var app = builder.Build();

// Map the controllers
app.MapControllers();

// Run the application.
app.Run();

