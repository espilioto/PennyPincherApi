using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PennyPincher.Services.Models.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PennyPincher.Web.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2")]
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
}
