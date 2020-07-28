////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore.Internal;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class ScriptAddActivity : aSpecialScript
    {
        public static new readonly string TAG = nameof(ScriptAddActivity);
        protected AppCompatEditText ScriptName;
        protected ScriptHardwareModel scriptHardware;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            FormEnableTrigger.Text = GetString(Resource.String.autorun_trigger_title);
            ScriptName = new AppCompatEditText(this);
            TopLayout.AddView(ScriptName);
        }

        protected override void CardButton_Click(object sender, EventArgs e)
        {
            AppCompatButton button = (AppCompatButton)sender;
            Log.Debug(TAG, $"CardButton_Click: {button.Text}");

            if (button.Id == CardButtonCancel.Id)
            {
                StartActivity(typeof(ScriptsListActivity));
                return;
            }

            string errMsg = ReadView();

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Log.Error(TAG, $"CardButton_Click - {errMsg}");
                CardSubHeader.Text = errMsg;
                CardSubHeader.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }
            if (scriptHardware == null)
            {
                scriptHardware = new ScriptHardwareModel()
                {
                    Name = ScriptName.Text
                };
            }

            if (FormEnableTrigger.Checked)
            {
                int port_id = Ports.Keys.ElementAt(PortFormFieldSpinner.SelectedItemPosition);
                scriptHardware.TriggerPortId = port_id;

                string[] statuses = Resources.GetStringArray(Resource.Array.script_trigger_port_states_array);
                string selectedPortStatus = statuses[StateFormFieldSpinner.SelectedItemPosition];
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
            else
            {
                scriptHardware.TriggerPortId = null;
                scriptHardware.TriggerPort = null;
                scriptHardware.TriggerPortState = null;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (scriptHardware.Id == 0)
                    {
                        db.ScriptsHardware.Add(scriptHardware);
                    }
                    else
                    {
                        db.ScriptsHardware.Update(scriptHardware);
                    }
                    db.SaveChanges();
                }
            }

            StartActivity(typeof(ScriptsListActivity));
        }

        protected override string ReadView()
        {
            string errMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(ScriptName.Text))
            {
                errMsg = GetText(Resource.String.error_empty_name_hardware) + System.Environment.NewLine;
            }
            int scriptHardwareId = scriptHardware?.Id ?? 0;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {

                    if (!string.IsNullOrWhiteSpace(ScriptName.Text))
                    {
                        ScriptHardwareModel scriptHardwareDuplicate = db.ScriptsHardware.FirstOrDefault(x => x.Name.Trim().ToLower() == ScriptName.Text.ToLower() && x.Id != scriptHardwareId);
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