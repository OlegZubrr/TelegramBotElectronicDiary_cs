using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class UsersRepository
    {
        private ApplicationDbContext _dbContext;

        public UsersRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<List<UserEntity>> Get() 
        {
            return await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<UserEntity?> GetById(Guid id) 
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<UserEntity?> GetByTelegramId(long telegramId)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }
        public async Task Add(Guid id , long telgramId , string? userName) 
        {
            var userEntity =  new UserEntity() 
            {
                Id = id,
                TelegramId = telgramId,
                UserName = userName
            };
            await _dbContext.AddAsync(userEntity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<UserEntity> GetOrAddUserAsync(long telegramId, string? userName)
        {
            var user = await GetByTelegramId(telegramId);
            if (user != null)
            {
                return user;
            }

            var newUserId = Guid.NewGuid();
            await Add(newUserId, telegramId, userName);

            return new UserEntity
            {
                Id = newUserId,
                TelegramId = telegramId,
                UserName = userName
            };
        }
        public async Task Update(Guid id,long telegramId, string? userName,List<Guid> messageIds) 
        {
            await _dbContext.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.TelegramId, telegramId)
                    .SetProperty(u => u.UserName, userName)
                    .SetProperty(u => u.MesssageIds, messageIds));
        }
        public async Task Delete(Guid id) 
        {
            await _dbContext.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
