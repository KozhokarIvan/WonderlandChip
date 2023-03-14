using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.DbContexts;
using WonderlandChip.Database.DTO.Account;
using WonderlandChip.Database.DTO.Authentication;
using WonderlandChip.Database.Models;
using WonderlandChip.Database.Repositories.Interfaces;

namespace WonderlandChip.Database.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ChipizationDbContext _dbContext;
        public AccountRepository(ChipizationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DoesEmailExist(string email)
        {
            return await _dbContext.Accounts.AnyAsync(a => a.Email == email);
        }

        public async Task<AccountGetDTO> GetAccountById(int id)
        {
            Account foundUser = await _dbContext.Accounts.FindAsync(id);
            if (foundUser == null) return null;
            AccountGetDTO returnAccount = new AccountGetDTO()
            {
                Id = foundUser.Id,
                Email = foundUser.Email,
                FirstName = foundUser.FirstName,
                LastName = foundUser.LastName
            };
            return returnAccount;
        }

        public async Task<RegisterGetDTO> RegisterAccount(RegisterPostDTO credentials)
        {
            Account user = new Account()
            {
                Email = credentials.Email,
                FirstName = credentials.FirstName,
                LastName = credentials.LastName,
                Password = credentials.Password
            };
            await _dbContext.Accounts.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            RegisterGetDTO returnRegister = new RegisterGetDTO()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return returnRegister;
        }

        public async Task<List<AccountGetDTO>> SearchAccounts(AccountSearchDTO user)
        {
            List<Account> foundUsers = await _dbContext.Accounts
                .Where(u =>
                (string.IsNullOrWhiteSpace(user.FirstName) || u.FirstName.Contains(user.FirstName)) &&
                (string.IsNullOrWhiteSpace(user.LastName) || u.LastName.Contains(user.LastName)) &&
                (string.IsNullOrWhiteSpace(user.Email) || u.Email.Contains(user.Email)))
                .OrderBy(u => u.Id)
                .Skip(user.From)
                .Take(user.Size)
                .ToListAsync();
            List<AccountGetDTO> returnUsers = foundUsers.Select(u => new AccountGetDTO()
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            }).ToList();
            return returnUsers;
        }

        public async Task<bool> TryAuthenticate(AuthorizeDTO credentials)
        {
            Account user = await _dbContext.Accounts
                .Where(u => u.Email == credentials.Email && u.Password == credentials.Password)
                .SingleOrDefaultAsync();
            if (user is null)
                return false;
            return true;
        }
    }
}
