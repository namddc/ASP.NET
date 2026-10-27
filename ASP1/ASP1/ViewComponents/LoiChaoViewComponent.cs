
using Microsoft.AspNetCore.Mvc;

namespace ASP1.ViewComponents
{
    public class LoiChaoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return Content("Xin chào, đây là View Component!");
        }
    }
}
