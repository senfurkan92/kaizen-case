using Microsoft.AspNetCore.Mvc;

namespace WebApp.Components
{
    public class HeaderComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        { 
            return View("~/Components/HeaderComponent.cshtml");
        }
    }
}
