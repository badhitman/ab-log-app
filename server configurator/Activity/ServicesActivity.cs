////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System;
using Xamarin.Essentials;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class ServicesActivity : aActivity
    {
        static readonly string TAG = typeof(ServicesActivity).Name;

        protected override int ViewId => Resource.Layout.services_activity;
        protected override int ToolbarId => Resource.Id.services_toolbar;
        protected override int DrawerLayoutId => Resource.Id.services_drawer_layout;
        protected override int NavId => Resource.Id.services_nav_view;

        protected AppCompatEditText mqtt_broker_tcp_port;
        protected AppCompatCheckBox mqtt_auth_check_box;
        protected AppCompatEditText mqtt_auth_username;
        protected AppCompatEditText mqtt_auth_passwd;
        protected AppCompatEditText mqtt_topic;

        protected AppCompatButton mqtt_broker_start_button;
        protected AppCompatButton mqtt_broker_stop_button;

        Intent startServiceIntent;
        Intent stopServiceIntent;
        bool isStarted = false;

        //public static MqttBrokerConnection serviceConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            if (savedInstanceState != null)
            {
                isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
            }

            base.OnCreate(savedInstanceState);
            OnNewIntent(this.Intent);

            mqtt_broker_start_button = FindViewById<AppCompatButton>(Resource.Id.mqtt_broker_start_button);
            mqtt_broker_stop_button = FindViewById<AppCompatButton>(Resource.Id.mqtt_broker_stop_button);

            startServiceIntent = new Intent(this, typeof(MqttBrokerService));
            startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
            //
            stopServiceIntent = new Intent(this, typeof(MqttBrokerService));
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);

            mqtt_broker_tcp_port = FindViewById<AppCompatEditText>(Resource.Id.mqtt_broker_port);
            mqtt_broker_tcp_port.Text = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_broker_port), 8080).ToString();
            //
            mqtt_auth_check_box = FindViewById<AppCompatCheckBox>(Resource.Id.mqtt_auth_check_box);
            mqtt_auth_check_box.Checked = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_auth_check_box), false);

            mqtt_auth_username = FindViewById<AppCompatEditText>(Resource.Id.mqtt_auth_username);
            mqtt_auth_username.Text = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_auth_username), "");
            mqtt_auth_passwd = FindViewById<AppCompatEditText>(Resource.Id.mqtt_auth_passwd);
            mqtt_auth_passwd.Text = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_auth_passwd), "");
            mqtt_topic = FindViewById<AppCompatEditText>(Resource.Id.mqtt_topic);
            mqtt_topic.Text = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.mqtt_topic), "");

            //if (serviceConnection == null)
            //{
            //    serviceConnection = new MqttBrokerConnection(this);
            //}
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            Bundle bundle = intent.Extras;
            if ((bundle != null && bundle.ContainsKey(Constants.SERVICE_STARTED_KEY)) || (MqttBrokerService.ForegroundServiceManager != null && MqttBrokerService.ForegroundServiceManager.isStartedMqtt))
            {
                isStarted = true;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(Constants.SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug(TAG, "OnResume");

            if (isStarted)
            {
                mqtt_broker_stop_button.Click += HandlerMqttBrokerButton_Click;
                mqtt_broker_stop_button.Enabled = true;
                mqtt_broker_start_button.Enabled = false;
                mqtt_broker_tcp_port.Enabled = false;
                mqtt_auth_check_box.Enabled = false;
                mqtt_auth_username.Enabled = false;
                mqtt_auth_passwd.Enabled = false;
                mqtt_topic.Enabled = false;
            }
            else
            {
                mqtt_broker_start_button.Click += HandlerMqttBrokerButton_Click;
                mqtt_broker_start_button.Enabled = true;
                mqtt_broker_stop_button.Enabled = false;

                mqtt_broker_tcp_port.TextChanged += MqttBroker_TextChanged;
                mqtt_broker_tcp_port.Enabled = true;
                mqtt_auth_check_box.CheckedChange += MqttBrokerAuthReq_CheckedChange;
                mqtt_auth_check_box.Enabled = true;
                mqtt_auth_username.Enabled = mqtt_auth_check_box.Checked;
                mqtt_auth_username.TextChanged += MqttBroker_TextChanged;
                mqtt_auth_passwd.Enabled = mqtt_auth_check_box.Checked;
                mqtt_auth_passwd.TextChanged += MqttBroker_TextChanged;
                mqtt_topic.Enabled = mqtt_auth_check_box.Checked;
                mqtt_topic.TextChanged += MqttBroker_TextChanged;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(TAG, "OnPause");

            mqtt_broker_start_button.Click -= HandlerMqttBrokerButton_Click;
            mqtt_broker_stop_button.Click -= HandlerMqttBrokerButton_Click;
            mqtt_broker_tcp_port.TextChanged -= MqttBroker_TextChanged;
            mqtt_auth_check_box.CheckedChange -= MqttBrokerAuthReq_CheckedChange;
            mqtt_auth_username.TextChanged -= MqttBroker_TextChanged;
            mqtt_auth_passwd.TextChanged -= MqttBroker_TextChanged;
            mqtt_topic.TextChanged -= MqttBroker_TextChanged;
        }

        internal void UpdateUiForStartService()
        {
            Log.Debug(TAG, "Update ui for START service");

            mqtt_broker_start_button.Click -= HandlerMqttBrokerButton_Click;
            mqtt_broker_start_button.Enabled = false;

            mqtt_broker_stop_button.Click += HandlerMqttBrokerButton_Click;
            mqtt_broker_stop_button.Enabled = true;

            mqtt_broker_tcp_port.TextChanged -= MqttBroker_TextChanged;
            mqtt_broker_tcp_port.Enabled = false;

            mqtt_auth_check_box.CheckedChange -= MqttBrokerAuthReq_CheckedChange;
            mqtt_auth_check_box.Enabled = false;

            mqtt_auth_username.TextChanged -= MqttBroker_TextChanged;
            mqtt_auth_username.Enabled = false;
            mqtt_auth_passwd.TextChanged -= MqttBroker_TextChanged;
            mqtt_auth_passwd.Enabled = false;
            mqtt_topic.TextChanged -= MqttBroker_TextChanged;
            mqtt_topic.Enabled = false;
        }

        internal void UpdateUiForStopService()
        {
            Log.Debug(TAG, "Update ui for STOP service");

            mqtt_broker_start_button.Click += HandlerMqttBrokerButton_Click;
            mqtt_broker_start_button.Enabled = true;

            mqtt_broker_stop_button.Click -= HandlerMqttBrokerButton_Click;
            mqtt_broker_stop_button.Enabled = false;

            mqtt_broker_tcp_port.TextChanged += MqttBroker_TextChanged;
            mqtt_broker_tcp_port.Enabled = true;

            mqtt_auth_check_box.CheckedChange += MqttBrokerAuthReq_CheckedChange;
            mqtt_auth_check_box.Enabled = true;

            if (mqtt_auth_check_box.Checked)
            {
                mqtt_auth_username.Enabled = mqtt_auth_passwd.Enabled = mqtt_topic.Enabled = true;
                mqtt_auth_username.TextChanged += MqttBroker_TextChanged;
                mqtt_auth_passwd.TextChanged += MqttBroker_TextChanged;
                mqtt_topic.TextChanged += MqttBroker_TextChanged;
            }
        }

        private void HandlerMqttBrokerButton_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "Handler MqttBroker button start click");
            AppCompatButton button = sender as AppCompatButton;

            switch (button.Id)
            {
                case Resource.Id.mqtt_broker_start_button:
                    Log.Info(TAG, "User requested that the service be started.");
                    UpdateUiForStartService();

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        StartForegroundService(startServiceIntent);
                    }
                    else
                    {
                        StartService(startServiceIntent);
                    }

                    isStarted = true;
                    break;
                case Resource.Id.mqtt_broker_stop_button:
                    Log.Info(TAG, "User requested that the service be stopped.");
                    UpdateUiForStopService();
                    StopService(stopServiceIntent);
                    isStarted = false;
                    break;
                default:
                    Log.Error(TAG, "HandlerMqttBrokerButton_Click() - id");
                    break;
            }
        }

        private void MqttBrokerAuthReq_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            AppCompatCheckBox auth_req = (AppCompatCheckBox)sender;
            Preferences.Set(Resources.GetResourceEntryName(auth_req.Id), auth_req.Checked);
            mqtt_auth_username.Enabled = mqtt_auth_passwd.Enabled = mqtt_topic.Enabled = auth_req.Checked;
        }

        private void MqttBroker_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            AppCompatEditText text_field = (AppCompatEditText)sender;
            if (text_field.InputType == Android.Text.InputTypes.ClassNumber)
            {
                Preferences.Set(Resources.GetResourceEntryName(text_field.Id), int.Parse("0" + text_field.Text));
            }
            else
            {
                Preferences.Set(Resources.GetResourceEntryName(text_field.Id), text_field.Text);
            }
        }
    }
}