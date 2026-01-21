using Assignmen_PRN232__.Dto;
using Frontend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // GET: Tag/Index
        public async Task<IActionResult> Index(TagSearchDto dto)
        {
            var searchDto = new TagSearchDto
            {
                PageIndex = dto.PageIndex,
                PageSize = dto.PageSize,
                Keyword = dto.Keyword
            };

            var result = await _tagService.GetListPagingAsync(searchDto);

            // Truyền thêm search params để giữ lại khi phân trang
            ViewBag.CurrentPage = dto.PageIndex;
            ViewBag.PageSize = dto.PageSize;
            ViewBag.Keyword = dto.Keyword;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalRecords = result.TotalRecords;

            return View(result);
        }

        // GET: Tag/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                TempData["ErrorMessage"] = "Tag not found";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        // GET: Tag/Create
        public IActionResult Create()
        {
            return View(new TagSaveDto());
        }

        // GET: Tag/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                TempData["ErrorMessage"] = "Tag not found";
                return RedirectToAction(nameof(Index));
            }

            var saveDto = new TagSaveDto
            {
                TagId = tag.TagId,
                TagName = tag.TagName,
                Note = tag.Note
            };

            return View("CreateEdit", saveDto);
        }

        // POST: Tag/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TagSaveDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateEdit", dto);
            }

            var result = await _tagService.CreateOrEditAsync(dto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Message;
            return View("CreateEdit", dto);
        }

        // POST: Tag/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tagService.DeleteAsync(id);

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
        public async Task<IActionResult> GetCreateEditForm(int? id)
        {
            if (id.HasValue && id > 0)
            {
                // Edit mode
                var tag = await _tagService.GetByIdAsync(id.Value);
                if (tag == null)
                    return BadRequest("Tag not found");

                var saveDto = new TagSaveDto
                {
                    TagId = tag.TagId,
                    TagName = tag.TagName,
                    Note = tag.Note
                };

                return PartialView("_CreateEditForm", saveDto);
            }
            else
            {
                // Create mode
                return PartialView("_CreateEditForm", new TagSaveDto());
            }
        }

        // AJAX: Save tag (create/edit)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTag(TagSaveDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return BadRequest(new { success = false, message = "Invalid input", errors });
            }

            var result = await _tagService.CreateOrEditAsync(dto);

            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }

            return BadRequest(new { success = false, message = result.Message });
        }

        // AJAX: Delete tag
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var result = await _tagService.DeleteAsync(id);

            if (result.Success)
            {
                return Ok(new { success = true, message = result.Message });
            }

            return BadRequest(new { success = false, message = result.Message });
        }
    }
}