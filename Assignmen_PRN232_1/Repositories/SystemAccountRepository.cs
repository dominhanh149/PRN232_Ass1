using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Models;
using Assignmen_PRN232__.Repositories.IRepositories;
using Assignmen_PRN232_1.Data;
using Assignmen_PRN232_1.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Assignmen_PRN232__.Repositories
{
    public class SystemAccountRepository
    : BaseRepository<SystemAccount, AppDbContext>, ISystemAccountRepository
    {
        public SystemAccountRepository(AppDbContext context, IUnitOfWork unitOfWork)
            : base(context, unitOfWork)
        {
        }

        public async Task<PagingResponse<SystemAccount>> GetListPagingAsync(SystemAccountSearchDto searchDto)
        {
            var query = FindAll();

            // 🔍 Search theo Keyword
            if (!string.IsNullOrWhiteSpace(searchDto.Keyword))
            {
                var keyword = searchDto.Keyword.Trim();
                query = query.Where(x =>
                    x.AccountName.Contains(keyword) ||
                    x.AccountEmail.Contains(keyword));
            }

            // 🔍 Filter theo Role
            if (searchDto.AccountRole.HasValue && searchDto.AccountRole >= 0)
            {
                query = query.Where(x => x.AccountRole == searchDto.AccountRole);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.AccountId)
                .Skip((searchDto.PageIndex - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            return new PagingResponse<SystemAccount>
            {
                PageIndex = searchDto.PageIndex,
                PageSize = searchDto.PageSize,
                TotalRecords = totalRecords,
                Items = items
            };
        }

        public async Task<SystemAccount?> GetByEmailAsync(string email)
        {
            return await FindAll()
                .FirstOrDefaultAsync(x => x.AccountEmail == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await ExistsAsync(x => x.AccountEmail == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email, short excludeAccountId)
        {
            return await ExistsAsync(x => x.AccountEmail == email && x.AccountId != excludeAccountId);
        }

        // Explicit implementation cho short GetById
        public Task<SystemAccount?> GetByIdAsync(short id) => GetByIdAsync<short>(id);
    }
}
