using Integrations.Model;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(LoginRequest request);
    }
}


