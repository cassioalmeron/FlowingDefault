using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core.Services
{
    public class ProjectService
    {
        public ProjectService(FlowingDefaultDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingDefaultDbContext _dbContext;

        public async Task<IEnumerable<Project>> GetAll()
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<Project?> GetById(int id)
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetByUserId(int userId)
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task Save(Project project)
        {
            // Check if project name already exists for the same user (excluding current project if updating)
            var existingProject = await _dbContext.Projects
                .FirstOrDefaultAsync(x => x.Name == project.Name && 
                                         x.UserId == project.UserId && 
                                         x.Id != project.Id);
            
            if (existingProject != null)
                throw new FlowingDefaultException($"Project '{project.Name}' already exists for this user.");

            // Verify that the user exists
            var user = await _dbContext.Users.FindAsync(project.UserId);
            if (user == null)
                throw new FlowingDefaultException($"User with ID {project.UserId} not found.");

            if (project.Id == 0)
            {
                // New project
                _dbContext.Projects.Add(project);
            }
            else
            {
                // Update existing project - handle detached entities
                var existingProjectToUpdate = await _dbContext.Projects.FindAsync(project.Id);
                if (existingProjectToUpdate == null)
                    throw new FlowingDefaultException($"Project with ID {project.Id} not found.");

                // Update properties of the tracked entity
                existingProjectToUpdate.Name = project.Name;
                existingProjectToUpdate.UserId = project.UserId;
            }
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var project = await _dbContext.Projects.FindAsync(id);
            if (project == null)
                return false;

            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProjectNameExistsForUser(string projectName, int userId)
        {
            var result = await _dbContext.Projects
                .AnyAsync(x => x.Name == projectName && x.UserId == userId);
            return result;
        }

        public async Task<bool> ProjectExists(int id)
        {
            var result = await _dbContext.Projects.AnyAsync(x => x.Id == id);
            return result;
        }
    }
} 