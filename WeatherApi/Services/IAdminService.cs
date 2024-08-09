using WeatherApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeatherApi.Data;
using ToDo.Services;

namespace WeatherApi.Services
{
    public interface IAdminService
    {
        Task<string> RegisterAsync(RegisterModel model);
        Task<UserValidationResult> ValidateUserAsync(string username, string password);
    }

    public enum UserValidationResult
    {
        Valid,
        InvalidUsername,
        InvalidPassword,
        UserNotFound
    }

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        private readonly TokenService _tokenService;

        public AdminService(ApplicationDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<string> RegisterAsync(RegisterModel registerModel)
        {
            // Kullanıcı adı kontrolü
            var existingUser = await _context.Admins
                .FirstOrDefaultAsync(u => u.Username == registerModel.Username);

            if (existingUser != null)
            {
                return "Kullanıcı adı mevcut."; // Kullanıcı adı veya e-posta zaten mevcut
            }

            // Yeni kullanıcı oluştur
            var user = new Admin
            {
                Username = registerModel.Username,
                Password = registerModel.Password // Düz metin şifre saklanıyor
            };

            await _context.Admins.AddAsync(user);
            await _context.SaveChangesAsync();

            return "Kayıt başarılı!";
        }

        public async Task<UserValidationResult> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = await _context.Admins
                                         .SingleOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return UserValidationResult.UserNotFound;
                }

                // Şifreyi hashlemeden doğrula
                if (user.Password != password)
                {
                    return UserValidationResult.InvalidPassword;
                }

                return UserValidationResult.Valid;
            }
            catch (Exception ex)
            {
                // Hata yönetimi yapılabilir
                // Örneğin:
                // _logger.LogError(ex, "Kullanıcı doğrulama sırasında bir hata oluştu.");
                throw;
            }
        }
    }
}
