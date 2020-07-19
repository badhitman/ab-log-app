////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ab.Services
{
    public class DatabaseContext : DbContext
    {
        private string _databasePath;
        public static object DbLocker = new object();

        public DbSet<UserModel> Users { get; set; }
        public DbSet<HardwareModel> Hardwares { get; set; }
        public DbSet<CloudEmailMessageModel> Messages { get; set; }
        //public DbSet<CloudEmailMessage> TelegramMessages { get; set; }

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