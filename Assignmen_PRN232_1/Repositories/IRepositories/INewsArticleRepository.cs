using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232__.Repositories.IRepositories
{
    public interface INewsArticleRepository
    {
        Task<NewsArticle?> GetByIdAsync(string id);
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<IEnumerable<NewsArticle>> GetAllWithDetailsAsync();
        Task<NewsArticle?> GetByIdWithDetailsAsync(string id);
        Task<NewsArticle> AddAsync(NewsArticle newsArticle);
        Task UpdateAsync(NewsArticle newsArticle);
        Task DeleteAsync(NewsArticle newsArticle);
        Task<bool> ExistsAsync(string id);
        Task<IEnumerable<NewsArticle>> SearchAsync(string? keyword, short? categoryId, DateTime? fromDate, DateTime? toDate);
        Task<Dictionary<string, int>> GetStatisticsByCategoryAsync();
        Task<int> SaveChangesAsync();
    }
}
