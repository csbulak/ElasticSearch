using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    /// <summary>
    /// Controller class for managing blog operations.
    /// </summary>
    public class BlogController(BlogService blogService) : Controller
    {
        // GET
        /// <summary>
        /// Saves a blog post asynchronously.
        /// </summary>
        /// <param name="createViewModel">The view model containing the blog post data.</param>
        /// <returns>Returns a task representing the asynchronous operation. The task result is true if the blog post was saved successfully, false otherwise.</returns>
        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        /// <summary>
        /// Saves a blog post.
        /// </summary>
        /// <param name="createViewModel">The view model containing the details of the blog post to be saved.</param>
        /// <returns>A boolean indicating whether the save operation was successful.</returns>
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

        /// <summary>
        /// Performs a search based on the provided search text.
        /// </summary>
        /// <param name="searchText">The search text to use.</param>
        /// <returns>A list of Blog objects that match the search criteria.</returns>
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            return View(await blogService.SearchAsync(string.Empty));
        }

        /// <summary>
        /// Performs a search operation based on the provided search text.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.
        /// The task result contains a list of <see cref="Blog"/> objects that match the search text.</returns>
        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            ViewBag.searchText = searchText;
            return View(await blogService.SearchAsync(searchText));
        }
    }
}