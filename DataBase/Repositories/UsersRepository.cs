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
        public async Task Delete(Guid id) 
        {
            await _dbContext.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();
        }

        

    }
}
