////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.IO;

namespace ab
{
    /// <summary>
    /// global static
    /// </summary>
    public static class gs
    {
        /// <summary>
        /// имя файла основной базы данны
        /// </summary>
        public const string DatabaseFilenameBase = "ab-log-server-base.db";
        /// <summary>
        /// путь к файлу основной базы данных
        /// </summary>
        public static string DatabasePathBase
        {
            get
            {
                var basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilenameBase);
            }
        }

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
                var basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilenameLogs);
            }
        }

        public static int SelectedListPosition { get; internal set; }
    }
}