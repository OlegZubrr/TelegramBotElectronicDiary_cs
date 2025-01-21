using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotEFCore.DataBase.Models;

namespace TelegramBotEFCore.DataBase.Repositories
{
    public class UserRoleVerificationRepository
    {
        private ApplicationDbContext _dbContext;
        public UserRoleVerificationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<UserRoleVerificationEntity>> Get()
        {
            return await _dbContext.UserRoleVerification
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<UserRoleVerificationEntity?>GetByUserId(Guid userId) 
        {
            return await _dbContext.UserRoleVerification
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task Add(Guid userId, Guid id,Guid registerCode)
        {
           UserRoleVerificationEntity userRoleVerificationEntity = new UserRoleVerificationEntity() 
           {
               Id = id,
               UserId = userId,
               RegisterCode = registerCode
           };
           await _dbContext.UserRoleVerification.AddAsync(userRoleVerificationEntity);
           await _dbContext.SaveChangesAsync();
        }
        public async Task Delete(Guid id) 
        {
            await _dbContext.UserRoleVerification
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
