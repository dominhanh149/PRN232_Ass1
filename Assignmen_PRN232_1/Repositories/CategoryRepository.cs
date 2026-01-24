using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.Data;
using Assignmen_PRN232_1.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Assignmen_PRN232__.Repositories
{
    public class CategoryRepository
    : BaseRepository<Category, AppDbContext>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context, IUnitOfWork unitOfWork)
            : base(context, unitOfWork)
        {
        }

        public async Task<PagingResponse<CategoryDto>> GetListPagingAsync(CategorySearchDto searchDto)
        {
            var query = FindAll();

            
            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.Trim();
                query = query.Where(x =>
                    x.CategoryName.Contains(keyword) ||
                    (x.CategoryDesciption != null && x.CategoryDesciption.Contains(keyword)));
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(x => x.IsActive == searchDto.Status.Value);
            }

            var totalRecords = await query.CountAsync();

            
            var items = await query
                .OrderByDescending(x => x.CategoryId)
                .Skip((searchDto.PageIndex - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .GroupJoin(
                    FindAll(),
                    c => c.ParentCategoryId,
                    p => p.CategoryId,
                    (c, parents) => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        CategoryDesciption = c.CategoryDesciption,
                        ParentCategoryId = c.ParentCategoryId,
                        ParentCategoryName = parents.Select(p => p.CategoryName).FirstOrDefault(),
                        IsActive = c.IsActive
                    }
                )
                .ToListAsync();

            return new PagingResponse<CategoryDto>
            {
                PageIndex = searchDto.PageIndex,
                PageSize = searchDto.PageSize,
                TotalRecords = totalRecords,
                Items = items
            };
        }

        public async Task<bool> ExistsByNameAsync(string categoryName)
        {
            return await ExistsAsync(c => c.CategoryName == categoryName);
        }

        public async Task<bool> ExistsByNameAndParentAsync(string categoryName, short? parentCategoryId, short excludeCategoryId = 0)
        {
            return await ExistsAsync(c => 
                c.CategoryName == categoryName && 
                c.ParentCategoryId == parentCategoryId &&
                c.CategoryId != excludeCategoryId);
        }

        
        public Task<Category?> GetByIdAsync(short id) => GetByIdAsync<short>(id);
    }
}
