using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class GroupsRepository
    {
        private ApplicationDbContext _dbContext;

        public GroupsRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<GroupEntity>> Get()
        {
            return await _dbContext.Groups
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<GroupEntity?> GetById(Guid id)
        {
            return await _dbContext.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        public async Task<List<GroupEntity>> GetByIds(List<Guid> groupIds)
        {
            if (groupIds == null || groupIds.Count == 0)
                return new List<GroupEntity>(); 

            return await _dbContext.Groups
                .AsNoTracking()
                .Where(g => groupIds.Contains(g.Id)) 
                .ToListAsync();
        }
        public async Task Add(Guid teacherId,Guid id,string name) 
        {
            var groupEntity = new GroupEntity()
            {
               Id = id,
               //TeachersIds = new List<Guid>{ teacherId},
               Name = name,

            };
            await _dbContext.AddAsync(groupEntity);
            await _dbContext.SaveChangesAsync();    
        }
        public async Task AddStudent(GroupEntity group, Guid studentId)
        {
            group.StudentIds.Add(studentId);
            await _dbContext.Groups
                .Where(g => g.Id == group.Id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(g => g.StudentIds,group.StudentIds)
                );
        }
        public async Task Delete(Guid id) 
        {
            await _dbContext.Groups
                .Where(g => g.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
