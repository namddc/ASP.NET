namespace bai5.Models
{
    // =========================================================================
    // LỚP HỖ TRỢ PHÂN TRANG (PAGINATEDLIST)
    // - Tự xây dựng kế thừa từ List<T> chuẩn theo tài liệu Microsoft
    // - Không cần cài thêm thư viện NuGet nào khác, tương thích 100% mọi bản .NET
    // =========================================================================
    public class PaginatedList<T> : List<T>
    {
        // Trang hiện tại (ví dụ: Trang 1, Trang 2)
        public int PageIndex { get; private set; }

        // Tổng số trang (ví dụ: có 8 sản phẩm, mỗi trang 3 cái -> Tổng có 3 trang)
        public int TotalPages { get; private set; }

        // Tổng số bản ghi (ví dụ: 8 sản phẩm)
        public int TotalItems { get; private set; }

        // Kích thước trang (số bản ghi trên 1 trang, ví dụ: 3)
        public int PageSize { get; private set; }

        // Hàm khởi tạo (Constructor)
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            // Tính tổng số trang: Làm tròn lên (Math.Ceiling), ví dụ: 8 / 3 = 2.66 -> làm tròn thành 3 trang
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalItems = count;
            PageSize = pageSize;

            // Thêm các phần tử của trang hiện tại vào danh sách
            this.AddRange(items);
        }

        // Kiểm tra xem có trang trước đó không (nếu trang hiện tại > 1 thì có trang trước)
        public bool HasPreviousPage => PageIndex > 1;

        // Kiểm tra xem có trang kế tiếp không (nếu trang hiện tại < tổng số trang thì có trang sau)
        public bool HasNextPage => PageIndex < TotalPages;

        // Hàm static tạo đối tượng phân trang từ một tập hợp dữ liệu ban đầu
        public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            if (pageIndex < 1) pageIndex = 1;

            // DÙNG LINQ:
            // - Skip((pageIndex - 1) * pageSize): Bỏ qua các bản ghi của các trang trước đó
            // - Take(pageSize): Lấy đúng số lượng bản ghi của trang hiện tại
            // Ví dụ: Trang 2 (pageIndex = 2, pageSize = 3):
            //   Skip((2 - 1) * 3) = Bỏ qua 3 bản ghi đầu (bản ghi 1, 2, 3)
            //   Take(3) = Lấy 3 bản ghi tiếp theo (bản ghi 4, 5, 6)
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
