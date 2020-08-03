////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using System;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Android.Content;
using Android.Util;
using Microsoft.EntityFrameworkCore;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class ScriptEditActivity : ScriptAddActivity
    {
        public static new readonly string TAG = "● script-edit-activity";

        protected ScriptModel scriptHardware;

        protected LinearLayoutCompat TopLayout;

        AppCompatButton CommandsScript;
        AppCompatButton StartScript;
        AppCompatButton DeleteScript;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    scriptHardware = db.Scripts.Include(x => x.Commands).Include(x => x.TriggerPort).FirstOrDefault(x => x.Id == Intent.Extras.GetInt(nameof(ScriptModel.Id), 0));
                }
            }

            TopLayout = FindViewById<LinearLayoutCompat>(Resource.Id.script_top_layout);
            FooterLayout = FindViewById<LinearLayoutCompat>(Resource.Id.script_footer_layout);

            CardTitle.Text = GetText(Resource.String.script_edit_title);
            CardSubtitle.Text = GetText(Resource.String.script_edit_subtitle);

            DeleteScript = new AppCompatButton(this) { Text = GetText(Resource.String.delete_title) };
            DeleteScript.SetTextColor(Color.DarkRed);
            DeleteScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            FooterLayout.AddView(DeleteScript);

            ButtonOk.Tag = scriptHardware.Id;

            CommandsScript = new AppCompatButton(this) { Text = $"{GetString(Resource.String.commands_title)} ({scriptHardware.Commands.Count})" };
            CommandsScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            TopLayout.AddView(CommandsScript);

            StartScript = new AppCompatButton(this)
            {
                Text = GetText(Resource.String.run_the_script_title),
                Enabled = scriptHardware.Commands.Count > 0
            };
            StartScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);


            TopLayout.AddView(StartScript);

            ScriptName.Text = scriptHardware.Name;
            if (scriptHardware.TriggerPort != null)
            {
                AutorunTrigger.Checked = true;
                PortTrigger.Enabled = true;
                HardwareTrigger.Enabled = true;
                StateTrigger.Enabled = true;

                int indexPosition = Hardwares.Keys.ToList().IndexOf(scriptHardware.TriggerPort.HardwareId);
                HardwareTrigger.SetSelection(indexPosition);

                PortsList_UpdateSpinner(scriptHardware.TriggerPort.HardwareId, ref PortTrigger, scriptHardware.TriggerPort.Id);//, ref Ports

                string[] statuses = Resources.GetStringArray(Resource.Array.script_trigger_port_states_array);
                if (scriptHardware.TriggerPortState == true)
                {
                    indexPosition = Array.IndexOf(statuses, GetString(Resource.String.abc_capital_on));
                }
                else if (scriptHardware.TriggerPortState == false)
                {
                    indexPosition = Array.IndexOf(statuses, GetString(Resource.String.abc_capital_off));
                }
                else
                {
                    indexPosition = Array.IndexOf(statuses, GetString(Resource.String.abc_capital_switch));
                }
                StateTrigger.SetSelection(indexPosition);
            }
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();

            CommandsScript.Click += ButtonConfigScript_Click;
            StartScript.Click += ButtonStartScript_Click;
            DeleteScript.Click += ButtonDeleteScript_Click;

            int count_commands = 0;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    count_commands = db.Commands.Where(x => x.ScriptId == scriptHardware.Id).Count();
                }
            }
            CommandsScript.Text = $"{GetText(Resource.String.commands_title)} ({count_commands})";

            if (count_commands == 0)
            {
                StartScript.Enabled = false;
                StartScript.SetTextColor(Color.Gray);
            }
            else
            {
                StartScript.Enabled = true;
                StartScript.SetTextColor(Color.Indigo);
            }
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            base.OnPause();

            CommandsScript.Click -= ButtonConfigScript_Click;
            StartScript.Click -= ButtonStartScript_Click;
            DeleteScript.Click -= ButtonDeleteScript_Click;
        }

        private void ButtonConfigScript_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "ButtonConfigScript_Click");

            Intent intent = new Intent(this, typeof(CommandsListActivity));
            intent.PutExtra(nameof(scriptHardware.Id), scriptHardware.Id);
            StartActivity(intent);
        }

        private void ButtonStartScript_Click(object sender, EventArgs e)
        {
            string errMsg = ReadView(scriptHardware.Id);

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                errMsg += $"ButtonStartScript_Click - {errMsg}";
                Log.Error(TAG, errMsg);
                using (LogsContext logs = new LogsContext())
                {
                    logs.AddLogRow(LogStatusesEnum.Error, errMsg, TAG);
                }
                CardSubtitle.Text = errMsg;
                CardSubtitle.SetTextColor(Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            TaskModel task;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    task = new TaskModel()
                    {
                        Name = "http trigger",
                        TaskInitiatorType = TaskInitiatorsTypes.Manual,
                        TaskInitiatorId = (sender as AppCompatButton).Id,
                        ScriptId = scriptHardware.Id
                    };
                    task.FinishedAt = task.CreatedAt;
                    db.Tasks.Add(task);
                    db.SaveChanges();
                }
            }
            task.Script = scriptHardware;
            aForegroundService.RunScriptAction(task);
        }

        protected override string ReadView(int scriptId)
        {
            string errMsg = base.ReadView(scriptId);
            if (scriptHardware.Name != ScriptName.Text)
            {
                errMsg += $"{GetString(Resource.String.saving_is_required)}";
            }
            return errMsg;
        }

        private void ButtonDeleteScript_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "ButtonDeleteScript_Click");

            CardTitle.Text = GetText(Resource.String.delete_script_card_title);

            CardSubtitle.Text = GetText(Resource.String.delete_script_card_sub_title);
            CardSubtitle.SetTextColor(Color.IndianRed);

            ScriptName.Enabled = false;

            ButtonOk.Enabled = false;
            ButtonOk.Text = GetText(Resource.String.ok_mute_button_with_remove_script);

            DeleteScript.Enabled = false;
            DeleteScript.SetTextColor(Color.Gray);
            DeleteScript.Click -= ButtonDeleteScript_Click;

            CommandsScript.Enabled = false;
            CommandsScript.SetTextColor(Color.Gray);
            CommandsScript.Click -= ButtonConfigScript_Click;


            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = GetText(Resource.String.footer_text_with_remove_script), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            FooterLayout.AddView(appCompatTextView);

            AppCompatButton ButtonConfirmDeleteScript = new AppCompatButton(this) { Text = GetText(Resource.String.button_confirm_remove) };
            ButtonConfirmDeleteScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ButtonConfirmDeleteScript.SetTextColor(Color.DarkRed);
            ButtonConfirmDeleteScript.Click += new EventHandler((sender, eventArg) =>
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        ScriptModel script = db.Scripts.Find(scriptHardware.Id);
                        db.Scripts.Remove(script);
                        db.SaveChanges();

                        StartActivity(typeof(ScriptsListActivity));
                    }
                }
            });
            FooterLayout.AddView(ButtonConfirmDeleteScript);
        }
    }
}