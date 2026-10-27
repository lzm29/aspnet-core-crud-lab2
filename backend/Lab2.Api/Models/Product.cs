namespace Lab2.Api.Models;

// Product là Entity được EF Core ánh xạ thành bảng Products trong SQLite.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Price { get; set; }   // Giá tính theo đồng (VND).
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
