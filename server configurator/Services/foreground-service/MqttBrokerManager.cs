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
        public static readonly string TAG = "● mqtt-broker-manager";

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
            logsDB.AddLogRow(LogStatusesEnum.Info, msg, TAG);

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
            logsDB.AddLogRow(LogStatusesEnum.Info, "StopForegroundService()", TAG);
            await mqttServer.StopAsync();
        }

        public void MqttConnectionValidator(MqttConnectionValidatorContext connection_context)
        {
            Log.Debug(TAG, "MqttConnectionValidator");
            logsDB.AddLogRow(LogStatusesEnum.Info, $"MqttConnectionValidator - ClientId={connection_context.ClientId} Username={connection_context.Username}", TAG);
        }

        public void MqttMessageInterceptor(MqttApplicationMessageInterceptorContext message_context)
        {
            Log.Debug(TAG, "MqttMessageInterceptor");
            string PayloadTest = Encoding.UTF8.GetString(message_context.ApplicationMessage.Payload);
            
            logsDB.AddLogRow(LogStatusesEnum.Info, $"MqttMessageInterceptor - ClientId={message_context.ClientId} Topic={message_context.ApplicationMessage.Topic} Payload={PayloadTest}", TAG);
        }

        private void MqttSubscriptionInterceptor(MqttSubscriptionInterceptorContext subscription_context)
        {
            Log.Debug(TAG, "MqttSubscriptionInterceptor");
            logsDB.AddLogRow(LogStatusesEnum.Info, $"MqttSubscriptionInterceptor - ClientId={subscription_context.ClientId} Topic={subscription_context.TopicFilter.Topic}", TAG);
        }
    }
}