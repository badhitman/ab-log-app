////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace ab
{
    public abstract class aSpecialScript : aActivity
    {
        public static readonly string TAG = nameof(aSpecialScript);

        protected override int ViewId => Resource.Layout.script_hardware_activity;
        protected override int ToolbarId => Resource.Id.script_hardware_toolbar;
        protected override int DrawerLayoutId => Resource.Id.script_hardware_drawer_layout;
        protected override int NavId => Resource.Id.script_hardware_nav_view;

        public static Dictionary<int, string> Hardwares;
        public static Dictionary<int, string> Ports;

        protected int selected_port_id = 0;

        protected AppCompatTextView CardHeader;
        protected AppCompatTextView CardSubHeader;

        protected LinearLayout TopLayout;
        protected LinearLayout BottomLayout;
        protected LinearLayout FooterLayout;

        protected Switch FormEnableTrigger;
        protected AppCompatSpinner HardwareFormFieldSpinner;
        protected AppCompatSpinner PortFormFieldSpinner;
        protected AppCompatSpinner StateFormFieldSpinner;

        protected ArrayAdapter adapterPortStatuses;

        protected AppCompatButton CardButtonOk;
        protected AppCompatButton CardButtonCancel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Log.Debug(TAG, "OnCreate");

            CardHeader = FindViewById<AppCompatTextView>(Resource.Id.script_card_title);
            CardSubHeader = FindViewById<AppCompatTextView>(Resource.Id.script_card_subtitle);

            TopLayout = FindViewById<LinearLayout>(Resource.Id.script_top_layout);
            BottomLayout = FindViewById<LinearLayout>(Resource.Id.script_bottom_layout);
            FooterLayout = FindViewById<LinearLayout>(Resource.Id.script_footer_layout);

            FormEnableTrigger = FindViewById<Switch>(Resource.Id.switchAutorunScriptTrigger);
            HardwareFormFieldSpinner = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptHardwareTrigger);
            HardwareFormFieldSpinner.Enabled = false;
            PortFormFieldSpinner = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptPortTrigger);
            PortFormFieldSpinner.Enabled = false;
            StateFormFieldSpinner = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptStateTrigger);
            StateFormFieldSpinner.Enabled = false;

            CardButtonOk = FindViewById<AppCompatButton>(Resource.Id.script_button_ok);
            CardButtonCancel = FindViewById<AppCompatButton>(Resource.Id.cancel_button_ok);

            Hardwares = new Dictionary<int, string>();

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    foreach (HardwareModel hw in db.Hardwares)
                    {
                        Hardwares.Add(hw.Id, hw.ToString());
                    }
                }
            }

            UpdateHardwaresListSpinner(HardwareFormFieldSpinner);

            adapterPortStatuses = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.script_trigger_port_states_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterPortStatuses.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            StateFormFieldSpinner.Adapter = adapterPortStatuses;
        }

        protected void UpdateHardwaresListSpinner(AppCompatSpinner appCompatSpinnerHardwares)
        {
            ArrayAdapter<string> adapterHardwares = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Hardwares.Values.ToList());
            adapterHardwares.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            appCompatSpinnerHardwares.Adapter = adapterHardwares;
        }

        protected void HardwareSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            AppCompatSpinner spinner = (AppCompatSpinner)sender;
            Log.Debug(TAG, $"HardwareSpinner_ItemSelected(spinner id:{spinner.Id})");

            int hardware_id = Hardwares.Keys.ElementAt(e.Position);
            UpdatePortsListSpinner(hardware_id);
        }

        protected void UpdatePortsListSpinner(int hardware_id = 0)
        {
            Log.Debug(TAG, $"UpdatePortsListSpinner({hardware_id})");
            Ports = new Dictionary<int, string>();
            if (hardware_id > 0)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        foreach (PortHardwareModel portHardware in db.PortsHardwares.Where(x => x.HardwareId == hardware_id))
                        {
                            Ports.Add(portHardware.Id, portHardware.ToString());
                        }
                    }
                }
            }
            ArrayAdapter<string> adapterPorts = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Ports.Values.ToList());
            adapterPorts.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            PortFormFieldSpinner.Adapter = adapterPorts;
            if (selected_port_id > 0)
            {
                PortFormFieldSpinner.SetSelection(Ports.Keys.ToList().IndexOf(selected_port_id));
            }
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();
            CardButtonOk.Click += CardButton_Click;
            CardButtonCancel.Click += CardButton_Click;
            FormEnableTrigger.CheckedChange += FormEnableToggler_CheckedChange;

            HardwareFormFieldSpinner.ItemSelected += HardwareSpinner_ItemSelected;
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            base.OnPause();
            CardButtonOk.Click -= CardButton_Click;
            CardButtonCancel.Click -= CardButton_Click;
            FormEnableTrigger.CheckedChange -= FormEnableToggler_CheckedChange;

            HardwareFormFieldSpinner.ItemSelected -= HardwareSpinner_ItemSelected;
        }

        protected int GetIndexPortState(bool? port_state_as_bool, List<string> port_states_list)
        {
            if (port_state_as_bool == true)
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_on));
            }
            else if (port_state_as_bool == false)
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_off));
            }
            else
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_switch));
            }
        }

        protected int GetIndexPortState(string port_state_as_string)
        {
            Log.Debug(TAG, $"GetIndexPortState({port_state_as_string})");
            List<string> port_states_list = new List<string>(Resources.GetStringArray(Resource.Array.script_trigger_port_states_array));
            bool port_state;
            if (string.IsNullOrWhiteSpace(port_state_as_string) || !bool.TryParse(port_state_as_string, out port_state))
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_switch));
            }

            return GetIndexPortState(port_state, port_states_list);
        }

        private void FormEnableToggler_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Log.Debug(TAG, $"FormEnableToggler_CheckedChange - {e.IsChecked}");
            if (e.IsChecked)
            {
                HardwareFormFieldSpinner.Enabled = true;
                PortFormFieldSpinner.Enabled = true;
                StateFormFieldSpinner.Enabled = true;
            }
            else
            {
                HardwareFormFieldSpinner.Enabled = false;
                PortFormFieldSpinner.Enabled = false;
                StateFormFieldSpinner.Enabled = false;
            }
        }

        protected abstract void CardButton_Click(object sender, EventArgs e);
        protected abstract string ReadView();
    }
}