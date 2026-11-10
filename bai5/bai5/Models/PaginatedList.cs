namespace bai5.Models
{
    // Lớp hỗ trợ phân trang generic, kế thừa từ List<T>
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }  // Số thứ tự của trang hiện tại (1, 2, 3...)
        public int TotalPages { get; private set; } // Tổng số trang được tính ra
        public int TotalItems { get; private set; } // Tổng số lượng bản ghi trong toàn bộ danh sách
        public int PageSize { get; private set; }   // Số lượng bản ghi trên một trang (ví dụ: 3)

        // Hàm khởi tạo
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;                                         // Gán trang hiện tại
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);       // Tính tổng số trang: chia làm tròn lên (8/3 = 3)
            TotalItems = count;                                            // Gán tổng số phần tử
            PageSize = pageSize;                                           // Gán số phần tử mỗi trang

            this.AddRange(items);                                          // Đưa các phần tử của trang hiện tại vào List
        }

        public bool HasPreviousPage => PageIndex > 1;          // Có trang trước nếu trang hiện tại lớn hơn 1
        public bool HasNextPage => PageIndex < TotalPages;      // Có trang kế tiếp nếu trang hiện tại nhỏ hơn tổng số trang

        // Hàm static cắt dữ liệu phân trang
        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();                        // Đếm tổng số phần tử
            if (pageIndex < 1) pageIndex = 1;                  // Đảm bảo số trang tối thiểu là 1

            // LINQ: Skip() bỏ qua các trang trước, Take() lấy đúng số phần tử trang hiện tại
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize); // Trả về đối tượng PaginatedList
        }
    }
}
