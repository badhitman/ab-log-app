////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.OS;
using Android.Util;
using MQTTnet.Server;
using System;

namespace ab.Services
{
    public class MqttBrokerServiceBinder : Binder, IForegroundService
    {
        static readonly string TAG = typeof(MqttBrokerServiceBinder).Name;

        public MqttBrokerService Service { get; private set; }

        public bool isStartedMqtt => Service?.isStartedMqtt ?? false;

        public ServicesActivity myActivity { get; set; }

        public MqttBrokerServiceBinder(MqttBrokerService service)
        {
            Log.Debug(TAG, "~ constructor");
            Service = service;
        }

        public void StartMqttBroker(int mqtt_broker_port, string mqtt_broker_topic, string mqtt_broker_user = null, string mqtt_broker_passwd = null)
        {
            Log.Debug(TAG, $"StartMqttBroker(port={mqtt_broker_port}, topic={mqtt_broker_topic}, user={mqtt_broker_user}, passwd={mqtt_broker_passwd})");
            if (Service == null)
            {
                Log.Error(TAG, "Can't start mqtt broker. Service is null");
                return;
            }
            Service.StartMqttBroker(mqtt_broker_port, mqtt_broker_topic, mqtt_broker_user, mqtt_broker_passwd);
        }

        public void StopMqttBroker()
        {
            Log.Debug(TAG, "StopMqttBroker()");
            if (Service == null)
            {
                Log.Error(TAG, "Can't stop mqtt broker. Service is null");
                return;
            }
            Service.StopMqttBroker();
        }
    }
}