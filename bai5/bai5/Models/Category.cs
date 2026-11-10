using System.ComponentModel.DataAnnotations;

namespace bai5.Models
{
    public class Category
    {
        // [Key]: Chỉ định CategoryId là khóa chính (Primary Key)
        [Key]
        public int CategoryId { get; set; }

        // [Required]: Bắt buộc nhập tên danh mục
        [Required(ErrorMessage = "Bạn phải nhập name")]
        [StringLength(100, ErrorMessage = "Name không được vượt quá 100 ký tự")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Discription không vượt quá 500 ký tự")]
        public string? Description { get; set; }

        // [DataType(DataType.Date)]: Chỉ định kiểu dữ liệu là Ngày (không lấy giờ/phút)
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Quan hệ 1-N: Một Category có thể chứa danh sách nhiều Product
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
