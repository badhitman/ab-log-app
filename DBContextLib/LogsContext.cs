////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

namespace ab.Services
{
    public class LogsContext : DbContext
    {
        public static object DbLocker = new object();

        /// <summary>
        /// имя файла базы данны логов
        /// </summary>
        public const string DatabaseFilenameLogs = "ab-log-server-logs.db";
        /// <summary>
        /// путь к файлу базы данных логов
        /// </summary>
        public static string DatabasePathLogs
        {
            get
            {
                string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilenameLogs);
            }
        }

        public DbSet<LogRowModel> Logs { get; set; }

        public async Task<LogRowModel> AddLogRowAsync(LogStatusesEnum logStatus, string Message, string tag)
        {
            LogRowModel logRow = new LogRowModel() { Status = logStatus, Name = Message, TAG = tag };
            await Logs.AddAsync(logRow);
            await SaveChangesAsync();
            return logRow;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={DatabasePathLogs}");
        }
    }
}