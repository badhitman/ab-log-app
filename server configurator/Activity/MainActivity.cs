////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.IO;
using System.Linq;
using System.Threading;
using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static bool DemoDataBase { get; } = true;
        public override void OnBackPressed() { }
        private LinearLayout main_splash;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ThreadPool.QueueUserWorkItem(o => SimulateStartup());
        }

        void SimulateStartup()
        {
            RunOnUiThread(() =>
            {
                SetContentView(Resource.Layout.main_activity);
                main_splash = FindViewById<LinearLayout>(Resource.Id.main_splash);
            });
#if DEBUG
            if (DemoDataBase)
            {
                RunOnUiThread(() =>
                {
                    AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = "automatic delete database!" };
                    main_splash.AddView(appCompatTextView);
                    //Toast.MakeText(this, "automatic delete database!", ToastLength.Short).Show();
                });
                File.Delete(gs.DatabasePathBase);
            }
#endif
            using (var db = new DatabaseContext(gs.DatabasePathBase))
            {
                RunOnUiThread(() =>
                {
                    //Toast.MakeText(this, "checking the database ...", ToastLength.Short).Show(); 
                    AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = "checking the database ..." };
                    main_splash.AddView(appCompatTextView);
                });
                db.Database.EnsureCreatedAsync();

                if (db.Users.Count() == 0)
                {
                    RunOnUiThread(() =>
                    {
                        //Toast.MakeText(this, "load of demo Users ...", ToastLength.Short).Show(); 
                        AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = "load of demo Users ..." };
                        main_splash.AddView(appCompatTextView);
                    });
                    db.Users.AddAsync(new UserModel { Name = "Tom", Email = "tom@gmail.com", Phone = "+79995554422", TelegramId = "00000000000", AlarmSubscriber = true, CommandsAllowed = true });
                    db.Users.AddAsync(new UserModel { Name = "Alice", Email = "alice@gmail.com", Phone = "+75556664411", TelegramId = "159357456258", AlarmSubscriber = false, CommandsAllowed = true });
                    db.SaveChangesAsync();
                }
                if (db.Hardwares.Count() == 0)
                {
                    RunOnUiThread(() =>
                    {
                        //Toast.MakeText(this, "load of demo Hardwares ...", ToastLength.Short).Show(); 
                        AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = "load of demo Hardwares ..." };
                        main_splash.AddView(appCompatTextView);
                    });
                    db.Hardwares.AddAsync(new HardwareModel { Name = "Home", Address = "192.168.1.5", Password = "sec", AlarmSubscriber = true, CommandsAllowed = true });
                    db.Hardwares.AddAsync(new HardwareModel { Name = "Outdoor", Address = "192.168.1.6", Password = "sec", AlarmSubscriber = false, CommandsAllowed = true });
                    db.SaveChangesAsync();
                }
                RunOnUiThread(() =>
                {
                    //Toast.MakeText(this, "caching controllers ...", ToastLength.Short).Show(); 
                    AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = "caching controllers ..." };
                    main_splash.AddView(appCompatTextView);
                });
                DatabaseContext.HardwaresCached = db.Hardwares.OrderBy(x => x.Id).ToArray();
            }
            RunOnUiThread(() =>
            {
                //Toast.MakeText(this, "start app ...", ToastLength.Short).Show(); 
                AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = "start app ..." };
                main_splash.AddView(appCompatTextView);
            });
            StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
        }
    }
}

