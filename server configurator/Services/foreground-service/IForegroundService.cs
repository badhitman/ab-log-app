////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

namespace ab.Services
{
    public interface IForegroundService
    {
        /// <summary>
        /// Служба запущена?
        /// </summary>
        /// <returns></returns>
        public bool isStartedForegroundService { get; }

        /// <summary>
        /// Запустить службу
        /// </summary>
        /// <param name="foreground_service_port">Порт работы сервиса</param>
        public void StartForegroundService(int foreground_service_port);

        /// <summary>
        /// Остановить работу службы
        /// </summary>
        public void StopForegroundService();
    }
}