////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Services;
using System.Collections.Generic;
using System.Linq;

namespace ab.Model
{
    /// <summary>
    /// Скрипты (пакеты заданий)
    /// </summary>
    public class ScriptHardwareModel : abstractEF
    {
        /// <summary>
        /// Порт-тригер на который должен срабатывть скрипт в автоматическом режиме
        /// </summary>
        public PortHardwareModel TriggerPort { get; set; }
        public int? TriggerPortId { get; set; }
        /// <summary>
        /// Состояние порта, которое следует считать тригером к выполнению скрипта
        /// </summary>
        public bool? TriggerPortState { get; set; }

        /// <summary>
        /// Команды скрипта
        /// </summary>
        public List<CommandScriptModel> CommandsScripts { get; set; }

        public override string ToString()
        {
            string script_name = Name;
            if (TriggerPortId > 0 && TriggerPort == null)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        TriggerPort = db.PortsHardwares.FirstOrDefault(x => x.Id == TriggerPortId);
                    }
                }
            }
            if (TriggerPort != null)
            {
                script_name += $" [tg:{TriggerPort}]";
            }

#if DEBUG
            script_name = $"#{Id}; {script_name}";
#endif

            return script_name;
        }
    }
}