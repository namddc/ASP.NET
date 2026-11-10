namespace bai5.ViewModel
{
    // ViewModel kết hợp dữ liệu hiển thị giữa Product và Category
    public class ProductViewModel
    {
        public int ProductId { get; set; }        // Mã sản phẩm
        public string? Name { get; set; }         // Tên sản phẩm
        public string? Description { get; set; }  // Mô tả sản phẩm
        public decimal Price { get; set; }        // Giá sản phẩm
        public string? CategoryName { get; set; } // Tên danh mục lấy từ bảng Category qua phép JOIN
    }
}
