////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace ab.Model
{
    /// <summary>
    /// Скрипты (пакеты заданий)
    /// </summary>
    public class ScriptHardwareModel : abstractEF
    {
        /// <summary>
        /// Необходимость уведомления пользователей о начале/окончании выполнения скрипта
        /// </summary>
        public bool Notifications { get; set; }

        /// <summary>
        /// Порт-тригер на который должен срабатывть скрипт в автоматическом режиме
        /// </summary>
        public PortHardwareModel TriggerPort { get; set; }
        public int? TriggerPortId { get; set; }
        /// <summary>
        /// Состояние порта, которое следует считать тригером к выполнению скрипта
        /// </summary>
        public bool TriggerPortState { get; set; }

        /// <summary>
        /// Команды скрипта
        /// </summary>
        public List<ComandScriptModel> ComandsScripts { get; set; }
    }
}