////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ab.Model;
using ab.Services;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class ScriptAddActivity : aSpecialScript
    {
        public static new readonly string TAG = "● script-add-activity";

        protected override int ViewId => Resource.Layout.script_activity;
        protected override int ToolbarId => Resource.Id.script_toolbar;
        protected override int DrawerLayoutId => Resource.Id.script_drawer_layout;
        protected override int NavId => Resource.Id.script_nav_view;

        protected AppCompatEditText ScriptName;

        protected SwitchCompat AutorunTrigger;
        //
        protected AppCompatSpinner HardwareTrigger;
        protected AppCompatSpinner PortTrigger;
        protected AppCompatSpinner StateTrigger;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);

            CardTitle = FindViewById<AppCompatTextView>(Resource.Id.script_card_title);
            CardSubtitle = FindViewById<AppCompatTextView>(Resource.Id.script_card_subtitle);

            ScriptName = FindViewById<AppCompatEditText>(Resource.Id.editTextScriptName);

            AutorunTrigger = FindViewById<SwitchCompat>(Resource.Id.switchAutorunTriggerScript);
            //
            HardwareTrigger = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptTriggerHardware);
            PortTrigger = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptTriggerPort);
            StateTrigger = FindViewById<AppCompatSpinner>(Resource.Id.spinnerScriptTriggerState);

            AutorunTrigger.Checked = false;
            //
            HardwareTrigger.Enabled = false;
            PortTrigger.Enabled = false;
            StateTrigger.Enabled = false;

            ButtonOk = FindViewById<AppCompatButton>(Resource.Id.script_button_ok);

            HardwaresListSpinnerLoad(ref HardwareTrigger);
            //
            ArrayAdapter adapterPortStatuses = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.script_trigger_port_states_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterPortStatuses.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            StateTrigger.Adapter = adapterPortStatuses;
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();

            AutorunTrigger.CheckedChange += AutorunTrigger_CheckedChange;
            HardwareTrigger.ItemSelected += TriggerHardware_ItemSelected;
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            base.OnPause();

            AutorunTrigger.CheckedChange -= AutorunTrigger_CheckedChange;
            HardwareTrigger.ItemSelected -= TriggerHardware_ItemSelected;
        }

        private void TriggerHardware_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int hardware_id = Hardwares.Keys.ElementAt(e.Position);
            Log.Debug(TAG, $"TriggerHardware_ItemSelected() - id={hardware_id}");
            PortsList_UpdateSpinner(hardware_id, ref PortTrigger);
        }

        private void AutorunTrigger_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Log.Debug(TAG, $"AutorunTrigger_CheckedChange - {e.IsChecked}");

            if (e.IsChecked)
            {
                HardwareTrigger.Enabled = true;
                PortTrigger.Enabled = true;
                StateTrigger.Enabled = true;
            }
            else
            {
                HardwareTrigger.Enabled = false;
                PortTrigger.Enabled = false;
                StateTrigger.Enabled = false;
            }
        }

        protected override void ButtonOk_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, $"ButtonOk_Click");

            string obj_id_tag = ButtonOk.Tag?.ToString() ?? "0";
            int obj_id = 0;
            if (Regex.IsMatch(obj_id_tag, @"^\d+$"))
            {
                obj_id = int.Parse(obj_id_tag);
            }

            string errMsg = ReadView(obj_id);

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Log.Error(TAG, $"ButtonOk_Click - {errMsg}");
                CardSubtitle.Text = errMsg;
                CardSubtitle.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            ScriptModel scriptHardware = null;
            if (obj_id > 0)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        scriptHardware = db.Scripts.FirstOrDefault(x => x.Id == obj_id);
                    }
                }
                scriptHardware.Name = ScriptName.Text.Trim();
            }
            if (scriptHardware == null)
            {
                scriptHardware = new ScriptModel()
                {
                    Name = ScriptName.Text
                };
            }
            if (AutorunTrigger.Checked)
            {
                scriptHardware.TriggerPortId = Ports.Keys.ElementAt(PortTrigger.SelectedItemPosition);

                string[] statuses = Resources.GetStringArray(Resource.Array.script_trigger_port_states_array);
                string selectedPortStatus = statuses[StateTrigger.SelectedItemPosition];
                if (selectedPortStatus == GetString(Resource.String.abc_capital_on))
                {
                    scriptHardware.TriggerPortState = true;
                }
                else if (selectedPortStatus == GetString(Resource.String.abc_capital_off))
                {
                    scriptHardware.TriggerPortState = false;
                }
                else
                {
                    scriptHardware.TriggerPortState = null;
                }
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (scriptHardware.Id > 0)
                    {
                        db.Scripts.Update(scriptHardware);
                    }
                    else
                    {
                        db.Scripts.Add(scriptHardware);
                    }
                    db.SaveChanges();
                }
            }

            StartActivity(typeof(ScriptsListActivity));
        }

        protected override string ReadView(int scriptId = 0)
        {
            string errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(ScriptName.Text))
            {
                errMsg = GetText(Resource.String.error_empty_name_script) + System.Environment.NewLine;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (!string.IsNullOrWhiteSpace(ScriptName.Text))
                    {
                        ScriptModel scriptHardwareDuplicate = db.Scripts.FirstOrDefault(x => x.Name.Trim().ToLower() == ScriptName.Text.ToLower() && x.Id != scriptId);
                        if (scriptHardwareDuplicate != null)
                        {
                            errMsg = (errMsg + GetText(Resource.String.duplicate_script_name_error)).Trim() + System.Environment.NewLine;
                        }
                    }
                }
            }
            return errMsg.Trim();
        }
    }
}