using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class MarksRepository
    {
        private ApplicationDbContext _dbContext;

        public MarksRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }   

        public async Task<List<MarkEntity>> Get() 
        {
            return await _dbContext.Marks
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<MarkEntity?> GetById(Guid id) 
        {
            return await _dbContext.Marks
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<List<MarkEntity?>> GetByStudentAndSubjectId(Guid studentId, Guid subjectId)
        {
            return await _dbContext.Marks
                .AsNoTracking()
                .Where(mark => mark.StudentId == studentId && mark.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task Add(Guid studentId,Guid subjectId,Guid id,int value) 
        {
            MarkEntity  markEntity = new MarkEntity() 
            {
                StudentId = studentId,
                SubjectId = subjectId,
                Id = id,
                Value = value
            };
            await _dbContext.AddAsync(markEntity);
            await _dbContext.SaveChangesAsync();

        }
        public async Task Delete(Guid id) 
        {
            await _dbContext.Marks
                .Where(m => m.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
