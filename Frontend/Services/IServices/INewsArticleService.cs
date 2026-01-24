using Assignmen_PRN232__.Dto;
using Assignmen_PRN232_1.DTOs.Common;

namespace Frontend.Services.IServices
{
    public interface INewsArticleService
    {
        Task<PagingResponse<NewsArticleDto>> GetListPagingAsync(NewsArticleSearchDto searchDto);
        Task<PagingResponse<NewsArticleDto>> GetPublicListPagingAsync(NewsArticleSearchDto searchDto);
        Task<List<NewsArticleDto>> GetAllAsync();
        Task<NewsArticleDto?> GetByIdAsync(string id);
        Task<(bool Success, string Message, NewsArticleDto? Data)> CreateOrEditAsync(NewsArticleSaveDto dto);
        Task<(bool Success, string Message, NewsArticleDto? Data)> DuplicateAsync(string id);
        Task<(bool Success, string Message)> DeleteAsync(string id);
        Task<(bool Success, string Message)> AddTagAsync(string newsArticleId, int tagId);
        Task<(bool Success, string Message)> RemoveTagAsync(string newsArticleId, int tagId);
    }
}
