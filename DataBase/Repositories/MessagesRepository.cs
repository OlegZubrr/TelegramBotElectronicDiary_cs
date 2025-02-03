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
        public async Task<List<MessageEntity>> GetByIds(List<Guid> ids) 
        {
            if (ids == null || ids.Count == 0)
                return new List<MessageEntity>();

            return await _dbContext.MessageEntities
              .AsNoTracking()
              .Where(m => ids.Contains(m.Id))
              .ToListAsync();

        }
        public async Task Add(Guid userId,Guid id,long chatId,long messageId,UserEntity user)  
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

            user.MesssageIds.Add(id);

            await _dbContext.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.MesssageIds, user.MesssageIds));

        }
        public async Task Update(Guid userId, Guid id, long chatId, long messageId) 
        {
            await _dbContext.MessageEntities
                .Where(m => m.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.UserId, userId)
                    .SetProperty(m => m.ChatId, chatId)
                    .SetProperty(m => m.MessageId, messageId));
        }
        public async Task Delete(Guid id) 
        {
            await _dbContext.MessageEntities
                .Where(m => m.Id == id)
                .ExecuteDeleteAsync();
                
        }
        public async Task AddOrUpdate(Guid userId, Guid id, long chatId, long messageId,UserEntity user) 
        {
            var messageEntity = await GetById(id);
            if (messageEntity == null)
            {
                await Add(userId, id, chatId, messageId,user);
            }
            else 
            {
               await Update(userId, id, chatId, messageId);
            }
        }
    }
}
