using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232_1.Services.IServices
{
    public interface ITagService
    {
        Task<PagingResponse<TagDto>> GetListPagingAsync(TagSearchDto dto);
        Task<IEnumerable<TagDto>> GetAllAsync();
        Task<TagDto?> GetByIdAsync(int id);

        Task<ApiResponse<TagDto>> CreateOrEditAsync(TagSaveDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
