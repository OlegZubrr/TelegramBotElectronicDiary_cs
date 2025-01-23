using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class SubjectsRepository
    {
        ApplicationDbContext _dbContext;

        public SubjectsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SubjectEntity>> Get()
        {
            return await _dbContext.Subjects
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<SubjectEntity?> GetById(Guid id)
        {
            return await _dbContext.Subjects
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<List<SubjectEntity>> GetByIds(List<Guid> groupIds)
        {
            if (groupIds == null || groupIds.Count == 0)
                return new List<SubjectEntity>();

            return await _dbContext.Subjects
                .AsNoTracking()
                .Where(g => groupIds.Contains(g.Id))
                .ToListAsync();
        }
        public async Task Add(Guid groupId,GroupEntity group,Guid id,string name) 
        {
            group.SubjectIds.Add(id);
            await _dbContext.Groups
              .Where(g => g.Id == group.Id)
              .ExecuteUpdateAsync(s => s
              .SetProperty(g => g.SubjectIds, group.SubjectIds)
              );
            var subjectEntity = new SubjectEntity() 
            {
                GroupId = groupId,
                Id = id,
                Name = name
            };
            await _dbContext.AddAsync(subjectEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            await _dbContext.Teachers
                .Where(t => t.Id == id)
                .ExecuteDeleteAsync();
        }


    }
}
