using Assignmen_PRN232__.Dto;
using Frontend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class NewsArticleController : Controller
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public NewsArticleController(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        // GET: NewsArticle/Index
        public async Task<IActionResult> Index(NewsArticleSearchDto dto)
        {
            var searchDto = new NewsArticleSearchDto
            {
                PageIndex = dto.PageIndex > 0 ? dto.PageIndex : 1,
                PageSize = dto.PageSize > 0 ? dto.PageSize : 10,
                Keyword = dto.Keyword,
                Title = dto.Title,
                Author = dto.Author,
                CategoryId = dto.CategoryId,
                Status = dto.Status,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate
            };

            var result = await _newsArticleService.GetListPagingAsync(searchDto);

            ViewBag.Categories = await _categoryService.GetAllAsync();

            ViewBag.CurrentPage = searchDto.PageIndex;
            ViewBag.PageSize = searchDto.PageSize;
            ViewBag.Keyword = searchDto.Keyword;
            ViewBag.Title = searchDto.Title;
            ViewBag.Author = searchDto.Author;
            ViewBag.CategoryId = searchDto.CategoryId;
            ViewBag.Status = searchDto.Status;
            ViewBag.FromDate = searchDto.FromDate;
            ViewBag.ToDate = searchDto.ToDate;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalRecords = result.TotalRecords;

            return View(result);
        }

        // GET: NewsArticle/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View("CreateEdit", new NewsArticleSaveDto());
        }

        // GET: NewsArticle/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var newsArticle = await _newsArticleService.GetByIdAsync(id);
            if (newsArticle == null)
            {
                TempData["ErrorMessage"] = "News Article not found";
                return RedirectToAction(nameof(Index));
            }

            var saveDto = new NewsArticleSaveDto
            {
                NewsArticleId = newsArticle.NewsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource,
                CategoryId = newsArticle.CategoryId,
                NewsStatus = newsArticle.NewsStatus,
                CreatedById = newsArticle.CreatedById,
                UpdatedById = newsArticle.UpdatedById,
                ModifiedDate = newsArticle.ModifiedDate
            };

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View("CreateEdit", saveDto);
        }

        // POST: NewsArticle/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(NewsArticleSaveDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                var errorMessage = string.Join("; ", errors.Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = "Invalid input: " + errorMessage;
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return RedirectToAction(nameof(Index));
            }

            var result = await _newsArticleService.CreateOrEditAsync(dto);

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

        // POST: NewsArticle/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _newsArticleService.DeleteAsync(id);

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
        public async Task<IActionResult> GetCreateEditForm(string? id)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;

            if (!string.IsNullOrEmpty(id))
            {
                // Edit mode - load data từ API
                var newsArticle = await _newsArticleService.GetByIdAsync(id);
                if (newsArticle == null)
                    return BadRequest("News Article not found");

                var saveDto = new NewsArticleSaveDto
                {
                    NewsArticleId = newsArticle.NewsArticleId,
                    NewsTitle = newsArticle.NewsTitle,
                    Headline = newsArticle.Headline,
                    CreatedDate = newsArticle.CreatedDate,
                    NewsContent = newsArticle.NewsContent,
                    NewsSource = newsArticle.NewsSource,
                    CategoryId = newsArticle.CategoryId,
                    NewsStatus = newsArticle.NewsStatus,
                    CreatedById = newsArticle.CreatedById,
                    UpdatedById = newsArticle.UpdatedById,
                    ModifiedDate = newsArticle.ModifiedDate
                };

                return PartialView("_CreateEditForm", saveDto);
            }
            else
            {
                // Create mode - form trống
                return PartialView("_CreateEditForm", new NewsArticleSaveDto());
            }
        }

        // AJAX: Get all tags for dropdown
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var tags = await _newsArticleService.GetAllTagsAsync();
            return Json(tags);
        }

        // AJAX: Add tag to news article
        [HttpPost]
        public async Task<IActionResult> AddTag(string id, int tagId)
        {
            var result = await _newsArticleService.AddTagAsync(id, tagId);
            return Json(new { success = result.Success, message = result.Message });
        }

        // AJAX: Remove tag from news article
        [HttpPost]
        public async Task<IActionResult> RemoveTag(string id, int tagId)
        {
            var result = await _newsArticleService.RemoveTagAsync(id, tagId);
            return Json(new { success = result.Success, message = result.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Duplicate(string id)
        {
            var result = await _newsArticleService.DuplicateAsync(id);

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
    }
}
