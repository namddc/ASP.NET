using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bai5.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bai5.Controllers
{
    public class ProductController : Controller
    {
        // Danh sách giả lập Database lưu tạm trong RAM máy tính
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

        // Action hiển thị danh sách: Tìm kiếm + Sắp xếp + Phân trang
        public ActionResult Index(
            string? sortOrder,     // Tham số nhận kiểu sắp xếp (id_asc, id_desc, name_asc, name_desc)
            string? currentFilter, // Tham số giữ từ khóa tìm kiếm khi bấm chuyển trang
            string? searchString,  // Tham số nhận từ khóa người dùng gõ vào ô tìm kiếm
            int? pageNumber)       // Tham số nhận số trang người dùng muốn xem (1, 2, 3...)
        {
            ViewBag.CurrentSort = sortOrder; // Gửi kiểu sắp xếp hiện tại sang View để vẽ mũi tên ▲ / ▼

            // Tạo giá trị đảo chiều: nếu đang tăng (hoặc rỗng) thì click lần tới sẽ thành giảm (id_desc)
            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "id_asc" ? "id_desc" : "id_asc";
            // Đảo chiều sắp xếp tên: nếu đang A-Z (name_asc) thì lần tới sẽ thành Z-A (name_desc)
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            if (searchString != null) // Nếu người dùng vừa gõ từ khóa mới và bấm Search
            {
                pageNumber = 1;       // Bắt buộc quay về trang 1 để xem từ đầu
            }
            else                      // Nếu người dùng đang bấm nút chuyển trang (2, 3...)
            {
                searchString = currentFilter; // Giữ lại từ khóa tìm kiếm cũ trước đó
            }
            ViewBag.CurrentFilter = searchString; // Gửi lại từ khóa sang View để giữ chữ trong ô input

            // Dùng LINQ Join bảng Product với Category để lấy ra tên danh mục (CategoryName)
            var query = from p in products                                        // Lấy từng Product trong danh sách
                        join c in CategoryController.categories on p.CategoryId equals c.CategoryId into gj // Khớp CategoryId
                        from subCategory in gj.DefaultIfEmpty()                   // Left join đề phòng sản phẩm chưa có Category
                        select new ViewModel.ProductViewModel                     // Đổ dữ liệu sang ViewModel để hiển thị
                        {
                            ProductId = p.ProductId,                             // Lấy mã sản phẩm
                            Name = p.Name,                                       // Lấy tên sản phẩm
                            Description = p.Description,                         // Lấy mô tả sản phẩm
                            Price = p.Price,                                     // Lấy giá tiền
                            CategoryName = subCategory != null ? subCategory.Name : string.Empty // Lấy tên danh mục
                        };

            if (!string.IsNullOrEmpty(searchString)) // Nếu có nhập từ khóa tìm kiếm
            {
                query = query.Where(p => 
                    (p.Name != null && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) // Tìm theo tên (không phân biệt hoa/thường)
                    || p.ProductId.ToString().Contains(searchString));                                   // Hoặc tìm theo mã ID sản phẩm
            }

            query = sortOrder switch // Thực hiện sắp xếp danh sách theo kiểu đã chọn
            {
                "id_desc" => query.OrderByDescending(p => p.ProductId), // Sắp xếp ID giảm dần (từ lớn đến bé)
                "name_asc" => query.OrderBy(p => p.Name),               // Sắp xếp Tên từ A đến Z
                "name_desc" => query.OrderByDescending(p => p.Name),     // Sắp xếp Tên từ Z đến A
                _ => query.OrderBy(p => p.ProductId)                   // Mặc định: ID tăng dần (1, 2, 3...)
            };

            int pageSize = 3;                // Quy định mỗi trang chỉ hiển thị đúng 3 sản phẩm
            int pageIndex = pageNumber ?? 1; // Nếu không chọn trang thì mặc định lấy trang 1
            // Cắt đúng 3 sản phẩm của trang tương ứng bằng hàm Skip() và Take()
            var pagedProducts = PaginatedList<ViewModel.ProductViewModel>.Create(query, pageIndex, pageSize);

            return View(pagedProducts);      // Trả 3 sản phẩm của trang đó ra View
        }

        // Action xem chi tiết sản phẩm theo ID
        public ActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm có ProductId khớp với id truyền vào
            if (product == null) return NotFound();                        // Không thấy thì trả về lỗi 404

            var category = CategoryController.categories.FirstOrDefault(c => c.CategoryId == product.CategoryId); // Tìm danh mục tương ứng
            var productViewModel = new ViewModel.ProductViewModel          // Tạo đối tượng ViewModel chứa đầy đủ thông tin
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryName = category?.Name                              // Lấy tên danh mục để xem chi tiết
            };
            return View(productViewModel);
        }

        // Action mở giao diện form thêm mới (GET)
        public ActionResult Create()
        {
            // Tạo danh sách dropdown chọn Category: Value là CategoryId, Text hiển thị là Name
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name");
            return View();
        }

        // Action nhận dữ liệu gửi lên từ form thêm mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công giả mạo CSRF
        public ActionResult Create(Product createproduct)
        {
            if (ModelState.IsValid) // Kiểm tra dữ liệu có hợp lệ theo các ràng buộc ở Model không
            {
                // Tự động sinh ID: lấy ID to nhất trong danh sách cộng 1
                createproduct.ProductId = products.Count > 0 ? products.Max(p => p.ProductId) + 1 : 1;
                products.Add(createproduct);            // Thêm sản phẩm mới vào danh sách
                return RedirectToAction(nameof(Index)); // Thêm xong chuyển về trang danh sách
            }
            // Nếu dữ liệu lỗi thì nạp lại dropdown Category và giữ lại trang form để người dùng sửa lỗi
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name", createproduct.CategoryId);
            return View(createproduct);
        }

        // Action mở giao diện form chỉnh sửa sản phẩm (GET)
        public ActionResult Edit(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm cần sửa theo ID
            if (product == null) return NotFound();

            // Nạp dropdown Category và chọn sẵn CategoryId hiện tại của sản phẩm
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // Action nhận dữ liệu cập nhật từ form sửa gửi lên (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product editproduct)
        {
            if (ModelState.IsValid)
            {
                var product = products.FirstOrDefault(p => p.ProductId == editproduct.ProductId); // Tìm sản phẩm gốc trong danh sách
                if (product == null) return NotFound();

                product.Name = editproduct.Name;               // Cập nhật tên mới
                product.Description = editproduct.Description; // Cập nhật mô tả mới
                product.Price = editproduct.Price;             // Cập nhật giá mới
                product.CategoryId = editproduct.CategoryId;   // Cập nhật danh mục mới
                return RedirectToAction(nameof(Index));        // Sửa xong chuyển về trang danh sách
            }
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name", editproduct.CategoryId);
            return View(editproduct);
        }

        // Action mở giao diện xác nhận xóa sản phẩm (GET)
        public ActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm cần xóa
            if (product == null) return NotFound();

            var category = CategoryController.categories.FirstOrDefault(c => c.CategoryId == product.CategoryId); // Tìm tên Category để hiện lên
            ViewBag.CategoryName = category?.Name;
            return View(product);
        }

        // Action thực hiện xóa sản phẩm khi người dùng ấn nút xác nhận xóa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm theo id
            if (product == null) return NotFound();

            products.Remove(product);               // Xóa sản phẩm ra khỏi danh sách
            return RedirectToAction(nameof(Index)); // Xóa xong chuyển về trang danh sách
        }
    }
}
