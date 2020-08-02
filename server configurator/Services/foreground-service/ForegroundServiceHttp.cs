////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace ab.Services
{
    [Service(Exported = true, Name = "com.xamarin.ab.http")]
    public class ForegroundServiceHttp : aForegroundService
    {
        public static new readonly string TAG = "● http-service";

        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(TAG, "OnBind");

            Binder = new ForegroundServiceBinder(this);
            return Binder;
        }

        public override void OnCreate()
        {
            Log.Info(TAG, "OnCreate");

            base.OnCreate();

            ForegroundServiceManager = new HttpListenerManager();
        }
    }
}