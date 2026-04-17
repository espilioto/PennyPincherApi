using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PennyPincher.Contracts.Users;
using PennyPincher.Data;
using PennyPincher.Services.Accounts;
using PennyPincher.Services.Categories;
using PennyPincher.Services.Statements;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PennyPincher.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IStatementsService _statementsService;
    private readonly IAccountService _accountService;
    private readonly ICategoriesService _categoriesService;
    private readonly PennyPincherApiDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<IdentityUser> userManager,
        IStatementsService statementsService,
        IAccountService accountService,
        ICategoriesService categoriesService,
        PennyPincherApiDbContext context,
        IConfiguration configuration,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _statementsService = statementsService;
        _accountService = accountService;
        _categoriesService = categoriesService;
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ErrorOr<List<UserResponse>>> GetAllAsync()
    {
        try
        {
            var users = await _userManager.Users
                .Select(x => new UserResponse(x.Email!))
                .ToListAsync();

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var result = await _userManager.CreateAsync(
                new IdentityUser
                {
                    UserName = request.Email,
                    Email = request.Email
                },
                request.Password);

            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(code: e.Code, description: e.Description)).ToList();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if (identityUser is null || !await _userManager.CheckPasswordAsync(identityUser, request.Password))
                return Error.Unauthorized(description: "Invalid email or password.");

            var jwtKey = _configuration["Jwt:Key"]!;
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtTtlHours = int.Parse(_configuration["Jwt:TtlHours"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Id),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(jwtTtlHours),
                signingCredentials: creds);

            return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token));
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.NotFound(description: "User not found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(code: e.Code, description: e.Description)).ToList();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }

    public async Task<ErrorOr<bool>> DeleteAsync(string userId, DeleteAccountRequest request)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Error.NotFound(description: "User not found");

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return Error.Validation(code: "PasswordMismatch", description: "Incorrect password.");

            await using var tx = await _context.Database.BeginTransactionAsync();

            var statementsPurge = await _statementsService.DeleteAllByUserAsync(userId);
            if (statementsPurge.IsError)
                return statementsPurge.Errors;

            var accountsPurge = await _accountService.DeleteAllByUserAsync(userId);
            if (accountsPurge.IsError)
                return accountsPurge.Errors;

            var categoriesPurge = await _categoriesService.DeleteAllByUserAsync(userId);
            if (categoriesPurge.IsError)
                return categoriesPurge.Errors;

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(code: e.Code, description: e.Description)).ToList();

            await tx.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Message}", ex.Message);
            return Error.Unexpected(description: ex.Message);
        }
    }
}
