////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using MQTTnet.Server;
using System;

namespace ab.Services
{
    public interface IForegroundService
    {
        /// <summary>
        /// MQTT брокер запущен?
        /// </summary>
        /// <returns></returns>
        public bool isStartedMqtt { get; }

        /// <summary>
        /// Запустить MQTT брокер
        /// </summary>
        /// <param name="mqtt_broker_port">Порт работы брокера</param>
        /// <param name="mqtt_broker_topic">Топик работы брокера для обслуживания MegaD</param>
        /// <param name="mqtt_broker_user">имя ползователя для авторизации MegaD в брокере</param>
        /// <param name="mqtt_broker_passwd">пароль ползователя для авторизации MegaD в брокере</param>
        public void StartMqttBroker(int mqtt_broker_port, string mqtt_broker_topic, string mqtt_broker_user = null, string mqtt_broker_passwd = null);

        /// <summary>
        /// Остановить работу MQTT брокера
        /// </summary>
        public void StopMqttBroker();
    }
}