namespace bai5.ViewModel
{
    // =========================================================================
    // PRODUCT VIEWMODEL: DÙNG ĐỂ HIỂN THỊ DỮ LIỆU RA VIEW
    // Vì bảng Product chỉ lưu CategoryId (khóa ngoại), khi hiển thị danh sách
    // người dùng cần nhìn thấy CategoryName (tên danh mục), nên ta tạo ViewModel này
    // =========================================================================
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; } // Chứa tên danh mục sau khi JOIN
    }
}
