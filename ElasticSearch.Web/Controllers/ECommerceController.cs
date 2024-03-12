using ElasticSearch.Web.Services;
using ElasticSearch.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.Web.Controllers
{
    public class ECommerceController(ECommerceService eCommerceService) : Controller
    {
        // GET: ECommerceController
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