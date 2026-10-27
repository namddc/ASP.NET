using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASP_BAI3.Models;// co cai nay de dung cai model Category 
namespace ASP_BAI3.Controllers
{
    public class CategoryController : Controller
    {
        // giả lập csdl 
        public static List<Category> categories = new List<Models.Category>()
        {
            new Category(){Id=1,Name="laptop 1"},
            new Category(){Id=2,Name="iphon 2"},
            new Category(){Id=3,Name="vivo 3"},
        };
        // GET: CategoryController1
        public ActionResult Index()
        {
            return View(categories);// truyen danh sach categories sang view
        }

        // GET: CategoryController1/Details/5
        public ActionResult Details(int id)
        {
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: CategoryController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                categories.Add(new Category
                {
                    Id = categories.Max(c => c.Id) + 1,
                    Name = collection["Name"]
                });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CategoryController1/Edit/5
        public ActionResult Edit(int id)
        {
            var EditCategory = categories.FirstOrDefault(c => c.Id == id);
            if (EditCategory == null)
            {
                return NotFound();
            }
            return View(EditCategory);
        }

        // POST: CategoryController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
           var exsistingCategory = categories.FirstOrDefault(c => c.Id == id);
            if (exsistingCategory == null)
            {
                return NotFound();
            }
            exsistingCategory.Name = collection["Name"];

            return RedirectToAction(nameof(Index));
        }

        // GET: CategoryController1/Delete/5
        public ActionResult Delete(int id)
        {
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View();
        }

        // POST: CategoryController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            categories.RemoveAll(c => c.Id == id);
            return RedirectToAction(nameof(Index));
            
        }
    }
}
