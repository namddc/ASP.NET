using ASP_BAI3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_BAI3.Controllers
{
    public class productController : Controller
    {
        // giả lập csdl 
        public static List<Models.product> products = new List<Models.product>()
        {
            new Models.product(){Id=1,Name="laptop 1",Price=1000,CategoryId=1,desription="laptop 1"},
            new Models.product(){Id=2,Name="iphon 2",Price=2000,CategoryId=2,desription="iphon 2"},
            new Models.product(){Id=3,Name="vivo 3",Price=3000,CategoryId=3,desription="vivo 3"},
        };
        // GET: productController
        public ActionResult Index()
        {
            return View(products);
        }

        // GET: productController/Details/5
        public ActionResult Details(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: productController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: productController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            products.Add(new product
            {
                Id = products.Count + 1,
                Name = collection["Name"],
                Price = decimal.Parse(collection["Price"]),
                CategoryId = int.Parse(collection["CategoryId"]),
                desription = collection["desription"]
            });
            return RedirectToAction(nameof(Index));
        }

        // GET: productController/Edit/5
        public ActionResult Edit(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: productController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            product.Name = collection["Name"];
            product.Price = decimal.Parse(collection["Price"]);
            product.CategoryId = int.Parse(collection["CategoryId"]);
            product.desription = collection["desription"];
            return RedirectToAction(nameof(Index));
        }

        // GET: productController/Delete/5
        public ActionResult Delete(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: productController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            products.RemoveAll(p => p.Id == id);
            return RedirectToAction(nameof(Index)); 
        }
    }
}
