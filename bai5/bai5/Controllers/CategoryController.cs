using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bai5.Models;

namespace bai5.Controllers
{
    public class CategoryController : Controller
    {
        public static List<Category> categories = new List<Category>
        {
            new Category { CategoryId = 1, Name = "Category 1", Description = "Description 1", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 2, Name = "Category 2", Description = "Description 2", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 3, Name = "Category 3", Description = "Description 3", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 4, Name = "Category 4", Description = "Description 4", CreatedDate = new DateTime(2026, 11, 1)},
            new Category { CategoryId = 5, Name = "Category 5", Description = "Description 5", CreatedDate = new DateTime(2026, 11, 2)}
        };

        public ActionResult Index(string? sortOrder, string? currentFilter, string? searchString, int? pageNumber)
        {   
            // SẮP XẾP: Lưu kiểu sắp xếp và thiết lập giá trị đảo chiều cho lần click tiếp theo
            ViewBag.CurrentSort = sortOrder; // Kiểu sắp xếp hiện tại
            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "id_asc" ? "id_desc" : "id_asc"; // Đảo chiều ID (tăng <-> giảm)
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";                             // Đảo chiều Name (A-Z <-> Z-A)

            // TÌM KIẾM: Nhận từ khóa tìm kiếm
            if (searchString != null) pageNumber = 1;       // Tìm kiếm từ khóa mới thì về trang 1
            else searchString = currentFilter;             // Giữ từ khóa cũ khi chuyển trang
            ViewBag.CurrentFilter = searchString;          // Gửi từ khóa sang View để giữ chữ trong ô input

            IEnumerable<Category> query = categories;

            // TÌM KIẾM: Lọc danh sách theo Name hoặc CategoryId
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => 
                    (c.Name != null && c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) // Tìm theo tên (không phân biệt hoa/thường)
                    || c.CategoryId.ToString().Contains(searchString));                                  // Hoặc tìm theo ID
            }

            // SẮP XẾP: Thực hiện sắp xếp danh sách theo kiểu đã chọn
            query = sortOrder switch
            {
                "id_desc" => query.OrderByDescending(c => c.CategoryId), // Sắp xếp ID giảm dần (từ lớn đến bé)
                "name_asc" => query.OrderBy(c => c.Name),               // Sắp xếp Tên từ A đến Z
                "name_desc" => query.OrderByDescending(c => c.Name),     // Sắp xếp Tên từ Z đến A
                _ => query.OrderBy(c => c.CategoryId)                   // Mặc định: ID tăng dần (1, 2, 3...)
            };

            int pageSize = 3;
            int pageIndex = pageNumber ?? 1;
            var pagedCategories = PaginatedList<Category>.Create(query, pageIndex, pageSize);

            return View(pagedCategories);
        }

        public ActionResult Details(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if(category == null) return NotFound();
            return View(category);
        }

        // THÊM: Mở form thêm mới danh mục (GET)
        public ActionResult Create()
        {
            return View();
        }

        // THÊM: Nhận dữ liệu từ form và thêm danh mục vào danh sách (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CategoryId = categories.Count > 0 ? categories.Max(c => c.CategoryId) + 1 : 1; // Tự động tăng ID
                categories.Add(category);               // Thêm vào danh sách
                return RedirectToAction(nameof(Index)); // Chuyển về trang danh sách
            }
            return View(category);
        }

        // SỬA: Mở form chỉnh sửa danh mục theo ID (GET)
        public ActionResult Edit(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục cần sửa theo ID
            if(category == null) return NotFound();
            return View(category);
        }

        // SỬA: Cập nhật thông tin danh mục mới vào danh sách (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {
            var editcategory = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục gốc trong danh sách
            if (editcategory == null) return NotFound();

            editcategory.Name = category.Name;               // Cập nhật tên mới
            editcategory.Description = category.Description; // Cập nhật mô tả mới
            editcategory.CreatedDate = category.CreatedDate; // Cập nhật ngày mới
            return RedirectToAction(nameof(Index));          // Chuyển về trang danh sách
        }

        // XÓA: Mở trang xác nhận xóa danh mục (GET)
        public ActionResult Delete(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục cần xóa theo ID
            if (category == null) return NotFound();
            return View(category);
        }

        // XÓA: Thực hiện xóa danh mục khỏi danh sách (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            categories.RemoveAll(c => c.CategoryId == id); // Xóa danh mục khỏi danh sách
            return RedirectToAction(nameof(Index));        // Chuyển về trang danh sách
        }
    }
}
