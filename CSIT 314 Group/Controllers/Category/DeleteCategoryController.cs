using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteCategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public DeleteCategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "platform manager, admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var existingCategory = await _categoryRepository.GetById(id);

            if (existingCategory == null)
            {
                return NotFound("Category not found");
            }

            bool result = await _categoryRepository.DeleteCategory(id);

            if (!result)
            {
                return StatusCode(500, "Failed to delete category");
            }

            return Ok($"Deleted category with Id {id}");
        }
    }
}