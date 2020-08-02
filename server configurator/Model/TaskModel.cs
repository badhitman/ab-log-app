////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;

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

    public class TaskModel : abstractEF
    {
        public TaskInitiatorsTypes TaskInitiatorType { get; set; }

        public long TaskInitiatorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime FinishedAt { get; set; } = DateTime.MinValue;

        public ScriptModel Script { get; set; }
        public int ScriptId { get; set; }

        public List<ReportModel> Reports { get; set; }
    }
}