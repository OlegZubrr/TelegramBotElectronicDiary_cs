using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;
using TelegramBotEFCore.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class TeacherMessagesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TeacherMessagesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TeacherMessageEntity>> Get()
        {
            return await _dbContext.TeacherMessages
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TeacherMessageEntity?> GetById(Guid id)
        {
            return await _dbContext.TeacherMessages
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<TeacherMessageEntity>> GetByTeacherId(Guid teacherId)
        {
            return await _dbContext.TeacherMessages
                .AsNoTracking()
                .Where(m => m.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task Add(Guid id, Guid teacherId, MessageTypeEnum messageTypeEnum)
        {
            var teacherMessage = new TeacherMessageEntity
            {
                Id = id,
                TeacherId = teacherId,
                MessageType = messageTypeEnum
            };
            await _dbContext.TeacherMessages.AddAsync(teacherMessage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Guid id, Guid teacherId, MessageTypeEnum messageType)
        {
            await _dbContext.TeacherMessages
                .Where(m => m.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.TeacherId, teacherId)
                    .SetProperty(m => m.MessageType, messageType));
        }
        public async Task Delete(Guid id,TeacherEntity teacher) 
        {
            teacher.MessagesIds.Remove(id);
            await _dbContext.Teachers
                .Where(t => t.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => teacher.MessagesIds, teacher.MessagesIds));

            await _dbContext.StudentMessages
                .Where(m => m.Id == id)
                .ExecuteDeleteAsync();
                
        }
    }
}
