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
using Android.Content.Res;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
    {
        public static bool reWriteDataBase { get; } = false;
        public override void OnBackPressed() { }
        private LinearLayout main_splash;

        static string TAG = "start-splash";

        static bool isRunning = false;
        static List<string> LoadingTracert = new List<string>();
        static object locker = new object();
        static bool isCompleted = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.main_activity);
            main_splash = FindViewById<LinearLayout>(Resource.Id.main_splash);

            lock (locker)
            {
                if (!isCompleted && !isRunning)
                {
                    isRunning = true;
                    ThreadPool.QueueUserWorkItem(o => SimulateSplash());
                }
                else if (isRunning)
                {
                    if (LoadingTracert == null)
                    {
                        StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
                        return;
                    }

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
            try
            {
                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, GetText(Resource.String.start_app_msg), TAG);
            }
            catch (DbUpdateException ex)
            {
                string err_message = ex.Message;
                if (ex.InnerException != null)
                {
                    err_message += System.Environment.NewLine +
                        ex.InnerException.Message + System.Environment.NewLine + System.Environment.NewLine +
                        GetText(Resource.String.delete_the_log_database_file) + System.Environment.NewLine +
                        LogsContext.DatabasePathLogs;

                    File.Delete(LogsContext.DatabasePathLogs);
                    logsDB = new LogsContext();
                    await logsDB.Database.EnsureCreatedAsync();
                    await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, GetText(Resource.String.start_app_msg), TAG);
                }
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, err_message, ToastLength.Long).Show();
                });
            }

#if DEBUG
            log_msg = GetText(Resource.String.deleting_outdated_logs);
            AddSplashText(log_msg);
            logsDB.Logs.RemoveRange(logsDB.Logs.Where(x => x.CreatedAt < DateTime.Now.AddDays(-7)).ToArray());
            
            if (reWriteDataBase)
            {
                log_msg = GetText(Resource.String.deleting_main_database_file);

                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg, TAG);
                AddSplashText(log_msg);
                File.Delete(gs.DatabasePathBase);
            }
#endif
            log_msg = GetText(Resource.String.initializing_db_demo_data);
            DatabaseContext db = new DatabaseContext(gs.DatabasePathBase);
            AddSplashText(log_msg);
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg, TAG);
            await db.Database.EnsureCreatedAsync();

            if (await db.Users.CountAsync() == 0)
            {
                log_msg = GetText(Resource.String.load_demo_users);
                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg, TAG);
                AddSplashText(log_msg);
                await db.Users.AddAsync(new UserModel { Name = "Tom", Email = "tom@gmail.com", Phone = "+79995554422", TelegramId = "00000000000", AlarmSubscriber = true, CommandsAllowed = true });
                await db.Users.AddAsync(new UserModel { Name = "Alice", Email = "alice@gmail.com", Phone = "+75556664411", TelegramId = "159357456258", AlarmSubscriber = false, CommandsAllowed = true });
                await db.SaveChangesAsync();
            }
            if (await db.Hardwares.CountAsync() == 0)
            {
                log_msg = GetText(Resource.String.load_demo_hardwares);
                await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg, TAG);
                AddSplashText(log_msg);
                await db.Hardwares.AddAsync(new HardwareModel { Name = "Home", Address = "192.168.2.114", Password = "sec", AlarmSubscriber = true, CommandsAllowed = true });
                await db.Hardwares.AddAsync(new HardwareModel { Name = "Outdoor", Address = "192.168.1.6", Password = "sec", AlarmSubscriber = false, CommandsAllowed = true });
                await db.SaveChangesAsync();
            }


            using (StreamReader sr = new StreamReader(Assets.Open("bootstrap.min.css")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.bootstrap_min_css, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("jquery.slim.min.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.jquery_slim_min_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("popper.min.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.popper_min_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("bootstrap.min.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.bootstrap_min_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf1.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf1_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf2.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf2_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf3.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf3_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf4.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf4_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf7.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf7_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf9.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf9_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.cf10.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_cf10_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.pt.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_pt_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }
            using (StreamReader sr = new StreamReader(Assets.Open("onload.root.js")))
            {
                using (StreamWriter sw = new StreamWriter(MyWebViewClient.onload_root_js, false))
                {
                    await sw.WriteAsync(await sr.ReadToEndAsync());
                }
            }

            log_msg = GetText(Resource.String.finish_initializing_application);
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, log_msg, TAG);
            AddSplashText(log_msg);
            isCompleted = true;
            StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
            await db.DisposeAsync();
            await logsDB.DisposeAsync();
            LoadingTracert = null;
        }

        private void AddSplashText(string log_msg)
        {
            lock (locker)
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

