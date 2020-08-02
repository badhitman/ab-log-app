﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System.Linq;
using ab.Model;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace ab.Services
{
    public class SettingsManageController : AbstractSettingsManage
    {
        public static new readonly string TAG = "● settings-manage-controller";

        public AppCompatSpinner Controllers { get; private set; }
        public EditText CommandText { get; private set; }

        public SettingsManageController(CommandAddActivity command_add_activity, AppCompatSpinner controllers, EditText commandText)
        {
            command_add_activity.HardwaresListSpinnerLoad(ref controllers);
            ParentActivity = command_add_activity;

            Controllers = controllers;
            CommandText = commandText;

            OnResume();
        }

        public override void OnResume()
        {
            Controllers.ItemSelected += Controllers_ItemSelected;
            CommandText.TextChanged += CommandText_TextChanged;
        }

        public override void OnPause()
        {
            Controllers.ItemSelected -= Controllers_ItemSelected;
            CommandText.TextChanged -= CommandText_TextChanged;
        }

        private void Controllers_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ParentActivity.command_executer_id = ParentActivity.Hardwares.Keys.ElementAt(e.Position);
            HardwareModel hw = null;
            if (ParentActivity.command_executer_id > 0)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        hw = db.Hardwares.Find(ParentActivity.command_executer_id);
                    }
                }
            }
            if (hw == null)
            {
                Log.Error(TAG, $"{ParentActivity.GetString(Resource.String.err_selected_device_was_not_found_title)} - ControllerCommand_SpinnerItemSelected({ParentActivity.command_executer_id}) - Position:{e.Position}");
            }
            else
            {
                Log.Debug(TAG, $"ControllerCommand_SpinnerItemSelected(hw:{hw}) - Position:{e.Position}");
            }
        }

        private void CommandText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            ParentActivity.command_executer_parameter = CommandText.Text.Trim();
        }
    }
}