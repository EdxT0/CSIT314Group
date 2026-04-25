using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewAllCategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public ViewAllCategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ViewAllCategory()
        {
            var result = await _categoryRepository.ViewAllCategories();

            if (result.Count == 0)
            {
                return NotFound("No categories found");
            }

            return Ok(result);
        }
    }
}