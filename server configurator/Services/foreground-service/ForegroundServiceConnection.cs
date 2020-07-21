////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Content;
using Android.OS;
using Android.Util;

namespace ab.Services
{
    public class ForegroundServiceConnection : Java.Lang.Object, IServiceConnection, IForegroundService
    {
        string TAG { get; }
        ServicesActivity myActivity;
        public bool IsConnected { get; private set; }
        public ForegroundServiceBinder Binder { get; private set; }
        LogsContext logsDB = new LogsContext();
        public bool isStartedForegroundService => Binder?.isStartedForegroundService ?? false;

        public ForegroundServiceConnection(ServicesActivity activity)
        {
            Log.Debug(TAG, "~ constructor");

            IsConnected = false;
            Binder = null;
            myActivity = activity;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

            ForegroundServiceBinder serviceBinder = service as ForegroundServiceBinder;
            serviceBinder.myActivity = myActivity;
            Binder = serviceBinder;
            IsConnected = Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            string msg = $"OnServiceDisconnected {name.ClassName}";
            Log.Error(TAG, msg);
            logsDB.AddLogRow(Model.LogStatusesEnum.Warn, msg, TAG);

            IsConnected = false;
            Binder = null;
            myActivity.UpdateUiForStopService();
        }

        public void StartForegroundService(int foreground_service_port)
        {
            Log.Debug(TAG, $"StartForegroundService(port={foreground_service_port})");

            if (Binder == null)
            {
                Log.Error(TAG, "Can't foreground service. Binder == null");
                return;
            }
            Binder.StartForegroundService(foreground_service_port);
        }

        public void StopForegroundService()
        {
            Log.Debug(TAG, "StopForegroundService()");

            if (Binder == null)
            {
                Log.Error(TAG, "Can't stop foreground service. Binder == null");
                return;
            }
            Binder.StopForegroundService();
        }
    }
}