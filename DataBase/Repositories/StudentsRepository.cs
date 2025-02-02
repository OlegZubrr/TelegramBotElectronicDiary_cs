using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class StudentsRepository
    {
        ApplicationDbContext _dbContext;

        public StudentsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<StudentEntity>> Get()
        {
            return await _dbContext.Students
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<StudentEntity?> GetById(Guid id)
        {
            return await _dbContext.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task <StudentEntity?> GetByUserId(Guid userId) 
        {
            
            return await _dbContext.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
        public async Task<List<StudentEntity>> GetByIds(List<Guid> groupIds)
        {
            if (groupIds == null || groupIds.Count == 0)
                return new List<StudentEntity>();

            return await _dbContext.Students
                .AsNoTracking()
                .Where(s => groupIds.Contains(s.Id))
                .ToListAsync();
        }
        public async Task Update(Guid id, Guid userId,Guid? groupId,string name)
        {
            await _dbContext.Students
                .Where(s=> s.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(s => s.Id, id)
                    .SetProperty(s => s.UserId, userId)
                    .SetProperty(s => s.GroupId, groupId)
                    .SetProperty(s => s.Name, name));
        }

        public async Task Add(Guid userId,Guid id,string name) 
        {
            var studentEntity = new StudentEntity() 
            {
                UserId = userId,
                Id = id, 
                Name = name,
            };
            await _dbContext.AddAsync(studentEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            await _dbContext.Students
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

    }
}
