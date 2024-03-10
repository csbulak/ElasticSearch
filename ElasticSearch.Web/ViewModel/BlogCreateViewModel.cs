using System.ComponentModel.DataAnnotations;

namespace ElasticSearch.Web.ViewModel
{
    public class BlogCreateViewModel
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        public List<string> Tags { get; set; } = new();
    }
}