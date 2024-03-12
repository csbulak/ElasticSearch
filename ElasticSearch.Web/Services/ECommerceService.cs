using ElasticSearch.Web.Repository;
using ElasticSearch.Web.ViewModel;

namespace ElasticSearch.Web.Services
{
    public class ECommerceService(ECommerceRepository eCommerceRepository)
    {
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