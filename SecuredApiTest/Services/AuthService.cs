using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecuredApiTest.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



namespace SecuredApiTest.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly Jwt _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<Jwt> jwt,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._jwt = jwt.Value;
        }


        public async Task<AuthModel> Register(RegisterModel model)
        {
            if (await userManager.FindByEmailAsync(model.Email) != null)
            {
                return new AuthModel { Message = "That Email already exists" };
            }
            if (await userManager.FindByNameAsync(model.UserName) != null)
            {
                return new AuthModel { Message = "That UserName is exists" };
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName
            };
            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += result.Errors;
                }
                return new AuthModel { Message = errors };
            }
            await userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiredOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Role = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName
            };
        }

        public async Task<AuthModel> GetToken(TokenRequestModel model)
        {
            var authModel = new AuthModel();
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or password is not valid";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var Userroles = await userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.ExpiredOn = jwtSecurityToken.ValidTo;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;

            
            authModel.Role = Userroles.ToList();

            return authModel;
        }

        public async Task<string> AddROle(AddRoleModel model)
        {
            var user = await userManager.FindByIdAsync(model.userId);
            if (user is null || !await roleManager.RoleExistsAsync(model.Role))
                return "Invalid User id or Role";
            if (await userManager.IsInRoleAsync(user, model.Role))
                return "This User already exists";

            var result = await userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "something went wrong";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }.Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials
                
                );
            return jwtSecurityToken;

        }
    }
}

