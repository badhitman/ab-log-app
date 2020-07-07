////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.Util;
using MQTTnet;
using MQTTnet.Server;
using System.Net;
using System.Text;

namespace ab.Services
{
    public class MqttBrokerManager : IForegroundService
    {
        static readonly string TAG = typeof(MqttBrokerManager).Name;
        private readonly IMqttServer mqttServer;
        LogsContext logsDB = new LogsContext();
        public IPAddress ipAddress => Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];

        public bool isStartedMqtt { get { return mqttServer?.IsStarted ?? false; } }

        public int MqttBrokerPort { get; private set; }
        public string MqttBrokerTopic { get; private set; }
        public string MqttBrokerUsername { get; private set; }
        public string MqttBrokerPassword { get; private set; }

        public MqttBrokerManager()
        {
            Log.Debug(TAG, "~ constructor");
            mqttServer = new MqttFactory().CreateMqttServer();
        }

        public async void StartMqttBroker(int mqtt_broker_port, string mqtt_broker_topic, string mqtt_broker_username = null, string mqtt_broker_passwd = null)
        {
            Log.Debug(TAG, $"StartMqttBroker(port={mqtt_broker_port}, topic={mqtt_broker_topic}, user={mqtt_broker_username}, passwd={mqtt_broker_passwd})");
            MqttBrokerPort = mqtt_broker_port;
            MqttBrokerTopic = mqtt_broker_topic;
            MqttBrokerUsername = mqtt_broker_username;
            MqttBrokerPassword = mqtt_broker_passwd;

            MqttServerOptionsBuilder optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(mqtt_broker_port)
                .WithDefaultEndpointBoundIPAddress(ipAddress)
                .WithDefaultEndpointBoundIPV6Address(IPAddress.None)
                .WithConnectionValidator(MqttConnectionValidator)
                .WithSubscriptionInterceptor(MqttSubscriptionInterceptor)
                .WithApplicationMessageInterceptor(MqttMessageInterceptor);

            await mqttServer.StartAsync(optionsBuilder.Build());
        }

        public async void StopMqttBroker()
        {
            Log.Debug(TAG, "StopMqttBroker()");
            await mqttServer.StopAsync();
        }

        public async void MqttConnectionValidator(MqttConnectionValidatorContext connection_context)
        {
            Log.Debug(TAG, "MqttConnectionValidator");
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, $"MqttConnectionValidator - ClientId={connection_context.ClientId} Username={connection_context.Username}");
            //.WithConnectionValidator(context => 
            //{
            //    if(context.Password == "")
            //    {
            //        context.ReasonCode = MQTTnet.Protocol.MqttConnectReasonCode.BadUserNameOrPassword;
            //        return;
            //    }
            //})
        }

        public async void MqttMessageInterceptor(MqttApplicationMessageInterceptorContext message_context)
        {
            Log.Debug(TAG, "MqttMessageInterceptor");
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, $"MqttMessageInterceptor - ClientId={message_context.ClientId} Topic={message_context.ApplicationMessage.Topic} Payload={Encoding.UTF8.GetString(message_context.ApplicationMessage.Payload)}");
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

        private async void MqttSubscriptionInterceptor(MqttSubscriptionInterceptorContext subscription_context)
        {
            Log.Debug(TAG, "MqttSubscriptionInterceptor"); 
             await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, $"MqttSubscriptionInterceptor - ClientId={subscription_context.ClientId} Topic={subscription_context.TopicFilter.Topic}");
        }

    }
}