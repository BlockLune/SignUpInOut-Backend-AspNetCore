using Microsoft.EntityFrameworkCore;
using SignUpInOut_Backend_AspNetCore.Models;

namespace SignUpInOut_Backend_AspNetCore.Services
{
    public class UserService
    {
        private readonly SignupinoutDbContext _dbContext;
        public UserService(SignupinoutDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User?> SignUpAsync(string email, string password)
        {
            if (await UserExistsAsync(email))
            {
                return null;
            }
            var passwordToStore = BCrypt.Net.BCrypt.HashPassword(password, 10);
            await _dbContext.Users.AddAsync(new User {
                Email = email,
                Password = passwordToStore
            });
            await _dbContext.SaveChangesAsync();
            return await GetUserAsync(email);
        }
        public async Task<User?> SignInAsync(string email, string password)
        {
            var user = await GetUserAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            return user;
        }
        public async Task<User?> UpdatePasswordAsync(int id, string password)
        {
            var user = await GetUserAsync(id);
            if (user == null)
            {
                return null;
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(password, 10);
            await _dbContext.SaveChangesAsync();
            return user;
        }
        public async Task<User?> GetUserAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }
        public async Task<User?> GetUserAsync(string email)
        {
            return await _dbContext.Users.FindAsync(email);
        }
        public async Task<bool> UserExistsAsync(int id)
        {
            return await _dbContext.Users.AnyAsync(e => e.Id == id);
        }
        public async Task<bool> UserExistsAsync(string email)
        {
           return await _dbContext.Users.AnyAsync(e => e.Email == email);
        }
        public static UserDTO UserToDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Email = user.Email
            };
        }
    }
}
