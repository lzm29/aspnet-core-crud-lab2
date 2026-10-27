using Lab2.Api.Data;
using Lab2.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/products - Lấy danh sách tất cả sản phẩm.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _db.Products
            .AsNoTracking()
            .OrderByDescending(p => p.Id)
            .ToListAsync();
        return Ok(products);
    }

    // GET /api/products/1 - Lấy chi tiết sản phẩm theo ID.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return product is null ? NotFound(new { message = "Không tìm thấy sản phẩm." }) : Ok(product);
    }

    // POST /api/products - Tạo sản phẩm mới và lưu vào database.
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] ProductInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            return BadRequest(new { message = "Tên sản phẩm không được để trống." });

        var product = new Product
        {
            Name = input.Name.Trim(),
            Description = input.Description?.Trim(),
            Price = input.Price,
            Quantity = input.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    // PUT /api/products/1 - Cập nhật sản phẩm đã tồn tại.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
            return BadRequest(new { message = "Tên sản phẩm không được để trống." });

        var product = await _db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = "Không tìm thấy sản phẩm." });

        product.Name = input.Name.Trim();
        product.Description = input.Description?.Trim();
        product.Price = input.Price;
        product.Quantity = input.Quantity;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/products/1 - Xóa sản phẩm khỏi database.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
            return NotFound(new { message = "Không tìm thấy sản phẩm." });

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
