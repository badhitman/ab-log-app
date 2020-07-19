﻿////////////////////////////////////////////////
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
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, msg, TAG);
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
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, "StopForegroundService()", TAG);
            await mqttServer.StopAsync();
        }

        public async void MqttConnectionValidator(MqttConnectionValidatorContext connection_context)
        {
            Log.Debug(TAG, "MqttConnectionValidator");
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, $"MqttConnectionValidator - ClientId={connection_context.ClientId} Username={connection_context.Username}", TAG);
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
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, $"MqttMessageInterceptor - ClientId={message_context.ClientId} Topic={message_context.ApplicationMessage.Topic} Payload={Encoding.UTF8.GetString(message_context.ApplicationMessage.Payload)}", TAG);
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
            await logsDB.AddLogRowAsync(LogStatusesEnum.Tracert, $"MqttSubscriptionInterceptor - ClientId={subscription_context.ClientId} Topic={subscription_context.TopicFilter.Topic}", TAG);
        }
    }
}