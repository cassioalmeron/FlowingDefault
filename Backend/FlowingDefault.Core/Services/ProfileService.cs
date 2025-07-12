using FlowingDefault.Core.Dtos;
using FlowingDefault.Core.Extensions;
using FlowingDefault.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core.Services
{
    public class ProfileService
    {
        public ProfileService(FlowingDefaultDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingDefaultDbContext _dbContext;

        public async Task Save(UserDto userDto)
        {
            var user = await _dbContext.Users.FindAsync(userDto.Id);
            if (user == null)
                throw new FlowingDefaultException($"User with ID {userDto.Id} not found.");

            // Check if username already exists (excluding current user)
            var duplicateUsernameUser = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == userDto.Username && x.Id != userDto.Id);
            
            if (duplicateUsernameUser != null)
                throw new FlowingDefaultException($"Username '{userDto.Username}' already exists.");

            userDto.CopyTo(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ChangePassword(int id, string password)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                throw new FlowingDefaultException($"User with ID {id} not found.");

            user.Password = HashUtils.GenerateMd5Hash(password);

            await _dbContext.SaveChangesAsync();
        }
    }
}