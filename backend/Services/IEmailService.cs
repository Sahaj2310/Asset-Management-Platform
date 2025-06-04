using System.Threading.Tasks;

namespace AssetWeb.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailConfirmationAsync(string email, Guid userId, string token);
        Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
    }
} 