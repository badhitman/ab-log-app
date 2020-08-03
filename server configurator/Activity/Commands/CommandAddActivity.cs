////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class CommandAddActivity : aSpecialScript
    {
        public static new readonly string TAG = "● command-add-activity";

        protected override int ViewId => Resource.Layout.command_activity;
        protected override int ToolbarId => Resource.Id.command_toolbar;
        protected override int DrawerLayoutId => Resource.Id.command_drawer_layout;
        protected override int NavId => Resource.Id.command_nav_view;

        protected int ScriptId;

        protected AbstractSettingsManage SettingsManageKit;

        protected AppCompatCheckBox HiddenCommandCheckBox;
        protected AppCompatEditText PauseSecondsBeforeStarting;

        protected SwitchCompat RequiredCondition;
        protected AppCompatSpinner HardwareCondition;
        protected AppCompatSpinner PortCondition;
        protected AppCompatSpinner StateCondition;

        protected AppCompatSpinner TypesCommand;
        protected AppCompatTextView about_selected_command;

        private TypesCommands SelectedTypeCommand;
        string selected_type_command_name;

        LinearLayoutCompat CommandConfigForm;
        LinearLayoutCompat CommandConfigFormBody;

        public int command_executer_id;
        public object command_executer_parameter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);

            ScriptId = Intent.Extras.GetInt(nameof(ScriptModel.Id), 0);

            CardTitle = FindViewById<AppCompatTextView>(Resource.Id.command_card_title);
            CardSubtitle = FindViewById<AppCompatTextView>(Resource.Id.command_card_subtitle);

            HiddenCommandCheckBox = FindViewById<AppCompatCheckBox>(Resource.Id.checkBoxHiddenCommand);
            PauseSecondsBeforeStarting = FindViewById<AppCompatEditText>(Resource.Id.editTextPauseSecBeforeExecuting);
            FooterLayout = FindViewById<LinearLayoutCompat>(Resource.Id.command_footer_layout);

            RequiredCondition = FindViewById<SwitchCompat>(Resource.Id.switchCondition);

            HardwareCondition = FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandHardwareCondition);
            PortCondition = FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandPortCondition);
            StateCondition = FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandStateCondition);

            RequiredCondition.Checked = false;
            //
            HardwareCondition.Enabled = false;
            PortCondition.Enabled = false;
            StateCondition.Enabled = false;

            TypesCommand = FindViewById<AppCompatSpinner>(Resource.Id.spinnerTypeCommand);

            about_selected_command = FindViewById<AppCompatTextView>(Resource.Id.about_selected_command);

            CommandConfigForm = FindViewById<LinearLayoutCompat>(Resource.Id.command_config_form);

            ButtonOk = FindViewById<AppCompatButton>(Resource.Id.command_button_ok);

            HardwaresListSpinnerLoad(ref HardwareCondition);

            ArrayAdapter adapterPortStatusesCondition = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.required_condition_port_states_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterPortStatusesCondition.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            StateCondition.Adapter = adapterPortStatusesCondition;

            ArrayAdapter adapterTypesCommands = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.commands_types_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterTypesCommands.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            TypesCommand.Adapter = adapterTypesCommands;
        }

        protected override void OnResume()
        {
            base.OnResume();
            SettingsManageKit?.OnResume();
            RequiredCondition.CheckedChange += RequiredCondition_CheckedChange;
            HardwareCondition.ItemSelected += HardwareCondition_ItemSelected;
            TypesCommand.ItemSelected += TypesCommand_ItemSelected;
        }

        private void HardwareCondition_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int hardware_id = Hardwares.Keys.ElementAt(e.Position);
            Log.Debug(TAG, $"TriggerHardware_ItemSelected() - id={hardware_id}");
            PortsList_UpdateSpinner(hardware_id, ref PortCondition);
        }

        protected override void OnPause()
        {
            base.OnPause();
            SettingsManageKit?.OnPause();
            RequiredCondition.CheckedChange -= RequiredCondition_CheckedChange;
            HardwareCondition.ItemSelected -= HardwareCondition_ItemSelected;
            TypesCommand.ItemSelected -= TypesCommand_ItemSelected;
        }

        protected void TypesCommand_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //command_executer_id = -1;
            //command_executer_parameter = null;
            int Position = TypesCommand.SelectedItemPosition;
            selected_type_command_name = Resources.GetStringArray(Resource.Array.commands_types_array)[Position];

            string command_type_array_item_port = GetString(Resource.String.command_type_array_item_port);
            string command_type_array_item_controller = GetString(Resource.String.command_type_array_item_controller);
            string command_type_array_item_transfer = GetString(Resource.String.command_type_array_item_transit);

            if (AbstractSettingsManage.Command != null)
            {
                switch (AbstractSettingsManage.Command.TypeCommand)
                {
                    case TypesCommands.Port:
                        command_type_array_item_port = GetString(Resource.String.command_type_array_item_port);
                        if (selected_type_command_name != command_type_array_item_port)
                        {
                            return;
                        }
                        break;
                    case TypesCommands.Controller:
                        if (selected_type_command_name != command_type_array_item_controller)
                        {
                            return;
                        }
                        break;
                    case TypesCommands.Exit:
                        if (selected_type_command_name != command_type_array_item_transfer)
                        {
                            return;
                        }
                        break;
                }
            }

            Log.Debug(TAG, $"TypesCommand_ItemSelected({selected_type_command_name}) - Position:{Position}");
            CommandConfigForm.RemoveAllViews();

            if (selected_type_command_name == command_type_array_item_port)
            {
                SelectedTypeCommand = TypesCommands.Port;
                about_selected_command.Text = GetString(Resource.String.about_selected_command_port_title);

                CommandConfigFormBody = LayoutInflater.Inflate(Resource.Layout.FormPortCommand, CommandConfigForm, false) as LinearLayoutCompat;

                AppCompatSpinner Controllers = CommandConfigFormBody.FindViewById<AppCompatSpinner>(Resource.Id.spinnerControllerPortCommand);
                AppCompatSpinner Ports = CommandConfigFormBody.FindViewById<AppCompatSpinner>(Resource.Id.spinnerPortCommand);
                AppCompatSpinner States = CommandConfigFormBody.FindViewById<AppCompatSpinner>(Resource.Id.spinnerStateCommand);

                SettingsManageKit = new SettingsManagePort(this, Controllers, Ports, States);
            }
            else if (selected_type_command_name == command_type_array_item_controller)
            {
                SelectedTypeCommand = TypesCommands.Controller;
                about_selected_command.Text = GetString(Resource.String.about_selected_command_controller_title);

                CommandConfigFormBody = LayoutInflater.Inflate(Resource.Layout.FormControllerCommand, CommandConfigForm, false) as LinearLayoutCompat;

                AppCompatSpinner Controllers = CommandConfigFormBody.FindViewById<AppCompatSpinner>(Resource.Id.spinnerControllerCommand);
                EditText CommandText = CommandConfigFormBody.FindViewById<EditText>(Resource.Id.editTextParametrControllerCommand);

                SettingsManageKit = new SettingsManageController(this, Controllers, CommandText);
            }
            else
            {
                SelectedTypeCommand = TypesCommands.Exit;
                about_selected_command.Text = GetString(Resource.String.about_selected_command_transit_title);

                CommandConfigFormBody = LayoutInflater.Inflate(Resource.Layout.FormTransferCommand, CommandConfigForm, false) as LinearLayoutCompat;

                AppCompatSpinner Scriptes = CommandConfigFormBody.FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandScript);
                AppCompatSpinner Steps = CommandConfigFormBody.FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandStepScript);

                SettingsManageKit = new SettingsManageTransfer(this, Scriptes, Steps);
            }

            CommandConfigForm.AddView(CommandConfigFormBody);
        }

        private void RequiredCondition_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Log.Debug(TAG, $"RequiredCondition_CheckedChange - {e.IsChecked}");

            if (e.IsChecked)
            {
                HardwareCondition.Enabled = true;
                PortCondition.Enabled = true;
                StateCondition.Enabled = true;
            }
            else
            {
                HardwareCondition.Enabled = false;
                PortCondition.Enabled = false;
                StateCondition.Enabled = false;
            }
        }

        protected CommandModel ReadFormToObject(CommandModel my_command)
        {
            Log.Debug(TAG, "ReadFormToObject");
            my_command.TypeCommand = SelectedTypeCommand;
            my_command.Hidden = HiddenCommandCheckBox.Checked;
            my_command.PauseBeforeExecution = double.Parse($"0{PauseSecondsBeforeStarting.Text}");
            my_command.Execution = command_executer_id;
            my_command.ExecutionParametr = command_executer_parameter?.ToString();

            if (RequiredCondition.Checked)
            {
                string PortExecutionConditionAllowingState;
                my_command.PortExecutionConditionId = Ports.Keys.ElementAt(PortCondition.SelectedItemPosition);

                PortExecutionConditionAllowingState = Resources.GetStringArray(Resource.Array.required_condition_port_states_array)[StateCondition.SelectedItemPosition];
                if (PortExecutionConditionAllowingState == GetString(Resource.String.abc_capital_on))
                {
                    my_command.PortExecutionConditionAllowingState = true;
                }
                else if (PortExecutionConditionAllowingState == GetString(Resource.String.abc_capital_off))
                {
                    my_command.PortExecutionConditionAllowingState = false;
                }
                else
                {
                    my_command.PortExecutionConditionAllowingState = null;
                }
            }

            switch (SelectedTypeCommand)
            {
                case TypesCommands.Port:
                    bool? setter_state_port = command_executer_parameter as bool?;
                    string setter_name_state_port = string.Empty;

                    if (setter_state_port == true)
                    {
                        setter_name_state_port = GetString(Resource.String.abc_capital_on);
                    }
                    else if (setter_state_port == false)
                    {
                        setter_name_state_port = GetString(Resource.String.abc_capital_off);
                    }
                    else
                    {
                        setter_name_state_port = GetString(Resource.String.abc_capital_switch);
                    }

                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            PortModel port = db.Ports.Include(x => x.Hardware).FirstOrDefault(x => x.Id == command_executer_id);
                            my_command.Name = $"{port.Hardware} ●> {port} ●> {setter_name_state_port}";
                        }
                    }
                    break;
                case TypesCommands.Controller:
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            HardwareModel hw = db.Hardwares.FirstOrDefault(x => x.Id == command_executer_id);
                            my_command.Name = $"{hw} ●> {command_executer_parameter}";
                        }
                    }
                    break;
                case TypesCommands.Exit:
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            CommandModel cmd = db.Commands.Include(x => x.Script).FirstOrDefault(x => x.Id == command_executer_id);
                            my_command.Name = $"{cmd.Script} ●> {cmd}";
                        }
                    }
                    break;
            }

            return my_command;
        }

        protected override void ButtonOk_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "ButtonOk_Click");
            string errMsg = ReadView(0);
            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Log.Error(TAG, $"ButtonOk_Click - {errMsg}");
                CardSubtitle.Text = errMsg;
                CardSubtitle.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return;
            }

            CommandModel command = ReadFormToObject(new CommandModel() { ScriptId = ScriptId });

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (db.Commands.Any(x => x.ScriptId == ScriptId))
                    {
                        command.Ordering = db.Commands.Where(x => x.ScriptId == ScriptId).Max(x => x.Ordering) + 1;
                    }
                    else
                    {
                        command.Ordering = 1;
                    }
                }
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    db.Commands.Add(command);
                    db.SaveChanges();
                }
            }

            Intent intent = new Intent(this, typeof(CommandsListActivity));
            intent.PutExtra(nameof(ScriptModel.Id), ScriptId);
            StartActivity(intent);
        }

        protected override string ReadView(int command_id)
        {
            string errMsg = string.Empty;
            switch (SelectedTypeCommand)
            {
                case TypesCommands.Port:
                    if (command_executer_id < 1)
                    {
                        errMsg = $"{GetString(Resource.String.error_controller_port_is_not_selected_title)}: [{command_executer_id}]";
                        break;
                    }
                    PortModel selected_port = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            selected_port = db.Ports.FirstOrDefault(x => x.Id == command_executer_id);
                        }
                    }
                    if (selected_port == null)
                    {
                        errMsg += $"{GetString(Resource.String.error_controller_port_was_not_found_in_the_database_title)}: [{command_executer_id}]";
                    }

                    break;
                case TypesCommands.Controller:
                    if (command_executer_id < 1)
                    {
                        errMsg = $"{GetString(Resource.String.error_no_controller_selected_title)}: [{command_executer_id}]";
                        break;
                    }
                    if (string.IsNullOrWhiteSpace(command_executer_parameter?.ToString()))
                    {
                        errMsg = GetString(Resource.String.error_not_specified_the_command_controller_title);
                        break;
                    }
                    HardwareModel hw = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            hw = db.Hardwares.FirstOrDefault(x => x.Id == command_executer_id);
                        }
                    }
                    if (hw == null)
                    {
                        errMsg += $"{GetString(Resource.String.err_selected_device_was_not_found_title)}: [{command_executer_id}]";
                    }
                    break;
                case TypesCommands.Exit:
                    if (command_executer_id < 1)
                    {
                        // выход из сценария
                        break;
                    }
                    CommandModel command = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            command = db.Commands.FirstOrDefault(x => x.Id == command_executer_id);
                        }
                    }
                    if (command == null)
                    {
                        errMsg += $"{GetString(Resource.String.error_not_specified_the_command_controller_title)}: [{command_executer_id}]";
                    }
                    break;
            }
            return errMsg.Trim();
        }
    }
}