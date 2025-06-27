using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using net_chat.Model;

namespace net_chat.Services
{
    public class JwtService(IConfiguration config)
    {
        private const string KeyFilePath = "jwtkey.txt";
        public static SymmetricSecurityKey GetSecurityKey()
        {
            string key;

            if (!File.Exists(KeyFilePath))
            {
                // Generate a new key if it doesn't exist
                var bytes = RandomNumberGenerator.GetBytes(32);
                key = Convert.ToBase64String(bytes);
                File.WriteAllText(KeyFilePath, key);
            }
            else
            {
                key = File.ReadAllText(KeyFilePath);
            }

            var keyBytes = Convert.FromBase64String(key);
            return new SymmetricSecurityKey(keyBytes);
        }

        public string GenerateToken(Account account)
        {
            var claims = new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim(ClaimTypes.Email, account.Email),
                };

            var key = GetSecurityKey();
            var reds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(config["Jwt:ExpiresInMinutes"])),
                signingCredentials: reds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
