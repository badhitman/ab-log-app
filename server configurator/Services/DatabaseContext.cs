////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Microsoft.EntityFrameworkCore;

namespace ab.Services
{
    public class DatabaseContext : DbContext
    {
        private string _databasePath;
        public static object DbLocker = new object();

        public DbSet<UserModel> Users { get; set; }
        public DbSet<HardwareModel> Hardwares { get; set; }
        public DbSet<CloudEmailMessageModel> CloudMessages { get; set; }
        public DbSet<TelegramMessageModel> TelegramMessages { get; set; }
        public DbSet<TelegramUserModel> TelegramUsers { get; set; }
        public DbSet<PortsHardwaresModel> PortsHardwares { get; set; }

        public DatabaseContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
        }
    }
}