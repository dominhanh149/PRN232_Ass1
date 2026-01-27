using Assignmen_PRN232__.Dto;
using Assignmen_PRN232__.Enums;
using Assignmen_PRN232_1.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assignmen_PRN232_1.Controllers.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class SystemAccountsController : ControllerBase
    {

    }
}
