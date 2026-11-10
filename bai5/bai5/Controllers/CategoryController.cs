using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bai5.Models;

namespace bai5.Controllers
{
    public class CategoryController : Controller
    {
        // Danh sách giả lập Database Category trong RAM
        public static List<Category> categories = new List<Category>
        {
            new Category { CategoryId = 1, Name = "Category 1", Description = "Description 1", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 2, Name = "Category 2", Description = "Description 2", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 3, Name = "Category 3", Description = "Description 3", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 4, Name = "Category 4", Description = "Description 4", CreatedDate = new DateTime(2026, 11, 1)},
            new Category { CategoryId = 5, Name = "Category 5", Description = "Description 5", CreatedDate = new DateTime(2026, 11, 2)}
        };

        // Action hiển thị danh sách: Tìm kiếm + Sắp xếp + Phân trang
        public ActionResult Index(
            string? sortOrder,     // Tham số kiểu sắp xếp
            string? currentFilter, // Từ khóa cũ khi chuyển trang
            string? searchString,  // Từ khóa mới nhập vào ô tìm kiếm
            int? pageNumber)       // Số trang hiện tại
        {   
            ViewBag.CurrentSort = sortOrder; // Gửi kiểu sắp xếp sang View để hiện mũi tên
            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "id_asc" ? "id_desc" : "id_asc"; // Đảo chiều ID
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";                             // Đảo chiều Name

            if (searchString != null) pageNumber = 1;       // Nếu tìm kiếm từ khóa mới thì về trang 1
            else searchString = currentFilter;             // Nếu đang chuyển trang thì giữ từ khóa cũ
            ViewBag.CurrentFilter = searchString;          // Lưu từ khóa vào ViewBag để hiển thị lại trên ô input

            IEnumerable<Category> query = categories;      // Khởi tạo danh sách nguồn

            if (!string.IsNullOrEmpty(searchString))       // Nếu có nhập từ khóa tìm kiếm
            {
                query = query.Where(c => 
                    (c.Name != null && c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) // Tìm theo tên
                    || c.CategoryId.ToString().Contains(searchString));                                  // Hoặc tìm theo ID
            }

            query = sortOrder switch                       // Sắp xếp danh sách
            {
                "id_desc" => query.OrderByDescending(c => c.CategoryId), // ID giảm dần
                "name_asc" => query.OrderBy(c => c.Name),               // Tên A -> Z
                "name_desc" => query.OrderByDescending(c => c.Name),     // Tên Z -> A
                _ => query.OrderBy(c => c.CategoryId)                   // Mặc định ID tăng dần
            };

            int pageSize = 3;                              // Mỗi trang hiện 3 danh mục
            int pageIndex = pageNumber ?? 1;               // Mặc định trang 1
            var pagedCategories = PaginatedList<Category>.Create(query, pageIndex, pageSize); // Phân trang bằng Skip/Take

            return View(pagedCategories);                  // Trả kết quả ra View
        }

        // Xem chi tiết danh mục theo ID
        public ActionResult Details(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục theo ID
            if(category == null) return NotFound();
            return View(category);
        }

        // Mở form thêm mới (GET)
        public ActionResult Create()
        {
            return View();
        }

        // Lưu danh mục mới từ form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid) // Dữ liệu hợp lệ
            {
                category.CategoryId = categories.Count > 0 ? categories.Max(c => c.CategoryId) + 1 : 1; // Tự tăng ID
                categories.Add(category);               // Thêm vào danh sách
                return RedirectToAction(nameof(Index)); // Chuyển về trang danh sách
            }
            return View(category);
        }

        // Mở form sửa danh mục (GET)
        public ActionResult Edit(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục cần sửa
            if(category == null) return NotFound();
            return View(category);
        }

        // Cập nhật danh mục (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {
            var editcategory = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục gốc
            if (editcategory == null) return NotFound();

            editcategory.Name = category.Name;               // Gán tên mới
            editcategory.Description = category.Description; // Gán mô tả mới
            editcategory.CreatedDate = category.CreatedDate; // Gán ngày mới
            return RedirectToAction(nameof(Index));          // Chuyển về trang danh sách
        }

        // Mở trang xác nhận xóa (GET)
        public ActionResult Delete(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id); // Tìm danh mục cần xóa
            if (category == null) return NotFound();
            return View(category);
        }

        // Thực hiện xóa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            categories.RemoveAll(c => c.CategoryId == id); // Xóa danh mục khỏi danh sách
            return RedirectToAction(nameof(Index));        // Chuyển về trang danh sách
        }
    }
}
