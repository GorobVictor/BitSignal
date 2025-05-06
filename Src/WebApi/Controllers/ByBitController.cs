using ByBitApi.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers;

public class ByBitController(IByBitService byBitSvc) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> WalletBalance()
    {
        return this.Ok(await byBitSvc.GetBalance());
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Ping()
    {
        await byBitSvc.CheckIsStarted();
        return this.Ok();
    }
}