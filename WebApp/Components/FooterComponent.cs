using Microsoft.AspNetCore.Mvc;

namespace WebApp.Components
{
    public class FooterComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Components/FooterComponent.cshtml");
        }
    }
}
