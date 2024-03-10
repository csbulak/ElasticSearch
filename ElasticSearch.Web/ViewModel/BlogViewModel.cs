namespace ElasticSearch.Web.ViewModel
{
    public class BlogViewModel
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string[] Tags { get; set; } = null!;
        public Guid UserId { get; set; }
        public DateTime Created { get; set; }
    }
}