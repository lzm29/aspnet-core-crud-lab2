using Lab2.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Bật Controller cho các endpoint HTTP GET/POST/PUT/DELETE.
builder.Services.AddControllers();

// 2. Lấy connection string từ appsettings.json và cấu hình EF Core + SQLite.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Thiếu ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// 3. Cho phép frontend Vite ở localhost gọi API từ trình duyệt.
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:5173", "http://127.0.0.1:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("Frontend");
app.MapControllers();

// Endpoint đơn giản kiểm tra backend đã chạy.
app.MapGet("/", () => Results.Ok(new
{
    name = "Lab 2 - ASP.NET Core CRUD API",
    status = "running",
    api = "/api/products"
}));

app.Run();
