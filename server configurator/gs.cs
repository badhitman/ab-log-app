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
                string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilenameBase);
            }
        }

        public static int SelectedListPosition { get; internal set; }
    }
}