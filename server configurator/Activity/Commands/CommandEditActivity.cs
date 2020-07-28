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
using System.Collections.Generic;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class CommandEditActivity : CommandAddActivity
    {
        CommandScriptModel command = null;

        AppCompatButton buttonDeleteCommand;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            int CommandId = Intent.Extras.GetInt(nameof(CommandScriptModel.Id), 0);

            if (CommandId > 0)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        command = db.CommandsScript.Include(x => x.ScriptHardware).Include(x => x.PortExecutionCondition).ThenInclude(x => x.Hardware).FirstOrDefault(x => x.Id == CommandId);
                    }
                }
            }

            if (command == null)
            {
                CardSubHeader.Text = GetText(Resource.String.err_title_2);
                TopLayout.Enabled = false;
                BottomLayout.Enabled = false;
                FooterLayout.Enabled = false;
                FormEnableTrigger.Enabled = false;
                CardButtonOk.Enabled = false;
                return;
            }

            ScriptId = command.ScriptHardwareId;

            buttonDeleteCommand = new AppCompatButton(this) { Text = GetText(Resource.String.delete_title) };
            buttonDeleteCommand.SetTextColor(Color.DarkRed);
            buttonDeleteCommand.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            FooterLayout.AddView(buttonDeleteCommand);

            CardHeader.Text = command.ScriptHardware.ToString();
            CardSubHeader.Text = command.ToString();
            appCompatEditTextPauseBeforeStarting.Text = command.PauseBeforeExecution.ToString();
            appCompatCheckBoxHiddenCommand.Checked = command.Hidden;

            if (command.PortExecutionCondition != null)
            {
                FormEnableTrigger.Checked = true;
                
                HardwareFormFieldSpinner.Enabled = true;
                HardwareFormFieldSpinner.SetSelection(Hardwares.Keys.ToList().IndexOf(command.PortExecutionCondition.HardwareId));
                
                PortFormFieldSpinner.Enabled = true;
                PortFormFieldSpinner.SetSelection(Ports.Keys.ToList().IndexOf(command.PortExecutionCondition.Id));
                
                StateFormFieldSpinner.Enabled = true;
                StateFormFieldSpinner.SetSelection(GetIndexPortState(command.ExecutionParametr));
            }

            List<string> types_command = new List<string>(Resources.GetStringArray(Resource.Array.commands_types_array));
            switch (command.TypeCommand)
            {
                case TypesCommands.Controller:
                    appCompatSpinnerTypesCommand.SetSelection(types_command.IndexOf(GetString(Resource.String.command_type_array_item_controller)));
                    TypesCommand_SpinnerItemSelected(null, null);
                    SettingsManageController settingsController = SettingsManageKit as SettingsManageController;
                    settingsController.controllers.SetSelection(Hardwares.Keys.ToList().IndexOf(command.Execution));
                    settingsController.command.Text = command.ExecutionParametr?.ToString();
                    break;
                case TypesCommands.Port:
                    appCompatSpinnerTypesCommand.SetSelection(types_command.IndexOf(GetString(Resource.String.command_type_array_item_port)));
                    TypesCommand_SpinnerItemSelected(null, null);
                    SettingsManagePort settingsPort = SettingsManageKit as SettingsManagePort;
                    PortHardwareModel port = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            port = db.PortsHardwares.FirstOrDefault(x => x.Id == command.Execution);
                        }
                    }
                    settingsPort.controllers.SetSelection(Hardwares.Keys.ToList().IndexOf(port.HardwareId));
                    settingsPort.ports.SetSelection(settingsPort.CommandsPorts.Keys.ToList().IndexOf(port.Id));

                    List<string> port_states_list = new List<string>(Resources.GetStringArray(Resource.Array.script_trigger_port_states_array));
                    settingsPort.states.SetSelection(GetIndexPortState(command.ExecutionParametr));
                    break;
                case TypesCommands.Exit:
                    appCompatSpinnerTypesCommand.SetSelection(types_command.IndexOf(GetString(Resource.String.command_type_array_item_transit)));
                    TypesCommand_SpinnerItemSelected(null, null);
                    SettingsManageTransfer settingsTransfer = SettingsManageKit as SettingsManageTransfer;

                    CommandScriptModel item_command = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            item_command = db.CommandsScript.Include(x => x.ScriptHardware).FirstOrDefault(x => x.Id == command.Execution);
                        }
                    }
                    settingsTransfer.scriptes.SetSelection(settingsTransfer.ScriptsList.Keys.ToList().IndexOf(item_command.ScriptHardwareId));
                    settingsTransfer.steps.SetSelection(settingsTransfer.StepsScriptsList.Keys.ToList().IndexOf(item_command.Id));
                    break;
            }
        }

        int GetIndexPortState(string port_state_as_string)
        {
            List<string> port_states_list = new List<string>(Resources.GetStringArray(Resource.Array.script_trigger_port_states_array));
            bool port_state;
            if (string.IsNullOrWhiteSpace(port_state_as_string) || !bool.TryParse(port_state_as_string, out port_state))
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_switch));
            }

            if (port_state == true)
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_on));
            }
            else if (port_state == false)
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_off));
            }
            else
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_switch));
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            buttonDeleteCommand.Click += ButtonDeleteCommands_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            buttonDeleteCommand.Click -= ButtonDeleteCommands_Click;
        }

        private void ButtonDeleteCommands_Click(object sender, EventArgs e)
        {
            CardHeader.Text = GetText(Resource.String.delete_command_card_title);

            CardSubHeader.Text = GetText(Resource.String.delete_command_card_sub_title);
            CardSubHeader.SetTextColor(Color.IndianRed);

            CardButtonOk.Enabled = false;
            CardButtonOk.Text = GetText(Resource.String.ok_mute_button_with_remove_command);

            buttonDeleteCommand.Enabled = false;
            buttonDeleteCommand.SetTextColor(Color.Gray);
            buttonDeleteCommand.Click -= ButtonDeleteCommands_Click;

            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = GetText(Resource.String.footer_text_with_remove_command), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            FooterLayout.AddView(appCompatTextView);

            AppCompatButton ButtonConfirmDeleteCommand = new AppCompatButton(this) { Text = GetText(Resource.String.button_confirm_remove) };
            ButtonConfirmDeleteCommand.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ButtonConfirmDeleteCommand.SetTextColor(Color.DarkRed);
            ButtonConfirmDeleteCommand.Click += new EventHandler((sender, eventArg) =>
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        db.CommandsScript.Remove(command);
                        db.SaveChanges();

                        Intent CancelIntent = new Intent(this, typeof(CommandsListActivity));
                        CancelIntent.PutExtra(nameof(CommandScriptModel.Id), ScriptId);
                        StartActivity(CancelIntent);
                    }
                }
            });
            FooterLayout.AddView(ButtonConfirmDeleteCommand);
        }
    }
}