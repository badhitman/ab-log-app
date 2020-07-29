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
using Microsoft.EntityFrameworkCore;
using Android.Content;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class ScriptEditActivity : ScriptAddActivity
    {
        AppCompatButton buttonDeleteScript;
        AppCompatButton ButtonConfigScript;
        AppCompatButton ButtonStartScript;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (gs.SelectedListPosition < 0 || db.ScriptsHardware.Count() < gs.SelectedListPosition + 1)
                    {
                        string err_title = GetText(Resource.String.err_title_2);
                        ScriptName.Text = err_title;
                        ScriptName.Enabled = false;

                        FormEnableTrigger.Checked = false;
                        FormEnableTrigger.Enabled = false;
                        return;
                    }

                    scriptHardware = db.ScriptsHardware.OrderBy(x => x.Id).Skip(gs.SelectedListPosition).Include(x => x.TriggerPort).ThenInclude(x => x.Hardware).FirstOrDefault();
                    //scriptId = scriptHardware?.Id ?? 0;
                    ScriptName.Text = scriptHardware?.Name;
                    if (scriptHardware.TriggerPort != null)
                    {
                        UpdatePortsListSpinner(scriptHardware.TriggerPort.HardwareId);
                        FormEnableTrigger.Checked = true;
                        int indexPosition = Hardwares.Keys.ToList().IndexOf(scriptHardware.TriggerPort.HardwareId);
                        HardwareFormFieldSpinner.SetSelection(indexPosition);
                        HardwareFormFieldSpinner.Enabled = true;

                        indexPosition = Ports.Keys.ToList().IndexOf(scriptHardware.TriggerPort.Id);
                        PortFormFieldSpinner.SetSelection(indexPosition);
                        PortFormFieldSpinner.Enabled = true;

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
                        StateFormFieldSpinner.SetSelection(indexPosition);
                        StateFormFieldSpinner.Enabled = true;
                    }
                }
            }

            CardHeader.Text = GetText(Resource.String.script_hardware_edit_title);
            CardSubHeader.Text = GetText(Resource.String.script_hardware_edit_subtitle);

            buttonDeleteScript = new AppCompatButton(this) { Text = GetText(Resource.String.delete_title) };
            buttonDeleteScript.SetTextColor(Color.DarkRed);
            buttonDeleteScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            FooterLayout.AddView(buttonDeleteScript);

            int count_commands = 0;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    count_commands = db.CommandsScript.Where(x => x.ScriptHardwareId == scriptHardware.Id).Count();
                }
            }
            ButtonConfigScript = new AppCompatButton(this) { Text = $"{GetText(Resource.String.commands_title)} ({count_commands})" };
            ButtonConfigScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            TopLayout.AddView(ButtonConfigScript);

            ButtonStartScript = new AppCompatButton(this) { Text = GetText(Resource.String.run_the_script_title) };
            ButtonStartScript.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            if (count_commands == 0)
            {
                ButtonStartScript.Enabled = false;
                ButtonStartScript.SetTextColor(Color.Gray);
            }
            else
            {
                ButtonStartScript.SetTextColor(Color.Indigo);
            }
            TopLayout.AddView(ButtonStartScript);
        }

        protected override void OnResume()
        {
            base.OnResume();
            buttonDeleteScript.Click += ButtonDeleteScript_Click;
            ButtonConfigScript.Click += ButtonConfigScript_Click;
            ButtonStartScript.Click += ButtonStartScript_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            buttonDeleteScript.Click -= ButtonDeleteScript_Click;
            ButtonConfigScript.Click -= ButtonConfigScript_Click;
            ButtonStartScript.Click -= ButtonStartScript_Click;
        }

        private void ButtonConfigScript_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(CommandsListActivity));
            intent.PutExtra(nameof(scriptHardware.Id), scriptHardware.Id);
            StartActivity(intent);
        }

        private void ButtonStartScript_Click(object sender, EventArgs e)
        {

        }

        private void ButtonDeleteScript_Click(object sender, EventArgs e)
        {
            CardHeader.Text = GetText(Resource.String.delete_script_card_title);

            CardSubHeader.Text = GetText(Resource.String.delete_script_card_sub_title);
            CardSubHeader.SetTextColor(Color.IndianRed);

            ScriptName.Enabled = false;

            CardButtonOk.Enabled = false;
            CardButtonOk.Text = GetText(Resource.String.ok_mute_button_with_remove_script);

            buttonDeleteScript.Enabled = false;
            buttonDeleteScript.SetTextColor(Color.Gray);
            buttonDeleteScript.Click -= ButtonDeleteScript_Click;

            ButtonConfigScript.Enabled = false;
            ButtonConfigScript.SetTextColor(Color.Gray);
            ButtonConfigScript.Click -= ButtonConfigScript_Click;


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
                        ScriptHardwareModel script = db.ScriptsHardware.Find(scriptHardware.Id);
                        db.ScriptsHardware.Remove(script);
                        db.SaveChanges();

                        StartActivity(typeof(ScriptsListActivity));
                    }
                }
            });
            FooterLayout.AddView(ButtonConfirmDeleteScript);
        }
    }
}