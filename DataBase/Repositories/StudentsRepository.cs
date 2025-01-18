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
       public async Task Add(UserEntity user,Guid id,GroupEntity group,string name) 
       {
            var studentEntity = new StudentEntity() 
            {
                User = user,
                Id = id, 
                Name = name,
                Group = group, 
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
