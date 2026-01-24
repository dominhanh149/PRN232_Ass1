using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232_1.DTOs.Common;

namespace Assignmen_PRN232__.Repositories.IRepositories
{
    public interface ISystemAccountRepository
    {
        Task<IEnumerable<SystemAccount>> GetAllAsync();
        Task<PagingResponse<SystemAccount>> GetListPagingAsync(SystemAccountSearchDto searchDto);
        Task<SystemAccount?> GetByIdAsync<TKey>(TKey id) where TKey : notnull;
        Task<SystemAccount?> GetByIdAsync(short id);
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<SystemAccount> AddAsync(SystemAccount account);
        Task UpdateAsync(SystemAccount account);
        Task DeleteAsync(SystemAccount account);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email, short excludeAccountId);
        Task<int> SaveChangesAsync();
    }
}
