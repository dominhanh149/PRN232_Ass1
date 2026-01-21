using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232_1.Services.IServices
{
    public interface ICategoryService
    {
        Task<PagingResponse<CategoryDto>> GetListPagingAsync(CategorySearchDto dto);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(short id);

        Task<ApiResponse<CategoryDto>> CreateOrEditAsync(CategorySaveDto dto);
        Task<ApiResponse<bool>> DeleteAsync(short id);
    }
}
