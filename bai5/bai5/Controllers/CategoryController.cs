using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using bai5.Models;

namespace bai5.Controllers
{
    public class CategoryController : Controller
    {
        // =========================================================================
        // 1. GIẢ LẬP CƠ SỞ DỮ LIỆU DANH MỤC (CATEGORY) TRONG BỘ NHỚ RAM
        // =========================================================================
        public static List<Category> categories = new List<Category>
        {
            new Category { CategoryId = 1, Name = "Category 1", Description = "Description 1", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 2, Name = "Category 2", Description = "Description 2", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 3, Name = "Category 3", Description = "Description 3", CreatedDate = new DateTime(2026, 10, 31)},
            new Category { CategoryId = 4, Name = "Category 4", Description = "Description 4", CreatedDate = new DateTime(2026, 11, 1)},
            new Category { CategoryId = 5, Name = "Category 5", Description = "Description 5", CreatedDate = new DateTime(2026, 11, 2)}
        };

        // =========================================================================
        // 2. ACTION INDEX: HIỂN THỊ DANH SÁCH + TÌM KIẾM + SẮP XẾP + PHÂN TRANG
        // =========================================================================
        public ActionResult Index(string? sortOrder, string? currentFilter, string? searchString, int? pageNumber)
        {   
            // Lưu và đảo trạng thái sắp xếp cho lần click tiếp theo
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = string.IsNullOrEmpty(sortOrder) || sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewBag.NameSortParm = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            // Nếu người dùng gõ tìm kiếm mới -> quay về trang 1; nếu không -> giữ từ khóa cũ
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            // Khởi tạo truy vấn danh sách
            IEnumerable<Category> query = categories;

            // Tìm kiếm theo Name (không phân biệt hoa thường) hoặc CategoryId
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => (c.Name != null && c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                                      || c.CategoryId.ToString().Contains(searchString));
            }

            // Sắp xếp dữ liệu theo lựa chọn
            query = sortOrder switch
            {
                "id_desc" => query.OrderByDescending(c => c.CategoryId),
                "name_asc" => query.OrderBy(c => c.Name),
                "name_desc" => query.OrderByDescending(c => c.Name),
                _ => query.OrderBy(c => c.CategoryId) // Mặc định ID tăng dần
            };

            // Phân trang: Mỗi trang 3 bản ghi
            int pageSize = 3;
            int pageIndex = pageNumber ?? 1;
            var pagedCategories = PaginatedList<Category>.Create(query, pageIndex, pageSize);

            return View(pagedCategories);
        }

        // =========================================================================
        // 3. ACTION DETAILS: XEM CHI TIẾT 1 DANH MỤC THEO ID
        // =========================================================================
        public ActionResult Details(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // =========================================================================
        // 4. ACTION CREATE (GET): HIỂN THỊ FORM THÊM MỚI DANH MỤC
        // =========================================================================
        public ActionResult Create()
        {
            return View();
        }

        // =========================================================================
        // 5. ACTION CREATE (POST): NHẬN DỮ LIỆU VÀ LƯU VÀO DANH SÁCH
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // Tự động tăng CategoryId
                category.CategoryId = categories.Count > 0 ? categories.Max(c => c.CategoryId) + 1 : 1;
                categories.Add(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // =========================================================================
        // 6. ACTION EDIT (GET): HIỂN THỊ FORM SỬA DANH MỤC
        // =========================================================================
        public ActionResult Edit(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // =========================================================================
        // 7. ACTION EDIT (POST): CẬP NHẬT DANH MỤC
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category category)
        {
            var editcategory = categories.FirstOrDefault(c => c.CategoryId == id);
            if (editcategory == null)
            {
                return NotFound();
            }
            editcategory.Name = category.Name;
            editcategory.Description = category.Description;
            editcategory.CreatedDate = category.CreatedDate;
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================
        // 8. ACTION DELETE (GET): HIỂN THỊ XÁC NHẬN XÓA DANH MỤC
        // =========================================================================
        public ActionResult Delete(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // =========================================================================
        // 9. ACTION DELETE (POST): THỰC HIỆN XÓA DANH MỤC
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            categories.RemoveAll(c => c.CategoryId == id);
            return RedirectToAction(nameof(Index));
        }
    }
}
