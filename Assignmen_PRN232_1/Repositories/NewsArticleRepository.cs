using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.Data;
using Assignmen_PRN232_1.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Assignmen_PRN232__.Repositories
{
    public class NewsArticleRepository
    : BaseRepository<NewsArticle, AppDbContext>, INewsArticleRepository
    {
        public NewsArticleRepository(AppDbContext context, IUnitOfWork unitOfWork)
            : base(context, unitOfWork)
        {
        }

        public async Task<PagingResponse<NewsArticle>> GetListPagingAsync(NewsArticleSearchDto searchDto)
        {
            // Use AsNoTracking vì đây là read-only operation
            var query = _dbContext.Set<NewsArticle>()
                .Include(x => x.Tags)
                .AsNoTracking();

            // 🔍 Search theo Keyword
            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.Trim();
                query = query.Where(x =>
                    x.NewsTitle.Contains(keyword) ||
                    x.Headline.Contains(keyword) ||
                    (x.NewsContent != null && x.NewsContent.Contains(keyword)) ||
                    (x.NewsSource != null && x.NewsSource.Contains(keyword)));
            }

            // 🔍 Search theo CategoryId
            if (searchDto.CategoryId.HasValue && searchDto.CategoryId > 0)
            {
                query = query.Where(x => x.CategoryId == searchDto.CategoryId);
            }

            // 🔍 Search theo Status
            if (searchDto.Status.HasValue)
            {
                query = query.Where(x => x.NewsStatus == searchDto.Status);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((searchDto.PageIndex - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return new PagingResponse<NewsArticle>
            {
                PageIndex = searchDto.PageIndex,
                PageSize = searchDto.PageSize,
                TotalRecords = totalRecords,
                Items = items
            };
        }

        public async Task<bool> ExistsByIdAsync(string newsArticleId)
        {
            return await ExistsAsync(x => x.NewsArticleId == newsArticleId);
        }

        // Explicit implementation cho string GetById - Include Tags with Change Tracking
        public async Task<NewsArticle?> GetByIdAsync(string id)
        {
            return await _dbContext.Set<NewsArticle>()
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.NewsArticleId == id);
        }
    }
}
