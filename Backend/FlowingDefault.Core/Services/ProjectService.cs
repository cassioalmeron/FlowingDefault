using FlowingDefault.Core.Dtos;
using FlowingDefault.Core.Extensions;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core.Services
{
    public class ProjectService
    {
        public ProjectService(FlowingDefaultDbContext dbContext) =>
            _dbContext = dbContext;

        private readonly FlowingDefaultDbContext _dbContext;

        public async Task<IEnumerable<Project>> GetAll(int userId)
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Project?> GetById(int id, int userId)
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        }

        public async Task<IEnumerable<Project>> GetByUserId(int userId)
        {
            return await _dbContext.Projects
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task Save(ProjectDto projectDto, int userId)
        {
            // Verify that the user exists
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                throw new FlowingDefaultException($"User with ID {userId} not found.");

            if (projectDto.Id == 0)
            {
                // Creating new project
                // Check if project name already exists for the same user
                var existingProject = await _dbContext.Projects
                    .FirstOrDefaultAsync(x => x.Name == projectDto.Name && x.UserId == userId);
                
                if (existingProject != null)
                    throw new FlowingDefaultException($"Project '{projectDto.Name}' already exists for this user.");

                var project = projectDto.CopyTo<Project>();
                project.UserId = userId;
                _dbContext.Projects.Add(project);
                
                await _dbContext.SaveChangesAsync();
                projectDto.Id = project.Id;
            }
            else
            {
                // Updating existing project
                var project = await _dbContext.Projects.FindAsync(projectDto.Id);
                if (project == null)
                    throw new FlowingDefaultException($"Project with ID {projectDto.Id} not found.");

                // Check if project name already exists for the same user (excluding current project)
                var duplicateNameProject = await _dbContext.Projects
                    .FirstOrDefaultAsync(x => x.Name == projectDto.Name && 
                                             x.UserId == userId && 
                                             x.Id != projectDto.Id);
                
                if (duplicateNameProject != null)
                    throw new FlowingDefaultException($"Project '{projectDto.Name}' already exists for this user.");

                projectDto.CopyTo(project);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var project = await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

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

        public async Task<bool> ProjectExists(int id, int userId)
        {
            var result = await _dbContext.Projects.AnyAsync(x => x.Id == id && x.UserId == userId);
            return result;
        }
    }
} 