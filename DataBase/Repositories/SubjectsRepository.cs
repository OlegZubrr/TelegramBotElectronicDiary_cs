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

        SubjectsRepository(ApplicationDbContext dbContext)
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
        public async Task Add(GroupEntity group,Guid id,string name) 
        {
            var subjectEntity = new SubjectEntity() 
            {
                Group = group,
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
