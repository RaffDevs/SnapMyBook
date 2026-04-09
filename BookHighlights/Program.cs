var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<BookHighlights.Data.AppDbContext>();

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();

app.Run();
