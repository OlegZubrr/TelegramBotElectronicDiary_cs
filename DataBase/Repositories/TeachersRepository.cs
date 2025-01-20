using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class TeachersRepository
    {
        private ApplicationDbContext _dbContext;
        

        public TeachersRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<TeacherEntity>> Get()
        {
            return await _dbContext.Teachers
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<TeacherEntity?> GetById(Guid id)
        {
            return await _dbContext.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<TeacherEntity?> GetByUserId(Guid id)
        {
            return await _dbContext.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == id);
        }
        public async Task Add(Guid userId,Guid id,string name) 
        {
            var teacherEntity = new TeacherEntity() 
            {
                UserId = userId,
                Id = id,
                Name = name
            };
            await _dbContext.AddAsync(teacherEntity);
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
