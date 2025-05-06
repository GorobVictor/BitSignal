using System.Security.Claims;
using Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Base;

[Route("Api/[controller]/[action]")]
[Authorize]
[ApiController]
public class BaseController : ControllerBase
{
    protected int UserId
    {
        get
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == Constant.JwtUserId);

            if (userId == null)
                throw new Exception("bad data");

            return Convert.ToInt32(userId.Value);
        }
    }

    protected string UserName
    {
        get
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;

            var claimUserRole = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (claimUserRole == null)
                throw new Exception("bad data");

            return claimUserRole.Value;
        }
    }
}