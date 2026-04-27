using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.CategoryDTO;
using CSIT_314_Group.Entity;
using CategoryEntity = CSIT_314_Group.Entity.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.Category
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateCategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;

        public UpdateCategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "platform manager, admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO updateCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetById(updateCategoryDto.Id);

            if (existingCategory == null)
            {
                return NotFound("Category not found");
            }

            var category = new CategoryEntity(
                updateCategoryDto.Name.ToLower(),
                updateCategoryDto.Description
            );

            category.Id = updateCategoryDto.Id;

            bool result = await _categoryRepository.UpdateCategory(category);

            if (!result)
            {
                return StatusCode(500, "Failed to update category");
            }

            return Ok($"Updated category with Id {updateCategoryDto.Id}");
        }
    }
}