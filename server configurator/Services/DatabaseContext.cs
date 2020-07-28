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
        public DbSet<PortHardwareModel> PortsHardwares { get; set; }
        public DbSet<ScriptHardwareModel> ScriptsHardware { get; set; }
        public DbSet<CommandScriptModel> CommandsScript { get; set; }

        public DatabaseContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<UserModel>().HasAlternateKey(u => u.Name);
            //modelBuilder.Entity<UserModel>().HasAlternateKey(u => u.Phone);
            //modelBuilder.Entity<UserModel>().HasAlternateKey(u => u.Email);

            modelBuilder.Entity<HardwareModel>().HasAlternateKey(u => u.Name);
            modelBuilder.Entity<HardwareModel>().HasAlternateKey(u => u.Address);

            modelBuilder.Entity<PortHardwareModel>().HasAlternateKey(u => new { u.HardwareId, u.PortNumb });

            //modelBuilder.Entity<ScriptHardwareModel>().HasAlternateKey(u => u.Name);
            //modelBuilder.Entity<ComandScriptModel>().HasAlternateKey(u => u.Name);
        }
    }
}