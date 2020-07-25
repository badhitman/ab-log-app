////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore.Internal;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class ScriptAddActivity : aActivity
    {
        public static readonly string TAG = nameof(ScriptAddActivity);
        protected override int ViewId => Resource.Layout.script_activity;
        protected override int ToolbarId => Resource.Id.script_toolbar;
        protected override int DrawerLayoutId => Resource.Id.script_drawer_layout;
        protected override int NavId => Resource.Id.script_nav_view;

        public static Dictionary<int, string> Hardwares;
        public static Dictionary<int, string> Ports;

        protected AppCompatTextView ScriptCardHeader;
        protected AppCompatTextView ScriptCardSubHeader;

        protected LinearLayout ScriptTopLayout;

        protected AppCompatEditText ScriptName;

        protected Switch ScriptAutorunScriptTrigger;
        protected AppCompatSpinner ScriptHardwareTrigger;
        protected AppCompatSpinner ScriptPortTrigger;
        protected AppCompatSpinner ScriptStateTrigger;

        protected AppCompatButton ScriptCardButtonOk;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Log.Debug(TAG, "OnCreate");
            Hardwares = new Dictionary<int, string>();
            if (this is ScriptAddActivity)
            {
                Hardwares.Add(0, "");
            }
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    foreach (HardwareModel hw in db.Hardwares)
                    {
                        Hardwares.Add(hw.Id, hw.Name);
                    }
                }
            }

            ScriptCardHeader = FindViewById<AppCompatTextView>(Resource.Id.script_card_title);
            ScriptCardSubHeader = FindViewById<AppCompatTextView>(Resource.Id.script_card_subtitle);

            ScriptTopLayout = FindViewById<LinearLayout>(Resource.Id.script_top_layout);

            ScriptName = FindViewById<AppCompatEditText>(Resource.Id.script_name);

            ScriptAutorunScriptTrigger = FindViewById<Switch>(Resource.Id.switchAutorunScriptTrigger);
            ScriptHardwareTrigger = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptHardwareTrigger);
            ScriptHardwareTrigger.Enabled = false;
            ScriptPortTrigger = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptPortTrigger);
            ScriptPortTrigger.Enabled = false;
            ScriptStateTrigger = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptStateTrigger);
            ScriptStateTrigger.Enabled = false;

            ScriptCardButtonOk = FindViewById<AppCompatButton>(Resource.Id.script_button_ok);

            UpdateHardwaresList();

            ArrayAdapter adapterPortStatuses = ArrayAdapter.CreateFromResource(this, Resource.Array.script_trigger_port_states_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterPortStatuses.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            ScriptStateTrigger.Adapter = adapterPortStatuses;
        }

        private void UpdateHardwaresList()
        {
            ArrayAdapter<string> adapterHardwares = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Hardwares.Values.ToList());
            adapterHardwares.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            ScriptHardwareTrigger.Adapter = adapterHardwares;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Log.Debug(TAG, "OnResume");
            ScriptCardButtonOk.Click += ScriptHardwareButtonOk_Click;
            ScriptAutorunScriptTrigger.CheckedChange += ScriptAutorunScriptTrigger_CheckedChange;

            ScriptHardwareTrigger.ItemSelected += ScriptHardwareTrigger_ItemSelected;
            ScriptPortTrigger.ItemSelected += ScriptHardwareTrigger_ItemSelected;
        }

        private void ScriptHardwareTrigger_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            AppCompatSpinner spinner = (AppCompatSpinner)sender;
            Log.Debug(TAG, $"ScriptHardwareTrigger_ItemSelected({Resources.GetResourceEntryName(spinner.Id)})");
            switch (spinner.Id)
            {
                case Resource.Id.spinnerScriptHardwareTrigger:
                    int hardware_id = Hardwares.Keys.ElementAt(e.Position);
                    Ports = new Dictionary<int, string>();
                    if (hardware_id == 0)
                    {
                        Ports = new Dictionary<int, string>();
                    }
                    else
                    {
                        if (Hardwares.ContainsKey(0))
                        {
                            Hardwares.Remove(0);
                            UpdateHardwaresList();
                        }
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
                    ScriptPortTrigger.Adapter = adapterPorts;
                    break;
                case Resource.Id.spinnerScriptPortTrigger:

                    break;
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            Log.Debug(TAG, "OnPause");
            ScriptCardButtonOk.Click -= ScriptHardwareButtonOk_Click;
            ScriptAutorunScriptTrigger.CheckedChange -= ScriptAutorunScriptTrigger_CheckedChange;

            ScriptHardwareTrigger.ItemSelected -= ScriptHardwareTrigger_ItemSelected;
            ScriptPortTrigger.ItemSelected -= ScriptHardwareTrigger_ItemSelected;
        }

        private void ScriptAutorunScriptTrigger_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Log.Debug(TAG, $"ScriptAutorunScriptTrigger_CheckedChange - {e.IsChecked}");
            if (e.IsChecked)
            {
                ScriptHardwareTrigger.Enabled = true;
                ScriptPortTrigger.Enabled = true;
                ScriptStateTrigger.Enabled = true;
            }
            else
            {
                ScriptHardwareTrigger.Enabled = false;
                ScriptPortTrigger.Enabled = false;
                ScriptStateTrigger.Enabled = false;
            }
        }

        protected virtual void ScriptHardwareButtonOk_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "ScriptHardwareButtonOk_Click");
            string errMsg = ReadView();

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Log.Debug(TAG, $"ScriptHardwareButtonOk_Click - {errMsg}");
                ScriptCardSubHeader.Text = errMsg;
                ScriptCardSubHeader.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    ScriptHardwareModel Script = new ScriptHardwareModel()
                    {
                        Name = ScriptName.Text,
                        Notifications = ScriptAutorunScriptTrigger.Checked
                        // AlarmSubscriber = HardwareAlarmSubscribing.Checked,
                        // CommandsAllowed = HardwareCommandsAllowed.Checked,
                        // Name = HardwareName.Text.Trim(),
                        // Address = HardwareAddress.Text.Trim(),
                        // Password = HardwarePassword.Text.Trim()
                    };
                    if (Script.Notifications)
                    {

                    }
                    db.ScriptsHardware.Add(Script);
                    db.SaveChanges();
                }
            }
            StartActivity(typeof(ScriptsListActivity));
        }

        protected string ReadView(int objectId = 0)
        {
            string errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(ScriptName.Text))
            {
                errMsg = GetText(Resource.String.error_empty_name_hardware) + System.Environment.NewLine;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (!string.IsNullOrWhiteSpace(ScriptName.Text) && db.ScriptsHardware.Where(x => x.Name.Trim().ToLower() == ScriptName.Text.ToLower() && x.Id != objectId).Any())
                    {
                        errMsg = (errMsg + GetText(Resource.String.duplicate_script_name_error)).Trim() + System.Environment.NewLine;
                    }
                }
            }

            return errMsg.Trim();
        }
    }
}