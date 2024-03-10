using System.ComponentModel.DataAnnotations;

namespace ElasticSearch.Web.ViewModel
{
    public class ECommerceSearchViewModel
    {
        [Display(Name = "Category")]
        public string? Category { get; set; }
        [Display(Name = "Gender")]
        public string? Gender { get; set; }
        [Display(Name = "Order Start Date")]
        [DataType(DataType.Date)]
        public DateTime? OrderDateStart { get; set; }
        [Display(Name = "Order End Date")]
        [DataType(DataType.Date)]
        public DateTime? OrderDateEnd { get; set; }
        [Display(Name = "Customer Full Name")]
        public string? CustomerFullName { get; set; }
    }
}