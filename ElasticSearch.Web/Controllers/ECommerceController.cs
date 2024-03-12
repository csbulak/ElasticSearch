using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ECommerceController"/> class.
    /// </summary>
    /// <param name="eCommerceService">The eCommerce service.</param>
    public class ECommerceController(ECommerceService eCommerceService) : Controller
    {
        // GET: ECommerceController
        /// <summary>
        /// Searches for products based on the search criteria provided.
        /// </summary>
        /// <param name="searchPageViewModel">The search criteria provided by the user.</param>
        /// <returns>An ActionResult representing the search results view.</returns>
        public async Task<ActionResult> Search([FromQuery] SearchPageViewModel searchPageViewModel)
        {
            var result = await eCommerceService.SearchAsync(searchPageViewModel.SearchViewModel, searchPageViewModel.Page, searchPageViewModel.PageSize);

            searchPageViewModel.List = result.list;
            searchPageViewModel.PageLinkCount = result.pageLinkCount;
            searchPageViewModel.TotalCount = result.totalCount;

            return View(searchPageViewModel);
        }
    }
}