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
    public class ScriptModel : abstractEF
    {
        /// <summary>
        /// Порт-тригер на который должен срабатывть скрипт в автоматическом режиме
        /// </summary>
        public PortModel TriggerPort { get; set; }
        public int? TriggerPortId { get; set; }
        /// <summary>
        /// Состояние порта, которое следует считать тригером к выполнению скрипта
        /// </summary>
        public bool? TriggerPortState { get; set; }

        /// <summary>
        /// Команды скрипта
        /// </summary>
        public List<CommandModel> Commands { get; set; }

        public override string ToString()
        {
            string script_name = Name;
            if (TriggerPortId > 0 && TriggerPort == null)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        TriggerPort = db.Ports.FirstOrDefault(x => x.Id == TriggerPortId);
                    }
                }
            }
            if (TriggerPort != null)
            {
                script_name += $" [tg:{TriggerPort}]";
            }
            return script_name;
        }
    }
}