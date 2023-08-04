using SecuredApiTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecuredApiTest.Services
{
    public interface IAuthService
    {
        Task<AuthModel> Register(RegisterModel model);

        Task<AuthModel> GetToken(TokenRequestModel model);

        Task<string> AddROle(AddRoleModel model);
    }
}
