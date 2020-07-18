////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using System.Net;
using Xamarin.Essentials;

namespace ab.Services
{
    public abstract class aForegroundService : Service, IForegroundService
    {
        readonly string TAG = "foreground-service";
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;

        public static IForegroundService ForegroundServiceManager;

        public IBinder Binder { get; protected set; }

        public bool isStartedForegroundService => ForegroundServiceManager?.isStartedForegroundService ?? false;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override bool OnUnbind(Intent intent)
        {
            Log.Debug(TAG, "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy");
            StopForegroundService();
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
            int listener_port = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.service_port), 1883);

            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStartedForegroundService)
                {
                    Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_already_running_title)}");
                }
                else
                {
                    Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_being_started_title)}");
                    RegisterForegroundService(ipAddress + ":" + listener_port);
                    StartForegroundService(listener_port);
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_stopped_title)}");
                StopForegroundService();
                StopForeground(true);
                StopSelf();
            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_SERVICE))
            {
                Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.restarting_the_service_title)}");
                StopForegroundService();
                StartForegroundService(listener_port);
            }

            return StartCommandResult.Sticky;
        }

        private void RegisterForegroundService(string content_text)
        {
            Log.Info(TAG, $"RegisterForegroundService: {content_text}");
            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(content_text)
                .SetSmallIcon(Resource.Drawable.ic_stat_name)
                .SetContentIntent(BuildIntentToShowServicesActivity())
                .SetOngoing(true)
                //.AddAction(BuildRestartMqttBrokerServiceAction())
                //.AddAction(BuildStopServiceAction())
                .Build();

            // Enlist this instance of the service as a foreground service
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        private PendingIntent BuildIntentToShowServicesActivity()
        {
            Log.Debug(TAG, "BuildIntentToShowServicesActivity()");

            Intent notificationIntent = new Intent(this, typeof(ServicesActivity));
            notificationIntent.SetAction(Constants.ACTION_SERVICE_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        public void StartForegroundService(int foreground_service_port)
        {
            Log.Debug(TAG, $"StartForegroundService(port={foreground_service_port})");

            ForegroundServiceManager.StartForegroundService(foreground_service_port);
        }

        public void StopForegroundService()
        {
            if (ForegroundServiceManager == null)
            {
                Log.Error(TAG, "StopForegroundService: ForegroundServiceManager == null");
            }
            else
            {
                Log.Debug(TAG, "StopForegroundService()");
                ForegroundServiceManager?.StopForegroundService();
            }
        }
    }
}