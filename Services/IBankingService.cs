using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IBankingService
    {
        Task<bool> ProcessPayment(int userId, decimal amount);
    }
}
