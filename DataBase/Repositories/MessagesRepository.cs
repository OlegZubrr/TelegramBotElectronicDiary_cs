using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class MessagesRepository
    {
        private ApplicationDbContext _dbContext;

        public MessagesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MessageEntity>> Get()
        {
            return await _dbContext.MessageEntities
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<MessageEntity?> GetById(Guid id) 
        {
            return await _dbContext.MessageEntities
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<List<MessageEntity>> GetByUserId(Guid userId) 
        {
            return await _dbContext.MessageEntities
                .AsNoTracking()
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }
        public async Task Add(Guid userId,Guid id,long chatId,long messageId)  
        {
            var messageEntity = new MessageEntity() 
            {
                UserId = userId,
                Id = id,
                ChatId = chatId,
                MessageId = messageId
            };
            await _dbContext.MessageEntities.AddAsync(messageEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task Delete(Guid id) 
        {
            await _dbContext.MessageEntities
                .Where(m => m.Id == id)
                .ExecuteDeleteAsync();
                
        }
    }
}
