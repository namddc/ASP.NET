using Microsoft.AspNetCore.Mvc;

namespace ASP1.Controllers
{
    public class bai2Controller : Controller
    {
        public String Greeting()
        {
            return "chào mừng bạn đến với trang web của tôi";
        }
        public String wellcome1(int id, string name)
        {
            return $"xin chào {id}-{ name}, đến với trang web của tôi";  
        }
        // IactionResult    
        public IActionResult Wellcome(int id1 , string name)
        {

            return View();
        }
    }
}
