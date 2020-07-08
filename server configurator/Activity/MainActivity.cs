////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static bool reWriteDataBase { get; } = true;
        public override void OnBackPressed() { }
        private LinearLayout main_splash;

        static bool isRunning = false;
        static List<string> LoadingTracert = new List<string>();
        static bool isCompleted = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.main_activity);
            main_splash = FindViewById<LinearLayout>(Resource.Id.main_splash);

            lock (LoadingTracert)
            {
                if (!isCompleted && !isRunning)
                {
                    isRunning = true;
                    ThreadPool.QueueUserWorkItem(o => SimulateSplash());
                }
                else if (isRunning)
                {
                    foreach (string s in LoadingTracert)
                    {
                        main_splash.AddView(new AppCompatTextView(this)
                        {
                            Text = s
                        });
                    }
                }
            }
        }

        async void SimulateSplash()
        {
            LogsContext logsDB = new LogsContext();
            string log_msg = GetText(Resource.String.logs_database_ensure_created);
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, log_msg, ToastLength.Short).Show();
            });
            await logsDB.Database.EnsureCreatedAsync();
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, GetText(Resource.String.start_app_msg));
                        
#if DEBUG
            log_msg = GetText(Resource.String.deleting_outdated_logs);
            AddSplashText(log_msg);
            logsDB.Logs.RemoveRange(logsDB.Logs.Where(x => x.CreatedAt < DateTime.Now.AddDays(-7)).ToArray());

            log_msg = GetText(Resource.String.deleting_main_database_file);
            if (reWriteDataBase)
            {
                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
                AddSplashText(log_msg);
                File.Delete(gs.DatabasePathBase);
            }
#endif
            log_msg = GetText(Resource.String.initializing_db_demo_data);
            DatabaseContext db = new DatabaseContext(gs.DatabasePathBase);
            AddSplashText(log_msg);
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
            await db.Database.EnsureCreatedAsync();

            if (await db.Users.CountAsync() == 0)
            {
                log_msg = GetText(Resource.String.load_demo_users);
                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
                AddSplashText(log_msg);
                await db.Users.AddAsync(new UserModel { Name = "Tom", Email = "tom@gmail.com", Phone = "+79995554422", TelegramId = "00000000000", AlarmSubscriber = true, CommandsAllowed = true });
                await db.Users.AddAsync(new UserModel { Name = "Alice", Email = "alice@gmail.com", Phone = "+75556664411", TelegramId = "159357456258", AlarmSubscriber = false, CommandsAllowed = true });
                await db.SaveChangesAsync();
            }
            if (db.Hardwares.Count() == 0)
            {
                log_msg = GetText(Resource.String.load_demo_hardwares);
                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
                AddSplashText(log_msg);
                await db.Hardwares.AddAsync(new HardwareModel { Name = "Home", Address = "192.168.2.114", Password = "sec", AlarmSubscriber = true, CommandsAllowed = true });
                await db.Hardwares.AddAsync(new HardwareModel { Name = "Outdoor", Address = "192.168.1.6", Password = "sec", AlarmSubscriber = false, CommandsAllowed = true });
                await db.SaveChangesAsync();
            }
            //log_msg = GetText(Resource.String.caching_hardwares_list);
            //await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
            //AddSplashText(log_msg);

            //log_msg = GetText(Resource.String.caching_users_list);
            //await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
            //AddSplashText(log_msg);

            log_msg = GetText(Resource.String.finish_initializing_application);
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg);
            AddSplashText(log_msg);
            isCompleted = true;
            StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
            await db.DisposeAsync();
            await logsDB.DisposeAsync();
        }

        private void AddSplashText(string log_msg)
        {
            lock (LoadingTracert)
            {
                LoadingTracert.Add(log_msg);
                RunOnUiThread(() =>
                {
                    main_splash.AddView(new AppCompatTextView(this) { Text = log_msg });
                });
            }
        }
    }
}

