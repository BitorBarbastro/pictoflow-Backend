using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using pictoflow_Backend.Models;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace pictoflow_Backend.Services
{
    public class UserManager
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserManager(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public void SetUserPassword(User user, string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            user.PasswordSalt = salt;
            user.PasswordHash = HashPassword(password, salt);
        }

        public bool CheckPassword(User user, string password)
        {
            string hashedPassword = HashPassword(password, user.PasswordSalt);
            return hashedPassword == user.PasswordHash;
        }


        public void CreateUser(User user, string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            user.PasswordSalt = salt;
            user.PasswordHash = HashPassword(password, salt);

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        private string HashPassword(string password, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("tu_clave_secreta_aqui"); // Asegúrate de usar una clave secreta segura
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Email)
                    // Puedes agregar más claims aquí si es necesario
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
