using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bai5.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bai5.Controllers
{
    public class ProductController : Controller
    {
        // =========================================================================
        // 1. GIẢ LẬP CƠ SỞ DỮ LIỆU (IN-MEMORY DATABASE)
        // Dùng biến static để lưu dữ liệu tạm thời trong bộ nhớ RAM khi chạy ứng dụng
        // =========================================================================
        public static List<Product> products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Product 1", Description = "Description 1", Price = 10.0m, CategoryId = 1 },
            new Product { ProductId = 2, Name = "Product 2", Description = "Description 2", Price = 20.0m, CategoryId = 2 },
            new Product { ProductId = 3, Name = "Product 3", Description = "Description 3", Price = 30.0m, CategoryId = 1 },
            new Product { ProductId = 4, Name = "Product 4", Description = "Description 4", Price = 40.0m, CategoryId = 3 },
            new Product { ProductId = 5, Name = "Product 5", Description = "Description 5", Price = 50.0m, CategoryId = 2 },
            new Product { ProductId = 6, Name = "Product 6", Description = "Description 6", Price = 60.0m, CategoryId = 1 },
            new Product { ProductId = 7, Name = "Product 7", Description = "Description 7", Price = 70.0m, CategoryId = 2 },
            new Product { ProductId = 8, Name = "Product 8", Description = "Description 8", Price = 80.0m, CategoryId = 3 }
        };

        // =========================================================================
        // 2. ACTION INDEX: HIỂN THỊ DANH SÁCH + TÌM KIẾM + SẮP XẾP + PHÂN TRANG
        // Các tham số nhận vào từ URL (Query String):
        // - sortOrder: Kiểu sắp xếp hiện tại (ví dụ: id_desc, name_asc, name_desc)
        // - currentFilter: Từ khóa tìm kiếm đang áp dụng (dùng khi bấm chuyển trang)
        // - searchString: Từ khóa mới người dùng nhập vào form tìm kiếm
        // - pageNumber: Số trang người dùng muốn xem (ví dụ: trang 1, trang 2)
        // =========================================================================
        public ActionResult Index(string? sortOrder, string? currentFilter, string? searchString, int? pageNumber)
        {
            // --- BƯỚC 1: XỬ LÝ SẮP XẾP (SORTING) ---
            // Lưu lại kiểu sắp xếp hiện tại để View hiển thị mũi tên ▲ / ▼
            ViewBag.CurrentSort = sortOrder;

            // Thiết lập giá trị đảo chiều sắp xếp cho lần click tiếp theo:
            // Nếu hiện tại đang là tăng dần (hoặc rỗng) -> lần click tới sẽ là giảm dần (id_desc)
            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "id_asc" ? "id_desc" : "id_asc";
            // Nếu hiện tại đang là A-Z (name_asc) -> lần click tới sẽ là Z-A (name_desc)
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            // --- BƯỚC 2: XỬ LÝ TÌM KIẾM (FILTERING / SEARCH) ---
            // Nếu người dùng nhập từ khóa mới: Bắt buộc reset trang về trang 1
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                // Nếu người dùng đang bấm chuyển trang: Giữ nguyên từ khóa cũ (currentFilter)
                searchString = currentFilter;
            }
            // Gửi từ khóa sang View để giữ lại chữ trong ô input và gắn vào link phân trang
            ViewBag.CurrentFilter = searchString;

            // --- BƯỚC 3: DÙNG LINQ KẾT HỢP DỮ LIỆU (JOIN BẢNG PRODUCT VỚI CATEGORY) ---
            // Vì bảng Product chỉ có CategoryId (khóa ngoại), ta cần Join với bảng Category
            // để lấy ra CategoryName (tên danh mục) đổ vào ViewModel
            var query = from p in products
                        join c in CategoryController.categories on p.CategoryId equals c.CategoryId into gj
                        from subCategory in gj.DefaultIfEmpty() // Left Join đề phòng sản phẩm chưa có danh mục
                        select new ViewModel.ProductViewModel
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            CategoryName = subCategory != null ? subCategory.Name : string.Empty
                        };

            // --- BƯỚC 4: LỌC DANH SÁCH THEO TỪ KHÓA TÌM KIẾM ---
            if (!string.IsNullOrEmpty(searchString))
            {
                // Lọc theo Name (không phân biệt hoa thường) HOẶC lọc theo ProductId
                query = query.Where(p => (p.Name != null && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                      || p.ProductId.ToString().Contains(searchString));
            }

            // --- BƯỚC 5: THỰC HIỆN SẮP XẾP DANH SÁCH ---
            query = sortOrder switch
            {
                "id_desc" => query.OrderByDescending(p => p.ProductId), // ID giảm dần (từ lớn đến bé)
                "name_asc" => query.OrderBy(p => p.Name),               // Tên từ A -> Z
                "name_desc" => query.OrderByDescending(p => p.Name),     // Tên từ Z -> A
                _ => query.OrderBy(p => p.ProductId)                   // Mặc định: ID tăng dần (1, 2, 3...)
            };

            // --- BƯỚC 6: PHÂN TRANG (PAGINATION) ---
            int pageSize = 3;                  // Số bản ghi hiển thị trên mỗi trang (ở đây đặt là 3)
            int pageIndex = pageNumber ?? 1;   // Nếu không truyền số trang thì mặc định lấy trang 1
            
            // Gọi lớp PaginatedList để dùng Skip() và Take() cắt đúng 3 bản ghi của trang đó
            var pagedProducts = PaginatedList<ViewModel.ProductViewModel>.Create(query, pageIndex, pageSize);

            // Trả về View danh sách đã phân trang
            return View(pagedProducts);
        }

        // =========================================================================
        // 3. ACTION DETAILS: XEM CHI TIẾT 1 SẢN PHẨM THEO ID
        // =========================================================================
        public ActionResult Details(int id)
        {
            // Tìm sản phẩm theo id
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(); // Trả về trang 404 nếu không tìm thấy
            }

            // Lấy tên danh mục tương ứng của sản phẩm đó
            var category = CategoryController.categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);

            // Gán dữ liệu sang ProductViewModel để hiển thị ra View
            var productViewModel = new ViewModel.ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = category?.Name
            };
            return View(productViewModel);
        }

        // =========================================================================
        // 4. ACTION CREATE (GET): HIỂN THỊ FORM THÊM MỚI SẢN PHẨM
        // =========================================================================
        public ActionResult Create()
        {
            // Tạo SelectList (danh sách dropdown chọn Category) truyền sang View qua ViewBag
            // Tham số: Nguồn danh sách, trường làm giá trị (CategoryId), trường hiển thị (Name)
            ViewBag.CategoryId = new SelectList(
                CategoryController.categories, "CategoryId", "Name");
            return View();
        }

        // =========================================================================
        // 5. ACTION CREATE (POST): NHẬN DỮ LIỆU TỪ FORM VÀ LƯU VÀO DANH SÁCH
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công giả mạo yêu cầu chéo (CSRF)
        public ActionResult Create(Product createproduct)
        {
            // Kiểm tra tính hợp lệ của dữ liệu theo các DataAnnotations khai báo ở Model Product
            if (ModelState.IsValid)
            {
                // Tự động tăng ID: lấy ID lớn nhất hiện tại cộng thêm 1
                createproduct.ProductId = products.Count > 0 ? products.Max(p => p.ProductId) + 1 : 1;
                products.Add(createproduct); // Thêm vào danh sách
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách
            }

            // Nếu dữ liệu không hợp lệ (ví dụ để trống tên, sai giá tiền):
            // Phải nạp lại SelectList danh mục rồi hiển thị lại form kèm thông báo lỗi
            ViewBag.CategoryId = new SelectList(
                CategoryController.categories, "CategoryId", "Name", createproduct.CategoryId);
            return View(createproduct);
        }

        // =========================================================================
        // 6. ACTION EDIT (GET): HIỂN THỊ FORM CHỈNH SỬA SẢN PHẨM THEO ID
        // =========================================================================
        public ActionResult Edit(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Đổ danh sách Category vào dropdown và chọn sẵn CategoryId hiện tại của sản phẩm
            ViewBag.CategoryId = new SelectList(
                CategoryController.categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // =========================================================================
        // 7. ACTION EDIT (POST): CẬP NHẬT THÔNG TIN SẢN PHẨM
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product editproduct)
        {
            if (ModelState.IsValid)
            {
                // Tìm sản phẩm gốc trong danh sách theo ProductId
                var product = products.FirstOrDefault(p => p.ProductId == editproduct.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                // Cập nhật các thông tin mới từ form vào sản phẩm gốc
                product.Name = editproduct.Name;
                product.Description = editproduct.Description;
                product.Price = editproduct.Price;
                product.CategoryId = editproduct.CategoryId;

                return RedirectToAction(nameof(Index));
            }

            // Nếu dữ liệu form không hợp lệ, nạp lại SelectList và giữ lại trang sửa
            ViewBag.CategoryId = new SelectList(
                CategoryController.categories, "CategoryId", "Name", editproduct.CategoryId);
            return View(editproduct);
        }

        // =========================================================================
        // 8. ACTION DELETE (GET): HIỂN THỊ TRANG XÁC NHẬN XÓA SẢN PHẨM
        // =========================================================================
        public ActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Lấy tên Category để hiển thị lên thông tin xác nhận xóa
            var category = CategoryController.categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
            ViewBag.CategoryName = category?.Name;

            return View(product);
        }

        // =========================================================================
        // 9. ACTION DELETE (POST): THỰC HIỆN XÓA SẢN PHẨM KHỎI DANH SÁCH
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Xóa sản phẩm khỏi List
            products.Remove(product);
            return RedirectToAction(nameof(Index));
        }
    }
}
