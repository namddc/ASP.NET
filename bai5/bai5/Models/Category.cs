using System.ComponentModel.DataAnnotations;

namespace bai5.Models
{
    public class Category
    {
        [Key]                                                                          // Khóa chính của bảng Category
        public int CategoryId { get; set; }                                            // Mã danh mục

        [Required(ErrorMessage = "Bạn phải nhập name")]                                // Bắt buộc nhập tên danh mục
        [StringLength(100, ErrorMessage = "Name không được vượt quá 100 ký tự")]        // Tối đa 100 ký tự
        public string? Name { get; set; }                                              // Tên danh mục

        [StringLength(500, ErrorMessage = "Discription không vượt quá 500 ký tự")]      // Tối đa 500 ký tự
        public string? Description { get; set; }                                       // Mô tả danh mục

        [DataType(DataType.Date)]                                                      // Chỉ định định dạng Ngày
        public DateTime CreatedDate { get; set; } = DateTime.Now;                      // Ngày tạo, mặc định lấy ngày hiện tại

        public List<Product> Products { get; set; } = new List<Product>();             // Quan hệ 1 danh mục có nhiều sản phẩm
    }
}
