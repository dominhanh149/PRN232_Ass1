using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.DTOs.Common;
using Assignmen_PRN232_1.Services.IServices;
using Mapster;

namespace Assignmen_PRN232_1.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            
            var result = categories.Adapt<IEnumerable<CategoryDto>>();

            return result;
        }

        public async Task<PagingResponse<CategoryDto>> GetListPagingAsync(CategorySearchDto dto)
        {
            var pagedData = await _categoryRepository.GetListPagingAsync(dto);

            return new PagingResponse<CategoryDto>
            {
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize,
                TotalRecords = pagedData.TotalRecords,
                Items = pagedData.Items
            };
        }

        public async Task<CategoryDto?> GetByIdAsync(short id)
        {
            var category = await _categoryRepository.GetByIdAsync<short>(id);
            if (category == null)
                return null;

            return category.Adapt<CategoryDto>();
        }

        
        
        
        public async Task<ApiResponse<CategoryDto>> CreateOrEditAsync(CategorySaveDto dto)
        {
            return dto.CategoryId == 0
                ? await CreateAsync(dto)
                : await UpdateAsync(dto);
        }

        #region Private

        private async Task<ApiResponse<CategoryDto>> CreateAsync(CategorySaveDto dto)
        {
            
            if (await _categoryRepository.ExistsByNameAndParentAsync(dto.CategoryName, dto.ParentCategoryId))
                return ApiResponse<CategoryDto>.Fail("Category name already exists with the same parent");

            var entity = dto.Adapt<Category>();

            await _categoryRepository.AddAsync(entity);
            await _categoryRepository.SaveChangesAsync();

            return ApiResponse<CategoryDto>.Ok(
                entity.Adapt<CategoryDto>(),
                "Created successfully"
            );
        }

        private async Task<ApiResponse<CategoryDto>> UpdateAsync(CategorySaveDto dto)
        {
            var existing = await _categoryRepository.GetByIdAsync<short>(dto.CategoryId);
            if (existing == null)
                return ApiResponse<CategoryDto>.Fail("Category not found");

            
            if (await _categoryRepository.ExistsByNameAndParentAsync(dto.CategoryName, dto.ParentCategoryId, dto.CategoryId))
                return ApiResponse<CategoryDto>.Fail("Category name already exists with the same parent");

            
            if (existing.ParentCategoryId != dto.ParentCategoryId && existing.NewsArticles != null && existing.NewsArticles.Any())
                return ApiResponse<CategoryDto>.Fail("Cannot change parent category when category has news articles");

            dto.Adapt(existing);

            await _categoryRepository.UpdateAsync(existing);
            await _categoryRepository.SaveChangesAsync();

            return ApiResponse<CategoryDto>.Ok(
                existing.Adapt<CategoryDto>(),
                "Updated successfully"
            );
        }

        #endregion

        public async Task<ApiResponse<bool>> DeleteAsync(short id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<bool>.Fail("Category not found");

            
            if (category.NewsArticles != null && category.NewsArticles.Any())
                return ApiResponse<bool>.Fail("Cannot delete category that has news articles");

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Deleted successfully");
        }
    }
}
