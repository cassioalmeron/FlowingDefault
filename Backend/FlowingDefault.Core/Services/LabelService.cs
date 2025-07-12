using FlowingDefault.Core.Dtos;
using FlowingDefault.Core.Extensions;
using FlowingDefault.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingDefault.Core.Services
{
    public class LabelService
    {
        private readonly FlowingDefaultDbContext _dbContext;
        public LabelService(FlowingDefaultDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Label>> GetAll()
        {
            return await _dbContext.Set<Label>().ToListAsync();
        }

        public async Task<LabelDto> GetById(int id)
        {
            var label = await _dbContext.Set<Label>().FindAsync(id);
            return label.CopyTo<LabelDto>();
        }

        public async Task Save(LabelDto labelDto)
        {
            if (labelDto.Id == 0)
            {
                // Create
                var label = new Label { Name = labelDto.Name };
                _dbContext.Set<Label>().Add(label);
                await _dbContext.SaveChangesAsync();
                labelDto.Id = label.Id;
            }
            else
            {
                // Update
                var label = await _dbContext.Set<Label>().FindAsync(labelDto.Id);
                if (label == null)
                    throw new FlowingDefaultException($"Label with ID {labelDto.Id} not found.");
                label.Name = labelDto.Name;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Delete(int id)
        {
            var label = await _dbContext.Set<Label>().FindAsync(id);
            if (label == null)
                return false;
            _dbContext.Set<Label>().Remove(label);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
} 