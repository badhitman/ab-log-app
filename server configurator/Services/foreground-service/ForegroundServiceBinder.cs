////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.OS;
using Android.Util;

namespace ab.Services
{
    public class ForegroundServiceBinder : Binder, IForegroundService
    {
        readonly string TAG = "foreground service binder";
        public ServicesActivity myActivity { get; set; }
        public aForegroundService Service { get; private set; }
        LogsContext logsDB = new LogsContext();
        public bool isStartedForegroundService => throw new NotImplementedException();

        public ForegroundServiceBinder(aForegroundService service)
        {
            Log.Debug(TAG, "~ constructor");

            Service = service;
        }

        public async void StartForegroundService(int foreground_service_port)
        {
            Log.Debug(TAG, $"StartForegroundService(port={foreground_service_port})");

            if (Service == null)
            {
                string msg = "Can't start foreground service. Service is null";
                Log.Error(TAG, msg);
                logsDB.AddLogRow(Model.LogStatusesEnum.Error, msg, TAG);

                return;
            }
            Service.StartForegroundService(foreground_service_port);
        }

        public void StopForegroundService()
        {
            Log.Debug(TAG, "StopForegroundService()");
            if (Service == null)
            {
                string msg = "Can't stop foreground service. Service is null";
                Log.Error(TAG, msg);
                logsDB.AddLogRow(Model.LogStatusesEnum.Error, msg, TAG);

                return;
            }
            Service.StopForegroundService();
        }
    }
}