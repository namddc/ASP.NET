using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bai5.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace bai5.Controllers
{
    public class ProductController : Controller
    {
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

        public ActionResult Index(string? sortOrder, string? currentFilter, string? searchString, int? pageNumber)
        {
            // SẮP XẾP: Lưu kiểu sắp xếp và thiết lập giá trị đảo chiều cho lần click tiếp theo
            ViewBag.CurrentSort = sortOrder; // Kiểu sắp xếp hiện tại
            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "id_asc" ? "id_desc" : "id_asc"; // Đảo chiều ID (tăng <-> giảm)
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";                             // Đảo chiều Name (A-Z <-> Z-A)

            // TÌM KIẾM: Nhận từ khóa tìm kiếm
            if (searchString != null) pageNumber = 1;       // Tìm kiếm từ khóa mới thì quay về trang 1
            else searchString = currentFilter;             // Giữ từ khóa cũ khi chuyển trang
            ViewBag.CurrentFilter = searchString;          // Gửi từ khóa sang View để giữ chữ trong ô input

            var query = from p in products
                        join c in CategoryController.categories on p.CategoryId equals c.CategoryId into gj
                        from subCategory in gj.DefaultIfEmpty()
                        select new ViewModel.ProductViewModel
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.Price,
                            CategoryName = subCategory != null ? subCategory.Name : string.Empty
                        };

            // TÌM KIẾM: Lọc danh sách theo Name hoặc ProductId
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => 
                    (p.Name != null && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) // Tìm theo tên (không phân biệt hoa/thường)
                    || p.ProductId.ToString().Contains(searchString));                                   // Hoặc tìm theo mã ID
            }

            // SẮP XẾP: Thực hiện sắp xếp danh sách theo kiểu đã chọn
            query = sortOrder switch
            {
                "id_desc" => query.OrderByDescending(p => p.ProductId), // Sắp xếp ID giảm dần (từ lớn đến bé)
                "name_asc" => query.OrderBy(p => p.Name),               // Sắp xếp Tên từ A đến Z
                "name_desc" => query.OrderByDescending(p => p.Name),     // Sắp xếp Tên từ Z đến A
                _ => query.OrderBy(p => p.ProductId)                   // Mặc định: ID tăng dần (1, 2, 3...)
            };

            int pageSize = 3;
            int pageIndex = pageNumber ?? 1;
            var pagedProducts = PaginatedList<ViewModel.ProductViewModel>.Create(query, pageIndex, pageSize);

            return View(pagedProducts);
        }

        public ActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) return NotFound();

            var category = CategoryController.categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
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

        // THÊM: Mở form thêm mới sản phẩm (GET)
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name");
            return View();
        }

        // THÊM: Nhận dữ liệu từ form và thêm sản phẩm vào danh sách (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product createproduct)
        {
            if (ModelState.IsValid)
            {
                createproduct.ProductId = products.Count > 0 ? products.Max(p => p.ProductId) + 1 : 1; // Tự động tăng ID
                products.Add(createproduct);            // Thêm sản phẩm vào danh sách
                return RedirectToAction(nameof(Index)); // Chuyển về trang danh sách
            }
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name", createproduct.CategoryId);
            return View(createproduct);
        }

        // SỬA: Mở form chỉnh sửa sản phẩm theo ID (GET)
        public ActionResult Edit(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm cần sửa theo ID
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // SỬA: Cập nhật thông tin sản phẩm mới vào danh sách (POST)
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
                return RedirectToAction(nameof(Index));        // Chuyển về trang danh sách
            }
            ViewBag.CategoryId = new SelectList(CategoryController.categories, "CategoryId", "Name", editproduct.CategoryId);
            return View(editproduct);
        }

        // XÓA: Mở trang xác nhận xóa sản phẩm (GET)
        public ActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm cần xóa theo ID
            if (product == null) return NotFound();

            var category = CategoryController.categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
            ViewBag.CategoryName = category?.Name;
            return View(product);
        }

        // XÓA: Thực hiện xóa sản phẩm khỏi danh sách (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            var product = products.FirstOrDefault(p => p.ProductId == id); // Tìm sản phẩm theo ID
            if (product == null) return NotFound();

            products.Remove(product);               // Xóa sản phẩm khỏi danh sách
            return RedirectToAction(nameof(Index)); // Chuyển về trang danh sách
        }
    }
}
