////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using System.Linq;
using System.Net;
using System.Threading;
using TelegramBot.TelegramMetadata.AvailableTypes;
using TelegramBot.TelegramMetadata.GettingUpdates;
using TelegramBotMin;
using Xamarin.Essentials;

namespace ab.Services
{
    public abstract class aForegroundService : Service, IForegroundService
    {
        readonly string TAG = "service";
        readonly string TelegramBotTAG = "telegram-bot";

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        LogsContext logsDB = new LogsContext();
        public static IForegroundService ForegroundServiceManager;
        private int TelegramBotSurveyInterval = -1;
        private string TelegramBotToken;
        TelegramClientCore telegramClient;
        public IBinder Binder { get; protected set; }

        public bool isStartedForegroundService => ForegroundServiceManager?.isStartedForegroundService ?? false;

        Thread TelegramBotClientThread;

        private void TelegramBotClientAction()
        {
            string log_msg;
            using (LogsContext log = new LogsContext())
            {
                log_msg = "TelegramBotClientAction()";
                Log.Debug(TelegramBotTAG, log_msg);
                log.AddLogRow(LogStatusesEnum.Tracert, log_msg, TAG);
            }
            if (TelegramBotSurveyInterval < 1 || string.IsNullOrWhiteSpace(TelegramBotToken))
            {
                log_msg = "TelegramBotSurveyInterval < 1 || string.IsNullOrWhiteSpace(TelegramBotToken)";
                Log.Error(TelegramBotTAG, log_msg);
                using (LogsContext log = new LogsContext())
                {
                    log.AddLogRow(LogStatusesEnum.Error, log_msg, TAG);
                }
                return;
            }
            telegramClient = new TelegramClientCore(TelegramBotToken);
            if (telegramClient?.Me == null)
            {
                log_msg = "telegramClient?.Me == null";
                Log.Error(TelegramBotTAG, log_msg);
                using (LogsContext log = new LogsContext())
                {
                    log.AddLogRow(LogStatusesEnum.Error, log_msg, TAG);
                }
                return;
            }

            while (TelegramBotSurveyInterval > 0)
            {
                Log.Debug(TelegramBotTAG, " ~ request telegram updates");
                Update[] updates = telegramClient.getUpdates().Result;
                if (updates == null)
                {
                    log_msg = "telegramClient.getUpdates() == null";
                    Log.Error(TelegramBotTAG, log_msg);
                    using (LogsContext log = new LogsContext())
                    {
                        log.AddLogRow(LogStatusesEnum.Error, log_msg, TAG);
                    }
                    return;
                }
                if (updates.Length > 0)
                {
                    log_msg = $"incoming telegram updates: {updates.Length} items";
                    Log.Info(TelegramBotTAG, log_msg);
                    using (LogsContext log = new LogsContext())
                    {
                        log.AddLogRow(LogStatusesEnum.Information, log_msg, TAG);
                    }
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            updates = updates.Where(x => x.message.chat.type == "private" && !x.message.from.is_bot).ToArray();
                            foreach (Update update in updates)
                            {
                                log_msg = $"incoming telegram message: {update.message.text}";
                                Log.Info(TelegramBotTAG, log_msg);
                                using (LogsContext log = new LogsContext())
                                {
                                    log.AddLogRow(LogStatusesEnum.Information, log_msg, TAG);
                                }
                                db.TelegramMessages.Add(new TelegramMessageModel()
                                {
                                    ChatId = update.message.chat.id,
                                    UpdateId = update.update_id,
                                    MessageId = update.message.message_id,
                                    TelegramBotId = telegramClient.Me.id,
                                    Name = update.message.text
                                });
                                db.SaveChanges();
                            }
                            foreach (var gUser in updates.GroupBy(x => x.message.from.id, (id, users) => new { id, users = users.ToArray() }))
                            {
                                UserClass user = gUser.users[0].message.from;
                                TelegramUserModel telegramUserModel = db.TelegramUsers.FirstOrDefault(x => x.TelegramId == gUser.id);
                                if (telegramUserModel == null)
                                {
                                    telegramUserModel = new TelegramUserModel()
                                    {
                                        Name = $"{user.first_name.Trim()} {user.last_name.Trim()}".Trim(),
                                        TelegramId = user.id,
                                        UserName = user.username,
                                        TelegramParentBotId = telegramClient.Me.id
                                    };
                                    db.TelegramUsers.Add(telegramUserModel);
                                    db.SaveChanges();

                                    log_msg = $"new telegram user: {telegramUserModel}";
                                    Log.Info(TelegramBotTAG, log_msg);
                                    using (LogsContext log = new LogsContext())
                                    {
                                        log.AddLogRow(LogStatusesEnum.Information, log_msg, TAG);
                                    }
                                }
                                else
                                {
                                    string new_full_name = $"{user.first_name.Trim()} {user.last_name.Trim()}".Trim();
                                    string new_username = user.username;
                                    long new_telegram_parent_bot_id = telegramClient.Me.id;
                                    if (telegramUserModel.Name != new_full_name || telegramUserModel.UserName != new_username || telegramUserModel.TelegramParentBotId != new_telegram_parent_bot_id)
                                    {
                                        telegramUserModel.Name = new_full_name;
                                        telegramUserModel.UserName = new_username;
                                        telegramUserModel.TelegramParentBotId = new_telegram_parent_bot_id;
                                        db.TelegramUsers.Update(telegramUserModel);
                                        db.SaveChanges();

                                        log_msg = $"update telegram user: {telegramUserModel}";
                                        Log.Info(TelegramBotTAG, log_msg);
                                        using (LogsContext log = new LogsContext())
                                        {
                                            log.AddLogRow(LogStatusesEnum.Information, log_msg, TAG);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    telegramClient.offset = updates.Max(x => x.update_id);
                }
                if (TelegramBotSurveyInterval > 0)
                {
                    Thread.Sleep(TelegramBotSurveyInterval * 1000);
                }
                else
                {
                    return;
                }
            }
        }

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

            int listener_port = Preferences.Get(base.Resources.GetResourceEntryName(Resource.Id.service_port), 8080);

            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStartedForegroundService)
                {
                    Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_already_running_title)}");
                }
                else
                {
                    TelegramBotSurveyInterval = intent.GetIntExtra(Constants.TELEGRAM_BOT_SURVEY_INTERVAL, -1);
                    TelegramBotToken = intent.GetStringExtra(Constants.TELEGRAM_BOT_TOKEN);

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
                .SetContentTitle(base.Resources.GetString(Resource.String.app_name))
                .SetContentText(content_text)
                .SetSmallIcon(Resource.Drawable.ic_stat_name)
                .SetContentIntent(BuildIntentToShowServicesActivity())
                .SetOngoing(true)
                .Build();

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
            string msg = $"StartForegroundService(port={foreground_service_port})";
            Log.Debug(TAG, msg);

            if (ForegroundServiceManager == null)
            {
                msg = "Error starting foreground service: ForegroundServiceManager == null";
                Log.Error(TAG, msg);
                logsDB.AddLogRow(LogStatusesEnum.Error, msg, TAG);
                return;
            }
            ForegroundServiceManager.StartForegroundService(foreground_service_port);

            TelegramBotClientThread?.Abort();
            TelegramBotClientThread = new Thread(TelegramBotClientAction);
            TelegramBotClientThread.Start();
        }

        public void StopForegroundService()
        {
            string msg = "StopForegroundService()";
            Log.Info(TAG, msg);

            TelegramBotSurveyInterval = -1;
            TelegramBotClientThread.Abort();

            if (ForegroundServiceManager == null)
            {
                msg = "Error stopping foreground service: ForegroundServiceManager == null";
                Log.Error(TAG, msg);
                logsDB.AddLogRow(LogStatusesEnum.Error, msg, TAG);
                return;
            }
            ForegroundServiceManager.StopForegroundService();
        }
    }
}