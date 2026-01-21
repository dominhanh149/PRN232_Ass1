using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232_1.DTOs.Common;
using System.Collections.Generic;

namespace Frontend.Services.IServices
{
    public interface ITagService
    {
        Task<PagingResponse<TagDto>> GetListPagingAsync(TagSearchDto searchDto);
        Task<List<TagDto>> GetAllAsync();
        Task<TagDto?> GetByIdAsync(int id);
        Task<(bool Success, string Message, TagDto? Data)> CreateOrEditAsync(TagSaveDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
