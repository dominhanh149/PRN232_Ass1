using Assignmen_PRN232__.Dto;
using Frontend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class SystemAccountController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        // GET: SystemAccount/Index
        public async Task<IActionResult> Index(SystemAccountSearchDto dto)
        {
            var searchDto = new SystemAccountSearchDto
            {
                PageIndex = dto.PageIndex > 0 ? dto.PageIndex : 1,
                PageSize = dto.PageSize > 0 ? dto.PageSize : 10,
                Keyword = dto.Keyword,
                AccountRole = dto.AccountRole
            };

            var result = await _systemAccountService.GetListPagingAsync(searchDto);

            // Truyền thêm search params để giữ lại khi phân trang
            ViewBag.CurrentPage = searchDto.PageIndex;
            ViewBag.PageSize = searchDto.PageSize;
            ViewBag.Keyword = searchDto.Keyword;
            ViewBag.AccountRole = searchDto.AccountRole;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalRecords = result.TotalRecords;

            return View(result);
        }

        // GET: SystemAccount/Create
        public IActionResult Create()
        {
            return View("CreateEdit", new SystemAccountSaveDto());
        }

        // GET: SystemAccount/Edit/5
        public async Task<IActionResult> Edit(short id)
        {
            var account = await _systemAccountService.GetByIdAsync(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = "System Account not found";
                return RedirectToAction(nameof(Index));
            }

            var saveDto = new SystemAccountSaveDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            };

            return View("CreateEdit", saveDto);
        }

        // POST: SystemAccount/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SystemAccountSaveDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                var errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = "Invalid input: " + errorMessage;
                return RedirectToAction(nameof(Index));
            }

            var result = await _systemAccountService.CreateOrEditAsync(dto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: SystemAccount/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _systemAccountService.DeleteAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get form for create/edit modal
        [HttpGet]
        public async Task<IActionResult> GetCreateEditForm(short? id)
        {
            if (id.HasValue && id > 0)
            {
                // Edit mode - load data từ API
                var account = await _systemAccountService.GetByIdAsync(id.Value);
                if (account == null)
                    return BadRequest("System Account not found");

                var saveDto = new SystemAccountSaveDto
                {
                    AccountId = account.AccountId,
                    AccountName = account.AccountName,
                    AccountEmail = account.AccountEmail,
                    AccountRole = account.AccountRole
                };

                return PartialView("_CreateEditForm", saveDto);
            }
            else
            {
                // Create mode - form trống
                return PartialView("_CreateEditForm", new SystemAccountSaveDto());
            }
        }
    }
}
