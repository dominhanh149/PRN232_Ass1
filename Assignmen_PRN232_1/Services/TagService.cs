using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.DTOs.Common;
using Assignmen_PRN232_1.Services.IServices;
using Mapster;

namespace Assignmen_PRN232_1.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();

            // Entity -> DTO
            var result = tags.Adapt<IEnumerable<TagDto>>();

            return result;
        }

        public async Task<PagingResponse<TagDto>> GetListPagingAsync(TagSearchDto dto)
        {
            var pagedData = await _tagRepository.GetListPagingAsync(dto);

            return new PagingResponse<TagDto>
            {
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize,
                TotalRecords = pagedData.TotalRecords,
                Items = pagedData.Items.Adapt<IEnumerable<TagDto>>()
            };
        }

        public async Task<TagDto?> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return null;

            return tag.Adapt<TagDto>();
        }

        /// <summary>
        /// Router
        /// </summary>
        public async Task<ApiResponse<TagDto>> CreateOrEditAsync(TagSaveDto dto)
        {
            return dto.TagId == 0
                ? await CreateAsync(dto)
                : await UpdateAsync(dto);
        }

        #region Private

        private async Task<ApiResponse<TagDto>> CreateAsync(TagSaveDto dto)
        {
            // Kiểm tra trùng TagName
            var exists = await _tagRepository.ExistsByNameAsync(dto.TagName!);
            if (exists)
                return ApiResponse<TagDto>.Fail("Tag name already exists");

            var entity = dto.Adapt<Tag>();
            
            // Tạo ID tuần tự
            var maxId = (await _tagRepository.GetAllAsync()).MaxBy(x => x.TagId)?.TagId ?? 0;
            entity.TagId = maxId + 1;

            await _tagRepository.AddAsync(entity);
            await _tagRepository.SaveChangesAsync();

            return ApiResponse<TagDto>.Ok(
                entity.Adapt<TagDto>(),
                "Created successfully"
            );
        }

        private async Task<ApiResponse<TagDto>> UpdateAsync(TagSaveDto dto)
        {
            var existing = await _tagRepository.GetByIdAsync(dto.TagId);
            if (existing == null)
                return ApiResponse<TagDto>.Fail("Tag not found");

            // Kiểm tra trùng TagName (trừ chính nó)
            var duplicateExists = await _tagRepository.ExistsByNameAsync(dto.TagName!, dto.TagId);
            if (duplicateExists)
                return ApiResponse<TagDto>.Fail("Tag name already exists");

            dto.Adapt(existing);

            await _tagRepository.UpdateAsync(existing);
            await _tagRepository.SaveChangesAsync();

            return ApiResponse<TagDto>.Ok(
                existing.Adapt<TagDto>(),
                "Updated successfully"
            );
        }

        #endregion

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                return ApiResponse<bool>.Fail("Tag not found");

            // Kiểm tra tag có đang được dùng trong NewsArticle không
            if (tag.NewsArticles != null && tag.NewsArticles.Any())
                return ApiResponse<bool>.Fail("Cannot delete tag that is being used in news articles");

            await _tagRepository.DeleteAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Deleted successfully");
        }
    }
}
