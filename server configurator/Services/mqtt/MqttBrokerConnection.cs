////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Content;
using Android.OS;
using Android.Util;
using MQTTnet.Server;
using System;

namespace ab.Services
{
    public class MqttBrokerConnection : Java.Lang.Object, IServiceConnection, IForegroundService
    {
        static readonly string TAG = typeof(MqttBrokerConnection).Name;

        ServicesActivity myActivity;
        public bool IsConnected { get; private set; }
        public MqttBrokerServiceBinder Binder { get; private set; }

        public bool isStartedMqtt => Binder?.isStartedMqtt ?? false;

        public MqttBrokerConnection(ServicesActivity activity)
        {
            Log.Debug(TAG, "~ constructor");
            IsConnected = false;
            Binder = null;
            myActivity = activity;
        }

        public void OnServiceConnected(ComponentName name, IBinder binder)
        {
            Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");
            MqttBrokerServiceBinder serviceBinder = binder as MqttBrokerServiceBinder;
            serviceBinder.myActivity = myActivity;
            Binder = serviceBinder;
            IsConnected = Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Warn(TAG, $"OnServiceDisconnected {name.ClassName}");
            IsConnected = false;
            Binder = null;
            myActivity.UpdateUiForStopService();
        }

        public void StartMqttBroker(int mqtt_broker_port, string mqtt_broker_topic, string mqtt_broker_user = null, string mqtt_broker_passwd = null)
        {
            Log.Debug(TAG, $"StartMqttBroker(port={mqtt_broker_port}, topic={mqtt_broker_topic}, user={mqtt_broker_user}, passwd={mqtt_broker_passwd})");
            if (Binder == null)
            {
                Log.Error(TAG, "Can't start mqtt broker. Binder == null");
                return;
            }
            Binder.StartMqttBroker(mqtt_broker_port, mqtt_broker_topic, mqtt_broker_user, mqtt_broker_passwd);
        }

        public void StopMqttBroker()
        {
            Log.Debug(TAG, "StopMqttBroker()");
            if (Binder == null)
            {
                Log.Error(TAG, "Can't stop mqtt broker. Binder == null");
                return;
            }
            Binder.StopMqttBroker();
        }
    }
}