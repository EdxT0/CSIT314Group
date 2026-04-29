using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteCategoryController : ControllerBase
    {
        private readonly Data.Category _categoryRepository;

        public DeleteCategoryController(Data.Category categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "platform manager")]
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