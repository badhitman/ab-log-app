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

        protected AppCompatEditText service_tcp_port;

        protected AppCompatRadioButton use_http_radio_button;
        protected AppCompatRadioButton use_mqtt_radio_button;

        protected AppCompatEditText telegram_bot_interval;

        protected AppCompatButton service_start_button;
        protected AppCompatButton service_stop_button;

        bool isStarted = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            if (savedInstanceState != null)
            {
                isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
            }

            base.OnCreate(savedInstanceState);
            OnNewIntent(this.Intent);

            use_http_radio_button = FindViewById<AppCompatRadioButton>(Resource.Id.radioButtonUseHtttpProtocol);
            use_mqtt_radio_button = FindViewById<AppCompatRadioButton>(Resource.Id.radioButtonUseMqttProtocol);

            string use_protocol = Preferences.Get("protocol", "http");
            if (use_protocol == use_mqtt_radio_button.Text.ToLower())
            {
                use_mqtt_radio_button.Checked = true;
            }
            else
            {
                use_http_radio_button.Checked = true;
            }

            service_tcp_port = FindViewById<AppCompatEditText>(Resource.Id.service_port);
            service_tcp_port.Text = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.service_port), 8080).ToString();

            telegram_bot_interval = FindViewById<AppCompatEditText>(Resource.Id.telegram_bot_interval);
            telegram_bot_interval.Text = Preferences.Get(Resources.GetResourceEntryName(Resource.Id.telegram_bot_interval), 0).ToString();

            service_start_button = FindViewById<AppCompatButton>(Resource.Id.service_start_button);
            service_stop_button = FindViewById<AppCompatButton>(Resource.Id.service_stop_button);
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            Bundle bundle = intent.Extras;

            string use_protocol = Preferences.Get("protocol", "http").ToLower();
            if (use_protocol == "mqtt")
            {
                if ((bundle != null && bundle.ContainsKey(Constants.SERVICE_STARTED_KEY)) || (ForegroundServiceMqtt.ForegroundServiceManager != null && ForegroundServiceMqtt.ForegroundServiceManager.isStartedForegroundService))
                {
                    isStarted = true;
                }
            }
            else
            {
                if ((bundle != null && bundle.ContainsKey(Constants.SERVICE_STARTED_KEY)) || (ForegroundServiceHttp.ForegroundServiceManager != null && ForegroundServiceHttp.ForegroundServiceManager.isStartedForegroundService))
                {
                    isStarted = true;
                }
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
                service_stop_button.Click += HandlerServiceStarterButton_Click;
                service_stop_button.Enabled = true;

                service_start_button.Enabled = false;
                service_tcp_port.Enabled = false;
                telegram_bot_interval.Enabled = false;
                use_http_radio_button.Enabled = false;
                use_http_radio_button.CheckedChange -= UseProtocolRadioButton_CheckedChange;
                use_mqtt_radio_button.Enabled = false;
                use_mqtt_radio_button.CheckedChange -= UseProtocolRadioButton_CheckedChange;
            }
            else
            {
                service_start_button.Click += HandlerServiceStarterButton_Click;
                service_start_button.Enabled = true;

                service_stop_button.Enabled = false;

                service_tcp_port.TextChanged += Input_TextChanged;
                service_tcp_port.Enabled = true;

                telegram_bot_interval.TextChanged += Input_TextChanged;
                telegram_bot_interval.Enabled = true;

                use_http_radio_button.Enabled = true;
                use_http_radio_button.CheckedChange += UseProtocolRadioButton_CheckedChange;
                use_mqtt_radio_button.Enabled = true;
                use_mqtt_radio_button.CheckedChange += UseProtocolRadioButton_CheckedChange;
            }
        }

        private void UseProtocolRadioButton_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            AppCompatRadioButton radioButton = (AppCompatRadioButton)sender;
            Log.Debug(TAG, $"UseProtocolRadioButton_CheckedChange: { radioButton.Text} - {e.IsChecked}");

            if (e.IsChecked)
            {
                Preferences.Set("protocol", radioButton.Text.ToLower());
            }

            switch (radioButton.Id)
            {
                case Resource.Id.radioButtonUseHtttpProtocol:
                    if (use_mqtt_radio_button.Checked == e.IsChecked)
                    {
                        use_mqtt_radio_button.Checked = !e.IsChecked;
                    }
                    break;
                case Resource.Id.radioButtonUseMqttProtocol:
                    if (use_http_radio_button.Checked == e.IsChecked)
                    {
                        use_http_radio_button.Checked = !e.IsChecked;
                    }
                    break;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(TAG, "OnPause");

            service_start_button.Click -= HandlerServiceStarterButton_Click;
            service_stop_button.Click -= HandlerServiceStarterButton_Click;
            service_tcp_port.TextChanged -= Input_TextChanged;
            use_http_radio_button.CheckedChange -= UseProtocolRadioButton_CheckedChange;
            use_mqtt_radio_button.CheckedChange -= UseProtocolRadioButton_CheckedChange;

        }

        internal void UpdateUiForStartService()
        {
            Log.Debug(TAG, "Update ui for START service");

            service_start_button.Click -= HandlerServiceStarterButton_Click;
            service_start_button.Enabled = false;

            service_stop_button.Click += HandlerServiceStarterButton_Click;
            service_stop_button.Enabled = true;

            service_tcp_port.TextChanged -= Input_TextChanged;
            service_tcp_port.Enabled = false;

            telegram_bot_interval.TextChanged -= Input_TextChanged;
            telegram_bot_interval.Enabled = false;

            use_http_radio_button.CheckedChange -= UseProtocolRadioButton_CheckedChange;
            use_http_radio_button.Enabled = false;

            use_mqtt_radio_button.CheckedChange -= UseProtocolRadioButton_CheckedChange;
            use_mqtt_radio_button.Enabled = false;
        }

        internal void UpdateUiForStopService()
        {
            Log.Debug(TAG, "Update ui for STOP service");

            service_start_button.Click += HandlerServiceStarterButton_Click;
            service_start_button.Enabled = true;

            service_stop_button.Click -= HandlerServiceStarterButton_Click;
            service_stop_button.Enabled = false;

            service_tcp_port.TextChanged += Input_TextChanged;
            service_tcp_port.Enabled = true;

            telegram_bot_interval.TextChanged += Input_TextChanged;
            telegram_bot_interval.Enabled = true;

            use_http_radio_button.CheckedChange += UseProtocolRadioButton_CheckedChange;
            use_http_radio_button.Enabled = true;
            use_mqtt_radio_button.CheckedChange += UseProtocolRadioButton_CheckedChange;
            use_mqtt_radio_button.Enabled = true;
        }

        private void HandlerServiceStarterButton_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "Handler Service button start click");
            AppCompatButton button = sender as AppCompatButton;

            Intent startServiceIntent;
            Intent stopServiceIntent;

            if (use_mqtt_radio_button.Checked)
            {
                startServiceIntent = new Intent(this, typeof(ForegroundServiceMqtt));
                stopServiceIntent = new Intent(this, typeof(ForegroundServiceMqtt));
            }
            else
            {
                startServiceIntent = new Intent(this, typeof(ForegroundServiceHttp));
                stopServiceIntent = new Intent(this, typeof(ForegroundServiceHttp));
            }

            startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);

            long telegramBotId = Preferences.Get(Constants.TELEGRAM_BOT_ID, (long)0);
            string token = Preferences.Get(base.Resources.GetResourceEntryName(Resource.Id.editTextTelegramBotToken), string.Empty);
            if (telegramBotId > 0 && !string.IsNullOrWhiteSpace(token))
            {
                startServiceIntent.PutExtra(Constants.TELEGRAM_BOT_TOKEN, token);
                startServiceIntent.PutExtra(Constants.TELEGRAM_BOT_SURVEY_INTERVAL, Preferences.Get(Resources.GetResourceEntryName(Resource.Id.telegram_bot_interval), 3));
            }
            else
            {
                Toast.MakeText(this, GetText(Resource.String.telegram_bot_is_not_configured_title), ToastLength.Long).Show();
            }

            switch (button.Id)
            {
                case Resource.Id.service_start_button:
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
                case Resource.Id.service_stop_button:
                    Log.Info(TAG, "User requested that the service be stopped.");
                    UpdateUiForStopService();
                    StopService(stopServiceIntent);
                    isStarted = false;
                    break;
                default:
                    Log.Error(TAG, "HandlerServiceStarterButton_Click() - id");
                    break;
            }
        }

        private void MqttBrokerAuthReq_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            AppCompatCheckBox auth_req = (AppCompatCheckBox)sender;
            Preferences.Set(Resources.GetResourceEntryName(auth_req.Id), auth_req.Checked);
        }

        private void Input_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
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