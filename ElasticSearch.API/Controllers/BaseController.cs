using System.Net;
using ElasticSearch.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ElasticSearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ResponseDto<T> responseDto)
        {
            if (responseDto.StatusCode == HttpStatusCode.NoContent)
            {
                return new ObjectResult(null)
                {
                    StatusCode = responseDto.StatusCode.GetHashCode()
                };
            }

            return new ObjectResult(responseDto)
            {
                StatusCode = responseDto.StatusCode.GetHashCode()
            };
        }
    }
}