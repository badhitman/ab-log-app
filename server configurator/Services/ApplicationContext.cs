﻿////////////////////////////////////////////////
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

        public static HardwareModel[] HardwaresCached { get; set; }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<HardwareModel> Hardwares { get; set; }

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