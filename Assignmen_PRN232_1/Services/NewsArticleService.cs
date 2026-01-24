using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.DTOs.Common;
using Assignmen_PRN232_1.Services.IServices;
using Mapster;

namespace Assignmen_PRN232_1.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NewsArticleService(INewsArticleRepository newsArticleRepository, ITagRepository tagRepository, IHttpContextAccessor httpContextAccessor)
        {
            _newsArticleRepository = newsArticleRepository;
            _tagRepository = tagRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<NewsArticleDto>> GetAllAsync()
        {
            var newsArticles = await _newsArticleRepository.GetAllAsync();

            // Entity -> DTO
            var result = newsArticles.Adapt<IEnumerable<NewsArticleDto>>();

            return result;
        }

        public async Task<PagingResponse<NewsArticleDto>> GetListPagingAsync(NewsArticleSearchDto dto)
        {
            var pagedData = await _newsArticleRepository.GetListPagingAsync(dto);

            // Eager load Tags cho mỗi NewsArticle
            var items = pagedData.Items.Select(na => new NewsArticleDto
            {
                NewsArticleId = na.NewsArticleId,
                NewsArticleName = na.NewsArticleId,
                NewsTitle = na.NewsTitle,
                Headline = na.Headline,
                CreatedDate = na.CreatedDate,
                NewsContent = na.NewsContent,
                NewsSource = na.NewsSource,
                CategoryId = na.CategoryId,
                CategoryName = na.Category?.CategoryName,
                NewsStatus = na.NewsStatus,
                CreatedById = na.CreatedById,
                CreatedByName = na.CreatedBy?.AccountName,
                UpdatedById = na.UpdatedById,
                ModifiedDate = na.ModifiedDate,
                Tags = na.Tags.Adapt<ICollection<TagDto>>()
            }).ToList();

            return new PagingResponse<NewsArticleDto>
            {
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize,
                TotalRecords = pagedData.TotalRecords,
                Items = items
            };
        }

        public async Task<PagingResponse<NewsArticleDto>> GetPublicListPagingAsync(NewsArticleSearchDto dto)
        {
            var pagedData = await _newsArticleRepository.GetListPagingAsync(dto);

            var items = pagedData.Items
                .Where(na => na.Category != null && na.Category.IsActive == true)
                .Select(na => new NewsArticleDto
                {
                    NewsArticleId = na.NewsArticleId,
                    NewsArticleName = na.NewsArticleId,
                    NewsTitle = na.NewsTitle,
                    Headline = na.Headline,
                    CreatedDate = na.CreatedDate,
                    NewsContent = na.NewsContent,
                    NewsSource = na.NewsSource,
                    CategoryId = na.CategoryId,
                    CategoryName = na.Category?.CategoryName,
                    NewsStatus = na.NewsStatus,
                    CreatedById = na.CreatedById,
                    CreatedByName = na.CreatedBy != null ? na.CreatedBy.AccountName : null,
                    UpdatedById = na.UpdatedById,
                    ModifiedDate = na.ModifiedDate,
                    Tags = na.Tags.Adapt<ICollection<TagDto>>()
                }).ToList();

            return new PagingResponse<NewsArticleDto>
            {
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize,
                TotalRecords = items.Count,
                Items = items
            };
        }

        public async Task<NewsArticleDto?> GetByIdAsync(string id)
        {
            var newsArticle = await _newsArticleRepository.GetByIdAsync(id);
            if (newsArticle == null)
                return null;

            var dto = new NewsArticleDto
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsArticleName = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryId = newsArticle.CategoryId,
                CategoryName = newsArticle.Category?.CategoryName,
                NewsStatus = newsArticle.NewsStatus,
                CreatedById = newsArticle.CreatedById,
                CreatedByName = newsArticle.CreatedBy?.AccountName,
                UpdatedById = newsArticle.UpdatedById,
                ModifiedDate = newsArticle.ModifiedDate,
                Tags = newsArticle.Tags.Adapt<ICollection<TagDto>>()
            };

            return dto;
        }

        /// <summary>
        /// Router - Create hoặc Update dựa vào ID
        /// </summary>
        public async Task<ApiResponse<NewsArticleDto>> CreateOrEditAsync(NewsArticleSaveDto dto)
        {
            return string.IsNullOrEmpty(dto.NewsArticleId)
                ? await CreateAsync(dto)
                : await UpdateAsync(dto);
        }

        #region Private

        /// <summary>
        /// Generate random string ID for NewsArticle
        /// Format: NA + 12 random alphanumeric characters
        /// Example: NA_ABC123XYZ456
        /// </summary>
        private string GenerateNewsArticleId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = "NA_" + new string(Enumerable.Range(0, 12)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
            return result;
        }

        private async Task<ApiResponse<NewsArticleDto>> CreateAsync(NewsArticleSaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Headline))
                return ApiResponse<NewsArticleDto>.Fail("Headline is required");

            if (string.IsNullOrEmpty(dto.NewsArticleId))
            {
                dto.NewsArticleId = GenerateNewsArticleId();
            }

            var exists = await _newsArticleRepository.ExistsByIdAsync(dto.NewsArticleId);
            if (exists)
                return ApiResponse<NewsArticleDto>.Fail("News article ID already exists");

            var entity = dto.Adapt<NewsArticle>();
            entity.CreatedDate = DateTime.Now;
            entity.NewsStatus = entity.NewsStatus ?? true;

            // Gán CreatedByID từ user hiện tại
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && short.TryParse(userId, out var accountId))
            {
                entity.CreatedById = accountId;
            }

            await _newsArticleRepository.AddAsync(entity);
            await _newsArticleRepository.SaveChangesAsync();

            return ApiResponse<NewsArticleDto>.Ok(
                entity.Adapt<NewsArticleDto>(),
                "Created successfully"
            );
        }

        private async Task<ApiResponse<NewsArticleDto>> UpdateAsync(NewsArticleSaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewsArticleId))
                return ApiResponse<NewsArticleDto>.Fail("News article ID is required for update");

            if (string.IsNullOrWhiteSpace(dto.Headline))
                return ApiResponse<NewsArticleDto>.Fail("Headline is required");

            var existing = await _newsArticleRepository.GetByIdAsync(dto.NewsArticleId);
            if (existing == null)
                return ApiResponse<NewsArticleDto>.Fail("News article not found");

            dto.Adapt(existing);
            existing.ModifiedDate = DateTime.Now;

            // Gán UpdatedByID từ user hiện tại
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId) && short.TryParse(userId, out var accountId))
            {
                existing.UpdatedById = accountId;
            }

            await _newsArticleRepository.UpdateAsync(existing);
            await _newsArticleRepository.SaveChangesAsync();

            return ApiResponse<NewsArticleDto>.Ok(
                existing.Adapt<NewsArticleDto>(),
                "Updated successfully"
            );
        }

        #endregion

        public async Task<ApiResponse<NewsArticleDto>> DuplicateAsync(string id)
        {
            try
            {
                var original = await _newsArticleRepository.GetByIdAsync(id);
                if (original == null)
                    return ApiResponse<NewsArticleDto>.Fail("News article not found");

                var tagIds = original.Tags?.Select(t => t.TagId).ToList() ?? new List<int>();

                var duplicate = new NewsArticle
                {
                    NewsArticleId = GenerateNewsArticleId(),
                    NewsTitle = original.NewsTitle + " (Copy)",
                    Headline = original.Headline,
                    NewsContent = original.NewsContent,
                    NewsSource = original.NewsSource,
                    CategoryId = original.CategoryId,
                    NewsStatus = original.NewsStatus,
                    CreatedDate = DateTime.Now
                };

                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId) && short.TryParse(userId, out var accountId))
                {
                    duplicate.CreatedById = accountId;
                }

                await _newsArticleRepository.AddAsync(duplicate);
                await _newsArticleRepository.SaveChangesAsync();

                if (tagIds.Any())
                {
                    var saved = await _newsArticleRepository.GetByIdAsync(duplicate.NewsArticleId);
                    if (saved != null)
                    {
                        foreach (var tagId in tagIds)
                        {
                            var tag = await _tagRepository.GetByIdAsync(tagId);
                            if (tag != null)
                            {
                                saved.Tags.Add(tag);
                            }
                        }
                        await _newsArticleRepository.SaveChangesAsync();
                    }
                }

                return ApiResponse<NewsArticleDto>.Ok(
                    duplicate.Adapt<NewsArticleDto>(),
                    "Duplicated successfully"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<NewsArticleDto>.Fail($"Error duplicating article: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            var newsArticle = await _newsArticleRepository.GetByIdAsync(id);
            if (newsArticle == null)
                return ApiResponse<bool>.Fail("News article not found");

            await _newsArticleRepository.DeleteAsync(newsArticle);
            await _newsArticleRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Deleted successfully");
        }

        public async Task<ApiResponse<bool>> AddTagAsync(string newsArticleId, int tagId)
        {
            // Kiểm tra NewsArticle tồn tại
            var newsArticle = await _newsArticleRepository.GetByIdAsync(newsArticleId);
            if (newsArticle == null)
                return ApiResponse<bool>.Fail("News article not found");

            // Kiểm tra tag đã được thêm chưa
            if (newsArticle.Tags.Any(x => x.TagId == tagId))
                return ApiResponse<bool>.Fail("Tag already added to this article");

            // QUAN TRỌNG: Phải load Tag từ database, không tạo object mới
            var tag = await _tagRepository.GetByIdAsync(tagId);
            if (tag == null)
                return ApiResponse<bool>.Fail("Tag not found");

            newsArticle.Tags.Add(tag);

            await _newsArticleRepository.UpdateAsync(newsArticle);
            await _newsArticleRepository.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Tag added successfully");
        }

        public async Task<ApiResponse<bool>> RemoveTagAsync(string newsArticleId, int tagId)
        {
            var newsArticle = await _newsArticleRepository.GetByIdAsync(newsArticleId);
            if (newsArticle == null)
                return ApiResponse<bool>.Fail("News article not found");

            var tagToRemove = newsArticle.Tags.FirstOrDefault(x => x.TagId == tagId);
            if (tagToRemove == null)
                return ApiResponse<bool>.Fail("Tag not found in this article");

            newsArticle.Tags.Remove(tagToRemove);

            await _newsArticleRepository.UpdateAsync(newsArticle);
            await _newsArticleRepository.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Tag removed successfully");
        }
    }
}
