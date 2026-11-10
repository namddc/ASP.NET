using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai5.Models
{
    public class Product
    {
        [Key]                                                                                   // Đánh dấu ProductId là khóa chính (Primary Key)
        public int ProductId { get; set; }                                                      // Mã sản phẩm

        [Required(ErrorMessage = "Bạn chưa nhập name")]                                         // Bắt buộc nhập, không được để trống
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name phải từ 3 đến 100 ký tự")]   // Độ dài chuỗi từ 3 đến 100 ký tự
        public string? Name { get; set; }                                                       // Tên sản phẩm

        [Required(ErrorMessage = "Bạn chưa nhập description")]                                  // Bắt buộc nhập mô tả
        [StringLength(500, ErrorMessage = "Description không được vượt quá 500 ký tự")]          // Tối đa 500 ký tự
        public string? Description { get; set; }                                                // Mô tả sản phẩm

        [Required(ErrorMessage = "Bạn chưa nhập price")]                                        // Bắt buộc nhập giá
        [Range(0.01, 100000, ErrorMessage = "Price phải từ 0.01 đến 10000.00")]                  // Giá trị số từ 0.01 đến 100.000
        public decimal Price { get; set; }                                                      // Giá bán

        [ForeignKey("Category")]                                                                // Khóa ngoại liên kết tới bảng Category
        public int CategoryId { get; set; }                                                     // Mã danh mục

        public Category? Category { get; set; }                                                 // Thuộc tính điều hướng liên kết bảng Category
    }
}
