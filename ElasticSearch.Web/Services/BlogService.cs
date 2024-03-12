using ElasticSearch.Web.Models;
using ElasticSearch.Web.Repository;
using ElasticSearch.Web.ViewModel;

namespace ElasticSearch.Web.Services
{
    /// <summary>
    /// Represents a service for managing Blog entities.
    /// </summary>
    public class BlogService(BlogRepository blogRepository)
    {
        /// <summary>
        /// Saves a blog asynchronously.
        /// </summary>
        /// <param name="createViewModel">The BlogCreateViewModel object containing the blog details to save.</param>
        /// <returns>A task representing the asynchronous save operation. The task will complete with a boolean value indicating whether the save operation was successful.</returns>
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

        /// <summary>
        /// Searches for blogs matching the specified search text.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        /// <returns>A list of blogs that match the search text.</returns>
        public async Task<List<Blog>> SearchAsync(string searchText)
        {
            return await blogRepository.SearchAsync(searchText);
        }
    }
}