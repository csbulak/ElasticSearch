using ElasticSearch.Web.Models;
using ElasticSearch.Web.Repository;
using ElasticSearch.Web.ViewModel;

namespace ElasticSearch.Web.Services
{
    public class BlogService(BlogRepository blogRepository)
    {
        public async Task<bool> SaveAsync(BlogCreateViewModel createViewModel)
        {
            var blog = new Blog
            {
                Title = createViewModel.Title,
                Content = createViewModel.Content,
                Tags = createViewModel.Tags.Split(","),
                UserId = Guid.NewGuid()
            };

            var savedBlog = await blogRepository.SaveAsync(blog);

            return savedBlog != null;
        }
    }
}