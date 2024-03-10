using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    public class BlogController(BlogService blogService) : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel createViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["result"] = "Model Validate Değil.";
                return View(createViewModel);
            }

            var result = await blogService.SaveAsync(createViewModel);

            if (result)
            {
                TempData["result"] = "Kaydetme Başarılı";
                return RedirectToAction("Save", "Blog");
            }

            TempData["result"] = "Kaydetme Başarısız";

            return View(createViewModel);
        }
    }
}