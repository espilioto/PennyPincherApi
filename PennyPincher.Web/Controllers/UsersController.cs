using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PennyPincher.Services.User.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PennyPincher.Web.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: api/<UsersController>
    [HttpGet]
    public async Task<List<User>> Get()
    {
        return await _userManager.Users.Select(x => new User { Username = x.UserName }).ToListAsync();
    }

    // GET api/<UsersController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<UsersController>
    [HttpPost]
    public async Task<ActionResult<User>> Post([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userManager.CreateAsync(
            new IdentityUser() { UserName = user.Username, Email = user.Email }, user.Password
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Created(string.Empty, user);
    }

    // POST api/<UsersController>/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest user)
    {
        var identityUser = await _userManager.FindByEmailAsync(user.Email);
        if (identityUser == null || !await _userManager.CheckPasswordAsync(identityUser, user.Password))
            return Unauthorized();

        var jwtKey = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Key"];
        var jwtIssuer = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Issuer"];
        var jwtTtlHours = int.Parse(HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:TtlHours"]!);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, identityUser.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, identityUser.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(jwtTtlHours),
            signingCredentials: creds);

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    // PUT api/<UsersController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<UsersController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
