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
            var Gravity = Android.Views.GravityFlags.CenterHorizontal;
#if DEBUG
            if (DemoDataBase)
            {
                RunOnUiThread(() =>
                {
                    AppCompatTextView appCompatTextView = new AppCompatTextView(this)
                    {
                        Text = "automatic delete database!",
                        Gravity = Gravity
                    };
                    main_splash.AddView(appCompatTextView);
                });
                File.Delete(gs.DatabasePathBase);
            }
#endif
            using (var db = new DatabaseContext(gs.DatabasePathBase))
            {
                RunOnUiThread(() =>
                {
                    AppCompatTextView appCompatTextView = new AppCompatTextView(this)
                    {
                        Text = "checking the database ...",
                        Gravity = Gravity
                    };
                    main_splash.AddView(appCompatTextView);
                });
                db.Database.EnsureCreatedAsync();

                if (db.Users.Count() == 0)
                {
                    RunOnUiThread(() =>
                    {
                        AppCompatTextView appCompatTextView = new AppCompatTextView(this)
                        {
                            Text = "load of demo Users ...",
                            Gravity = Gravity
                        };
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
                        AppCompatTextView appCompatTextView = new AppCompatTextView(this)
                        {
                            Text = "load of demo Hardwares ...",
                            Gravity = Gravity
                        };
                        main_splash.AddView(appCompatTextView);
                    });
                    db.Hardwares.AddAsync(new HardwareModel { Name = "Home", Address = "192.168.1.5", Password = "sec", AlarmSubscriber = true, CommandsAllowed = true });
                    db.Hardwares.AddAsync(new HardwareModel { Name = "Outdoor", Address = "192.168.1.6", Password = "sec", AlarmSubscriber = false, CommandsAllowed = true });
                    db.SaveChangesAsync();
                }
                RunOnUiThread(() =>
                {
                    AppCompatTextView appCompatTextView = new AppCompatTextView(this)
                    {
                        Text = "caching controllers ...",
                        Gravity = Gravity
                    };
                    main_splash.AddView(appCompatTextView);
                });
                DatabaseContext.HardwaresCached = db.Hardwares.OrderBy(x => x.Id).ToList();
            }
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "start app ...", ToastLength.Short).Show();
                AppCompatTextView appCompatTextView = new AppCompatTextView(this)
                {
                    Text = "start app ...",
                    Gravity = Gravity
                };
                main_splash.AddView(appCompatTextView);
            });
            StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
        }
    }
}

