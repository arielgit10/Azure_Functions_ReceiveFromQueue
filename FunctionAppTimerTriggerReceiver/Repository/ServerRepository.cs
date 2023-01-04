

using FunctionAppTimerTriggerReceiver;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace TimerTriggerReceiver
{
    public class ServerRepository : IServerRepository
    {
        private AppDbContext _context;
        private DbContextOptions<AppDbContext> GetAllOptions()
        {
            DbContextOptionsBuilder<AppDbContext> optionsBuilder =
                            new DbContextOptionsBuilder<AppDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection", EnvironmentVariableTarget.Process);
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
        public async Task PostAccount(Account account)
        {
            using (_context = new AppDbContext(GetAllOptions()))
            {
                try
                {
                    _context.AccountsReceived.Add(account);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

 
    }
}