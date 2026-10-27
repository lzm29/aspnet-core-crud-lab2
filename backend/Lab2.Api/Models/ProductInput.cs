using System.ComponentModel.DataAnnotations;

namespace Lab2.Api.Models;

// DTO: dữ liệu frontend gửi lên khi tạo/cập nhật sản phẩm.
public class ProductInput
{
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
    [StringLength(120, ErrorMessage = "Tên tối đa 120 ký tự.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
    public string? Description { get; set; }

    [Range(0, long.MaxValue, ErrorMessage = "Giá không được âm.")]
    public long Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm.")]
    public int Quantity { get; set; }
}
