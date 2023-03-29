using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WonderlandChip.Database.CustomExceptions;
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

        public async Task<int?> DeleteAccount(int? accountId)
        {
            Account? accountToDelete = await _dbContext.Accounts.FindAsync(accountId);
            if (accountToDelete is null) return null;
            await _dbContext
                .Entry(accountToDelete)
                .Collection(a => a.Animals)
                .LoadAsync();
            if (accountToDelete.Animals?.Count > 0) throw new AccountHasAnimalsException();
            _dbContext.Accounts.Remove(accountToDelete);
            await _dbContext.SaveChangesAsync();
            return accountToDelete?.Id;
        }

        public async Task<bool> DoesEmailExist(string email)
        {
            return await _dbContext.Accounts.AnyAsync(a => a.Email == email);
        }

        public async Task<AccountGetDTO> GetAccountById(int? id)
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

        public async Task<RegisterGetDTO> RegisterAccount(RegisterAccountPostDTO credentials)
        {
            Account user = new Account()
            {
                Email = credentials.Email,
                FirstName = credentials.FirstName,
                LastName = credentials.LastName,
                Password = credentials.Password
            };
            await _dbContext.Accounts.AddAsync(user);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
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

        public async Task<int?> TryAuthenticate(AuthorizeDTO credentials)
        {
            Account? user = await _dbContext.Accounts
                .Where(u => u.Email == credentials.Email && u.Password == credentials.Password)
                .SingleOrDefaultAsync();
            return user?.Id;
        }

        public async Task<AccountGetDTO> UpdateAccount(AccountUpdateDTO user)
        {
            bool doesEmailExist = await _dbContext.Accounts.AnyAsync(a => a.Email == user.Email && a.Id != user.Id);
            if (doesEmailExist) throw new EmailAlreadyExistsException();
            Account? accountToUpdate = await _dbContext.Accounts.FindAsync(user.Id);
            if (accountToUpdate == null) return null;
            accountToUpdate!.Email = user.Email;
            accountToUpdate!.Password = user.Password;
            accountToUpdate!.FirstName = user.FirstName;
            accountToUpdate!.LastName = user.LastName;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            return new AccountGetDTO
            {
                Id = accountToUpdate.Id,
                Email = user.Email,
                FirstName = accountToUpdate.FirstName,
                LastName = accountToUpdate.LastName
            };
        }
    }
}
