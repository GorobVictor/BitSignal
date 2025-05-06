using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApi.Controllers.Base;
using WebApi.Model.Account;

namespace WebApi.Controllers;

public class AccountController(IUserRepository userRepository) : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await userRepository.Login(request.UserName, request.Password);
        if (user is null) return this.NotFound();

        var accessToken = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: Constant.Issuer,
            audience: Constant.Audience,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Constant.JwtSecret),
                SecurityAlgorithms.HmacSha256),
            claims:
            [
                new Claim(Constant.JwtUserId, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            ]
        ));

        return this.Ok(new LoginResponse(accessToken));
    }
}