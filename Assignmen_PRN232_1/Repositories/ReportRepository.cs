using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.Data;
using Microsoft.EntityFrameworkCore;

namespace Assignmen_PRN232__.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _dbContext;

        public ReportRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<NewsArticleReportDto> GetNewsArticleReportAsync(ReportSearchDto searchDto)
        {
            var query = _dbContext.NewsArticles
                .Include(x => x.Category)
                .Include(x => x.CreatedBy)
                .AsNoTracking();

            
            if (searchDto.CategoryId.HasValue && searchDto.CategoryId > 0)
            {
                query = query.Where(x => x.CategoryId == searchDto.CategoryId);
            }

            
            if (searchDto.AuthorId.HasValue && searchDto.AuthorId > 0)
            {
                query = query.Where(x => x.CreatedById == searchDto.AuthorId);
            }

            
            if (searchDto.Status.HasValue)
            {
                query = query.Where(x => x.NewsStatus == searchDto.Status);
            }

            
            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate >= searchDto.FromDate);
            }
            if (searchDto.ToDate.HasValue)
            {
                query = query.Where(x => x.CreatedDate <= searchDto.ToDate);
            }

            var articles = await query.ToListAsync();

            var report = new NewsArticleReportDto
            {
                TotalArticles = articles.Count,
                TotalActive = articles.Count(x => x.NewsStatus == true),
                TotalInactive = articles.Count(x => x.NewsStatus == false),

                
                CategoryStats = articles
                    .GroupBy(x => new { x.CategoryId, x.Category?.CategoryName })
                    .Select(g => new CategoryStatDto
                    {
                        CategoryId = g.Key.CategoryId,
                        CategoryName = g.Key.CategoryName ?? "N/A",
                        ArticleCount = g.Count(),
                        ActiveCount = g.Count(x => x.NewsStatus == true),
                        InactiveCount = g.Count(x => x.NewsStatus == false)
                    })
                    .OrderByDescending(x => x.ArticleCount)
                    .ToList(),

                
                AuthorStats = articles
                    .GroupBy(x => new { x.CreatedById, x.CreatedBy?.AccountName })
                    .Select(g => new AuthorStatDto
                    {
                        AuthorId = g.Key.CreatedById,
                        AuthorName = g.Key.AccountName ?? "N/A",
                        ArticleCount = g.Count(),
                        ActiveCount = g.Count(x => x.NewsStatus == true),
                        InactiveCount = g.Count(x => x.NewsStatus == false)
                    })
                    .OrderByDescending(x => x.ArticleCount)
                    .ToList()
            };

            return report;
        }
    }
}
