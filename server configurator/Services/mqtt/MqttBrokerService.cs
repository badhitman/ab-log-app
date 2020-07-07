////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using MQTTnet.Server;
using System;
using System.Net;
using Xamarin.Essentials;

namespace ab.Services
{
    [Service(Exported = true, Name = "com.xamarin.ab.listener")]//, IsolatedProcess = true
    public class MqttBrokerService : Service, IForegroundService
    {
        static readonly string TAG = typeof(MqttBrokerService).Name;
        // This is any integer value unique to the application.
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        IForegroundService ForegroundServiceManager;

        public IBinder Binder { get; private set; }

        public bool isStartedMqtt => ForegroundServiceManager?.isStartedMqtt ?? false;

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            Binder = new MqttBrokerServiceBinder(this);
            return Binder;
        }

        public override bool OnUnbind(Intent intent)
        {
            Log.Debug(TAG, "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy");
            StopMqttBroker();
            Binder = null;
            ForegroundServiceManager = null;
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, $"OnStartCommand - start id: {startId}");
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            //return base.OnStartCommand(intent, flags, startId);
            // Code not directly related to publishing the notification has been omitted for clarity.
            // Normally, this method would hold the code to be run when the service is started.
            int listener_port = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_broker_port), 1883);
            string user = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_auth_username), "");
            string pass = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_auth_passwd), "");
            string topic = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_topic), "");
            StartMqttBroker(listener_port, topic, user, pass);

            Notification notification = new Notification.Builder(this)
                .SetContentTitle(GetText(Resource.String.app_name))
                .SetContentText(ipAddress + ":" + listener_port)
                .SetSmallIcon(Resource.Drawable.ic_stat_name)
                .SetContentIntent(BuildIntentToShowServicesActivity())
                .SetOngoing(true)
                .AddAction(BuildRestartMqttBrokerServiceAction())
                .AddAction(BuildStopServiceAction())
                .Build();
            // Enlist this instance of the service as a foreground service
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);

            return StartCommandResult.Sticky;
        }

        private PendingIntent BuildIntentToShowServicesActivity()
        {
            Log.Debug(TAG, "BuildIntentToShowServicesActivity()");

            Intent notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(Constants.ACTION_SERVICE_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private Notification.Action BuildRestartMqttBrokerServiceAction()
        {
            Log.Debug(TAG, "BuildRestartMqttBrokerServiceAction()");

            Intent restartMqttBrokerServiceIntent = new Intent(this, GetType());
            restartMqttBrokerServiceIntent.SetAction(Constants.ACTION_RESTART_SERVICE);
            PendingIntent restartMqttBrokerServicePendingIntent = PendingIntent.GetService(this, 0, restartMqttBrokerServiceIntent, 0);

            Notification.Action.Builder builder = new Notification.Action.Builder(Resource.Drawable.ic_action_restart_mqtt_broker_service, GetText(Resource.String.restart_title), restartMqttBrokerServicePendingIntent);

            return builder.Build();
        }

        private Notification.Action BuildStopServiceAction()
        {
            Log.Debug(TAG, "BuildStopServiceAction()");

            Intent stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            PendingIntent stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            Notification.Action.Builder builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause, GetText(Resource.String.stop_title), stopServicePendingIntent);

            return builder.Build();
        }

        public void StartMqttBroker(int mqtt_broker_port, string mqtt_broker_topic, string mqtt_broker_user = null, string mqtt_broker_passwd = null)
        {
            Log.Debug(TAG, $"StartMqttBroker(port={mqtt_broker_port}, topic={mqtt_broker_topic}, user={mqtt_broker_user}, passwd={mqtt_broker_passwd})");

            if (ForegroundServiceManager == null)
            {
                ForegroundServiceManager = new MqttBrokerManager();
            }

            ForegroundServiceManager.StartMqttBroker(mqtt_broker_port, mqtt_broker_topic, mqtt_broker_user, mqtt_broker_passwd);
        }

        public void StopMqttBroker()
        {
            Log.Debug(TAG, "StopMqttBroker()");

            ForegroundServiceManager.StopMqttBroker();
        }
    }
}