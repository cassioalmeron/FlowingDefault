using FlowingDefault.Core.Models;
using FlowingDefault.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core.Services
{
    public class LoginService
    {
        public LoginService(FlowingDefaultDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingDefaultDbContext _dbContext;

        public async Task<User> Execute(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new FlowingDefaultException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(password))
                throw new FlowingDefaultException("Password cannot be empty");

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            var invalidMessage = "Invalid username or password";
            if (user == null)
                throw new FlowingDefaultException(invalidMessage);

            var hashedPassword = HashUtils.GenerateMd5Hash(password);
            if (user.Password != hashedPassword)
                throw new FlowingDefaultException(invalidMessage);

            return user;
        }
    }
}