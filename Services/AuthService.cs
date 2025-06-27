using Microsoft.EntityFrameworkCore;
using net_chat.Enum;
using net_chat.Model;
using net_chat.Model.Request;

namespace net_chat.Services
{
    public interface IAuthService
    {
        Task<(Account account, string token)> RegisterAsync(RegisterRequest model);
        Task<(Account account, string token)> LoginAsync(LoginRequest model);
    }

    public class AuthService(AppDbContext context, IConfiguration config) : IAuthService
    {
        private readonly JwtService _jwtService = new(config);

        public async Task<(Account account, string token)> RegisterAsync(RegisterRequest model)
        {

            // Check if user exists
            var exists = await context.Accounts
                .AnyAsync(a =>a.Email == model.Email);

            if (exists)
            {
                throw new InvalidOperationException("Username or email already exists.");
            }

            var dateTimeNow = DateTime.UtcNow.ToString("u");

            // Create new account
            var account = new Account
            {
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                CreatedAt = dateTimeNow, // Ensure your database schema includes a 'CreatedAt' column in the 'Accounts' table
                
            };
            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                UserName = model.Username,
                AccountId = account.Id,
                AvatarUrl = null,
                Status = 1,
                DateOfBirth = model.BirthDate,
                Gender = (int)GenderType.Other,
            };
            context.UserProfiles.Add(userProfile);
            await context.SaveChangesAsync();
            // Generate token
            var token = _jwtService.GenerateToken(account);

            var newAccount = new Account
            {
                Email = account.Email,
                PasswordHash = account.PasswordHash,
                CreatedAt = account.CreatedAt,
                UserProfile = userProfile
            };

            return (newAccount, token);
        }

        public async Task<(Account account, string token)> LoginAsync(LoginRequest model)
        {
            var account = await context.Accounts
                .Include(a => a.UserProfile)
                .FirstOrDefaultAsync(a => a.Email == model.Email);

            if (account == null || !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
            {
                throw new InvalidOperationException("Invalid username or password.");
            }

            var token = _jwtService.GenerateToken(account);
            return (account, token);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }
    }
} 