////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.Util;
using MQTTnet;
using MQTTnet.Server;
using System.Linq;
using System.Net;
using System.Text;
using TelegramBotMin;
using Xamarin.Essentials;

namespace ab.Services
{
    public class MqttBrokerManager : IForegroundService
    {
        static readonly string TAG = "mqtt-manager";
        private readonly IMqttServer mqttServer;
        LogsContext logsDB = new LogsContext();
        public IPAddress ipAddress => Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
        public bool isStartedForegroundService { get { return mqttServer?.IsStarted ?? false; } }
        public int MqttBrokerPort { get; private set; }

        public MqttBrokerManager()
        {
            Log.Debug(TAG, "~ constructor");
            mqttServer = new MqttFactory().CreateMqttServer();
        }

        public async void StartForegroundService(int service_port)
        {
            string msg = $"StartForegroundService(port={service_port})";
            Log.Debug(TAG, msg);
            logsDB.AddLogRow(LogStatusesEnum.Trac, msg, TAG);
            MqttBrokerPort = service_port;

            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(service_port)
                .WithDefaultEndpointBoundIPAddress(ipAddress)
                .WithDefaultEndpointBoundIPV6Address(IPAddress.None)
                .WithConnectionValidator(MqttConnectionValidator)
                .WithSubscriptionInterceptor(MqttSubscriptionInterceptor)
                .WithApplicationMessageInterceptor(MqttMessageInterceptor);

            await mqttServer.StartAsync(optionsBuilder.Build());
        }

        public async void StopForegroundService()
        {
            Log.Debug(TAG, "StopForegroundService()");
            logsDB.AddLogRow(LogStatusesEnum.Trac, "StopForegroundService()", TAG);
            await mqttServer.StopAsync();
        }

        public void MqttConnectionValidator(MqttConnectionValidatorContext connection_context)
        {
            Log.Debug(TAG, "MqttConnectionValidator");
            logsDB.AddLogRow(LogStatusesEnum.Trac, $"MqttConnectionValidator - ClientId={connection_context.ClientId} Username={connection_context.Username}", TAG);
        }

        public void MqttMessageInterceptor(MqttApplicationMessageInterceptorContext message_context)
        {
            Log.Debug(TAG, "MqttMessageInterceptor");
            string PayloadTest = Encoding.UTF8.GetString(message_context.ApplicationMessage.Payload);
            
            logsDB.AddLogRow(LogStatusesEnum.Trac, $"MqttMessageInterceptor - ClientId={message_context.ClientId} Topic={message_context.ApplicationMessage.Topic} Payload={PayloadTest}", TAG);
            //string bot_message = $"В топике \"{message_context.ApplicationMessage.Topic}\" новое MQTT сообщение: {PayloadTest}";
            //string token = Preferences.Get(Constants.TELEGRAM_TOKEN, string.Empty);
            //if (!string.IsNullOrWhiteSpace(token))
            //{
            //    TelegramClientCore telegramClient = new TelegramClientCore(token);
            //    if (telegramClient?.Me != null)
            //    {
            //        lock (DatabaseContext.DbLocker)
            //        {
            //            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
            //            {
            //                foreach (UserModel user in db.Users.Where(x => x.AlarmSubscriber))
            //                {
            //                    foreach (TelegramUserModel telegramUser in db.TelegramUsers.Where(xx => xx.LinkedUserId == user.Id))
            //                    {
            //                        telegramClient.sendMessage(telegramUser.TelegramId.ToString(), bot_message.Trim());
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //.WithApplicationMessageInterceptor(context =>
            //{
            //    if (context.ApplicationMessage.Topic != mqtt_broker_topic)
            //    {
            //        context.AcceptPublish = false;
            //        return;
            //        //context.ApplicationMessage.Payload = Encoding.UTF8.GetBytes("The server injected payload.");
            //    }
            //});
        }

        private void MqttSubscriptionInterceptor(MqttSubscriptionInterceptorContext subscription_context)
        {
            Log.Debug(TAG, "MqttSubscriptionInterceptor");
            logsDB.AddLogRow(LogStatusesEnum.Trac, $"MqttSubscriptionInterceptor - ClientId={subscription_context.ClientId} Topic={subscription_context.TopicFilter.Topic}", TAG);
        }
    }
}