////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;

namespace ab.Model
{
    /// <summary>
    /// Типы инициаторов выполнения сценриев
    /// </summary>
    public enum TaskInitiatorsTypes
    {
        /// <summary>
        /// Ручной запуск из приложения
        /// </summary>
        Manual,
        /// <summary>
        /// Автозапуск по событию/триггеру
        /// </summary>
        Trigger,
        /// <summary>
        /// Запуск из TelegramBot
        /// </summary>
        Telegram
    }

    public class TaskScriptModel : abstractEF
    {
        public TaskInitiatorsTypes TaskInitiatorType { get; set; }

        public long TaskInitiatorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime FinishedAt { get; set; } = DateTime.MinValue;

        public ScriptHardwareModel ScriptHardware { get; set; }

        public int ScriptHardwareId { get; set; }
    }
}