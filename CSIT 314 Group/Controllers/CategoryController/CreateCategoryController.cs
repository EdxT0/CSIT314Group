using CSIT_314_Group.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSIT_314_Group.Controllers.CategoryController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateCategoryController : ControllerBase
    {
        private readonly Category _categoryRepository;

        public CreateCategoryController(Category categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Authorize(Roles = "platform manager")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category createCategoryDto)
        {
            var existingCategory = await _categoryRepository.GetByName(createCategoryDto.Name);

            if (existingCategory != null)
            {
                return Conflict($"Category with name {createCategoryDto.Name} already exists");
            }

            var category = new Category(
                createCategoryDto.Name.ToLower(),
                createCategoryDto.Description
            );

            bool result = await _categoryRepository.CreateCategory(category);

            if (!result)
            {
                return StatusCode(500, "Failed to create category");
            }

            return Ok($"Created category: {category.Name}");
        }
    }
}