using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewAllCategoryController : ControllerBase
    {
        private readonly Data.Category _categoryRepository;

        public ViewAllCategoryController(Data.Category categoryRepository)
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