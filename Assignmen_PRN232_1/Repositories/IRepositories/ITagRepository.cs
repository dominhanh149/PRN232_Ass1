using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232__.Repositories.IRepositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<PagingResponse<Tag>> GetListPagingAsync(TagSearchDto searchDto);
        Task<Tag?> GetByIdAsync<TKey>(TKey id) where TKey : notnull;
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag> AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Tag tag);
        Task<bool> ExistsByNameAsync(string tagName);
        Task<bool> ExistsByNameAsync(string tagName, int excludeTagId);
        Task<int> SaveChangesAsync();
    }

}
