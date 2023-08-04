using Microsoft.AspNetCore.Mvc;
using SecuredApiTest.Models;
using SecuredApiTest.Services;
using System.Security.Policy;
using System.Threading.Tasks;

namespace SecuredApiTest.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService AuthRepo;
        public AuthController(IAuthService AuthRepo)
        {
            this.AuthRepo = AuthRepo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var result = await AuthRepo.Register(model);
            if (!result.IsAuthenticated) { return BadRequest(result.Message); }

            return Ok(result);
            // Or we can make an anonymous object if we don't return all the object in it.
            //return Ok(new {token = result.Token, expiredOn = result.ExpiredOn});
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await AuthRepo.GetToken(model);
            if (!result.IsAuthenticated) { return BadRequest(result.Message); }
            return Ok(result);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole(AddRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await AuthRepo.AddROle(model);
            if (!string.IsNullOrEmpty(result)) { return BadRequest(result); }
            return Ok(model);
        }
    }
}
