using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai5.Models
{
    public class Product
    {
        // [Key]: Chỉ định ProductId là khóa chính (Primary Key)
        [Key]
        public int ProductId { get; set; }

        // [Required]: Bắt buộc phải nhập, không được để trống
        [Required(ErrorMessage = "Bạn chưa nhập name")]
        // [StringLength]: Giới hạn độ dài chuỗi từ 3 đến 100 ký tự
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name phải từ 3 đến 100 ký tự")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập description")]
        [StringLength(500, ErrorMessage = "Description không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập price")]
        // [Range]: Giới hạn giá trị số trong khoảng từ 0.01 đến 100000.00
        [Range(0.01, 100000, ErrorMessage = "Price phải từ 0.01 đến 10000.00")]
        public decimal Price { get; set; }

        // [ForeignKey("Category")]: Đánh dấu CategoryId là khóa ngoại liên kết tới bảng Category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // Navigation Property: Thuộc tính điều hướng để truy xuất đối tượng Category tương ứng
        public Category? Category { get; set; }
    }
}
