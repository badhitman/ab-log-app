////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace ab.Services
{
    [Service(Exported = true, Name = "com.xamarin.ab.mqtt")]
    public class ForegroundServiceMqtt : aForegroundService
    {
        public static new readonly string TAG = "● foreground-service-mqtt";

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");
            Binder = new ForegroundServiceBinder(this);
            return Binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate");

            ForegroundServiceManager = new MqttBrokerManager();
        }
    }
}