////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
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

        /// <summary>
        /// Преобразовать размер файла в человекочитаемы вид
        /// </summary>
        public static string SizeDataAsString(long SizeFile)
        {
            if (SizeFile < 1024)
                return SizeFile.ToString() + " bytes";
            else if (SizeFile < 1024 * 1024)
                return Math.Round((double)SizeFile / 1024, 2).ToString() + " KB";
            else
                return Math.Round((double)SizeFile / 1024 / 1024, 2).ToString() + " MB";
        }
    }
}