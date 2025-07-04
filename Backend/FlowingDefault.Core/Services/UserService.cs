using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core.Services
{
    public class UserService
    {
        public UserService(FlowingDefaultDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingDefaultDbContext _dbContext;

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User?> GetById(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task Save(User user)
        {
            // Check if username already exists (excluding current user if updating)
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Username == user.Username && x.Id != user.Id);
            
            if (existingUser != null)
                throw new FlowingDefaultException($"Username '{user.Username}' already exists.");

            if (user.Id == 0)
            {
                // New user
                _dbContext.Users.Add(user);
            }
            else
            {
                // Update existing user - handle detached entities
                var existingUserToUpdate = await _dbContext.Users.FindAsync(user.Id);
                if (existingUserToUpdate == null)
                    throw new FlowingDefaultException($"User with ID {user.Id} not found.");

                // Update properties of the tracked entity
                existingUserToUpdate.Name = user.Name;
                existingUserToUpdate.Username = user.Username;
                existingUserToUpdate.Password = user.Password;
            }
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UsernameExists(string username)
        {
            var result = await _dbContext.Users.AnyAsync(x => x.Username == username);
            return result;
        }
    }
}