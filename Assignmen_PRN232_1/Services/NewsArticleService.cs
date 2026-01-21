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

        public NewsArticleService(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
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

        private async Task<ApiResponse<NewsArticleDto>> CreateAsync(NewsArticleSaveDto dto)
        {
            // Tạo ID mới nếu không có
            if (string.IsNullOrEmpty(dto.NewsArticleId))
            {
                dto.NewsArticleId = Guid.NewGuid().ToString();
            }

            // Kiểm tra xem ID đã tồn tại chưa
            var exists = await _newsArticleRepository.ExistsByIdAsync(dto.NewsArticleId);
            if (exists)
                return ApiResponse<NewsArticleDto>.Fail("News article ID already exists");

            // DTO -> Entity
            var entity = dto.Adapt<NewsArticle>();
            entity.CreatedDate = DateTime.Now;
            entity.NewsStatus = entity.NewsStatus ?? true;

            await _newsArticleRepository.AddAsync(entity);
            await _newsArticleRepository.SaveChangesAsync();

            return ApiResponse<NewsArticleDto>.Ok(
                entity.Adapt<NewsArticleDto>(),
                "Created successfully"
            );
        }

        private async Task<ApiResponse<NewsArticleDto>> UpdateAsync(NewsArticleSaveDto dto)
        {
            var existing = await _newsArticleRepository.GetByIdAsync(dto.NewsArticleId);
            if (existing == null)
                return ApiResponse<NewsArticleDto>.Fail("News article not found");

            // Mapster update object hiện tại
            dto.Adapt(existing);
            existing.ModifiedDate = DateTime.Now;

            await _newsArticleRepository.UpdateAsync(existing);
            await _newsArticleRepository.SaveChangesAsync();

            return ApiResponse<NewsArticleDto>.Ok(
                existing.Adapt<NewsArticleDto>(),
                "Updated successfully"
            );
        }

        #endregion

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

            // Tạo tag object để thêm vào relationship
            var tag = new Tag { TagId = tagId };
            newsArticle.Tags.Add(tag);

            await _newsArticleRepository.UpdateAsync(newsArticle);
            await _newsArticleRepository.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Tag added successfully");
        }

        public async Task<ApiResponse<bool>> RemoveTagAsync(string newsArticleId, int tagId)
        {
            // Kiểm tra NewsArticle tồn tại
            var newsArticle = await _newsArticleRepository.GetByIdAsync(newsArticleId);
            if (newsArticle == null)
                return ApiResponse<bool>.Fail("News article not found");

            // Kiểm tra tag có trong article không
            var tagToRemove = newsArticle.Tags.FirstOrDefault(x => x.TagId == tagId);
            if (tagToRemove == null)
                return ApiResponse<bool>.Fail("Tag not found in this article");

            // Xóa tag
            newsArticle.Tags.Remove(tagToRemove);

            await _newsArticleRepository.UpdateAsync(newsArticle);
            await _newsArticleRepository.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "Tag removed successfully");
        }
    }
}
