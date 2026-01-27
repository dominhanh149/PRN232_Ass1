using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Dto.Common;
using Assignmen_PRN232_1.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [Authorize(Roles = "Staff, Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
    }
}
