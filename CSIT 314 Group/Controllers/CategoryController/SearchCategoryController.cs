using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchCategoryController : ControllerBase
    {
        private readonly Data.Category _categoryRepository;

        public SearchCategoryController(Data.Category categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SearchCategory([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required");
            }

            var result = await _categoryRepository.SearchCategories(keyword);

            if (result.Count == 0)
            {
                return NotFound("No matching categories found");
            }

            return Ok(result);
        }
    }
}