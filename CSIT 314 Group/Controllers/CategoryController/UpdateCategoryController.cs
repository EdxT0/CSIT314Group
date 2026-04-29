using CSIT_314_Group.Data;
using CSIT_314_Group.DTO.CategoryDTO;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateCategoryController : ControllerBase
    {
        private readonly Data.Category _categoryRepository;

        public UpdateCategoryController(Data.Category categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "platform manager")]
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDTO updateCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetById(updateCategoryDto.Id);

            if (existingCategory == null)
            {
                return NotFound("Category not found");
            }

            var category = new Category(
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