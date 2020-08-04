////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore;

namespace ab.Services
{
    public class SettingsManageTransfer : AbstractSettingsManage
    {
        public static new readonly string TAG = "● settings-manage-transfer";

        public Dictionary<int, string> ScriptsList { get; private set; }
        public Dictionary<int, string> StepsList { get; private set; }

        public AppCompatSpinner Scriptes { get; private set; }
        public AppCompatSpinner Steps { get; private set; }

        public SettingsManageTransfer(CommandAddActivity commandAddActivity, AppCompatSpinner scriptes, AppCompatSpinner steps)
        {
            Log.Debug(TAG, "~ constructor");
            ParentActivity = commandAddActivity;

            Scriptes = scriptes;
            Steps = steps;

            ScriptsList = new Dictionary<int, string>() { { 0, ParentActivity.GetString(Resource.String.completing_the_script_title) } };

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    db.Scripts.ToList().ForEach(x =>
                    {
                        ScriptsList.Add(x.Id, $"{(x.Id == Command?.Script.Id ? "→ " : "")}{x}");
                    });
                }
            }

            ArrayAdapter<string> adapterScriptes = new ArrayAdapter<string>(ParentActivity, Android.Resource.Layout.SimpleSpinnerItem, ScriptsList.Values.ToList());
            adapterScriptes.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Scriptes.Adapter = adapterScriptes;

            OnResume();
        }

        private void Scriptes_ItemSelected(object sender, Android.Widget.AdapterView.ItemSelectedEventArgs e)
        {
            int script_id = ScriptsList.Keys.ElementAt(e.Position);

            if (Command != null)
            {
                CommandModel command = null;
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        command = db.Commands.FirstOrDefault(x => x.Id == Command.Execution);
                    }
                }

                if (command != null && command.ScriptId != script_id)
                {
                    Scriptes.SetSelection(ScriptsList.Keys.ToList().IndexOf(command.ScriptId));
                    return;
                }
            }

            StepsList = new Dictionary<int, string>();

            if (script_id == 0)
            {
                ParentActivity.command_executer_id = 0;
                ArrayAdapter adapterSteps = new ArrayAdapter<string>(ParentActivity, Android.Resource.Layout.SimpleSpinnerItem, Array.Empty<string>());
                adapterSteps.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                Steps.Adapter = adapterSteps;
                Steps.Enabled = false;
                Command = null;
            }
            else
            {
                Steps.Enabled = true;
                ScriptModel scriptHardware = null;
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        scriptHardware = db.Scripts.Include(x => x.Commands).FirstOrDefault(x => x.Id == script_id);
                    }
                }
                if (scriptHardware != null)
                {
                    Log.Debug(TAG, $"ScriptCommand_SpinnerItemSelected(id:{script_id}) - {scriptHardware}");
                    scriptHardware.Commands.ForEach(x => { StepsList.Add(x.Id, x.ToString()); });
                }

                ArrayAdapter<string> adapterSteps = new ArrayAdapter<string>(ParentActivity, Android.Resource.Layout.SimpleSpinnerItem, StepsList.Values.ToList());
                adapterSteps.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                Steps.Adapter = adapterSteps;
            }
        }

        private void Steps_ItemSelected(object sender, Android.Widget.AdapterView.ItemSelectedEventArgs e)
        {
            int step_id = StepsList.Keys.ElementAt(e.Position);
            if (Command != null && Command?.Execution != step_id)
            {
                Steps.SetSelection(StepsList.Keys.ToList().IndexOf(Command.Execution));
                return;
            }
            Command = null;

            ParentActivity.command_executer_id = step_id;
        }

        public override void OnResume()
        {
            Scriptes.ItemSelected += Scriptes_ItemSelected;
            Steps.ItemSelected += Steps_ItemSelected;
        }

        public override void OnPause()
        {
            Scriptes.ItemSelected -= Scriptes_ItemSelected;
            Steps.ItemSelected -= Steps_ItemSelected;
        }
    }
}