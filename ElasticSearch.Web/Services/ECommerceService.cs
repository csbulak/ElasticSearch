using ElasticSearch.Web.Repository;
using ElasticSearch.Web.ViewModel;

namespace ElasticSearch.Web.Services
{
    /// <summary>
    /// This class provides functionality to perform eCommerce search operations.
    /// </summary>
    public class ECommerceService(ECommerceRepository eCommerceRepository)
    {
        /// <summary>
        /// Searches for eCommerce data based on the provided search criteria and pagination parameters.
        /// </summary>
        /// <param name="searchViewModel">The search criteria for eCommerce data (optional).</param>
        /// <param name="page">The page number for pagination (starting from 1).</param>
        /// <param name="pageSize">The maximum number of records per page.</param>
        /// <returns>
        /// A tuple containing the list of eCommerce view models, the total count of records matching the search criteria,
        /// and the total number of pages available based on the pagination parameters.
        /// </returns>
        public async Task<(List<ECommerceViewModel> list, long totalCount, long pageLinkCount)> SearchAsync(ECommerceSearchViewModel searchViewModel, int page, int pageSize)
        {
            var result = await eCommerceRepository.SearchAsync(searchViewModel, page, pageSize);

            var pageLinkCalculate = result.count % pageSize;
            long pageLinkCount;

            if (pageLinkCalculate == 0)
            {
                pageLinkCount = result.count / pageSize;
            }
            else
            {
                pageLinkCount = (result.count / pageSize) + 1;
            }

            var eCommerceList = result.list.Select(x => new ECommerceViewModel()
            {
                Category = string.Join(",", x.Category),
                OrderDate = x.OrderDate.ToShortDateString(),
                Id = x.Id,
                CustomerFirstName = x.CustomerFirstName,
                CustomerLastName = x.CustomerLastName,
                CustomerFullName = x.CustomerFullName,
                OrderId = x.OrderId,
                TaxFullTotalPrice = x.TaxFullTotalPrice,
                Gender = x.Gender
            }).ToList();

            return (list: eCommerceList, totalCount: result.count, pageLinkCount);
        }
    }
}