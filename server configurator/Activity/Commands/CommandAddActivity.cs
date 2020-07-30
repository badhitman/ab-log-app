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
using System.Collections.Generic;
using System.Linq;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class CommandAddActivity : aSpecialScript
    {
        public static new readonly string TAG = nameof(ScriptAddActivity);
        protected int ScriptId;

        protected ISettingsManage SettingsManageKit;

        protected AppCompatEditText appCompatEditTextPauseBeforeStarting;
        protected AppCompatSpinner appCompatSpinnerTypesCommand;
        protected AppCompatCheckBox appCompatCheckBoxHiddenCommand;
        protected AppCompatTextView about_selected_command;

        LinearLayoutCompat linearLayoutTypeCommandForm;
        LinearLayoutCompat linearLayoutConfigCommandForm;
        private TypesCommands typeCommand;
        int command_executer_id;
        object command_executer_parameter;
        string type_command_name;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);
            ScriptId = Intent.Extras.GetInt(nameof(ScriptHardwareModel.Id), 0);

            CardHeader.Text = GetString(Resource.String.command_script_creating_title);
            CardSubHeader.Text = GetString(Resource.String.command_script_adding_subtitle);

            LinearLayoutCompat linearLayoutTopForm = LayoutInflater.Inflate(Resource.Layout.FormTopCommand, TopLayout, false) as LinearLayoutCompat;
            //
            appCompatEditTextPauseBeforeStarting = linearLayoutTopForm.FindViewById<AppCompatEditText>(Resource.Id.editTextPauseSecBeforeExecuting);
            appCompatCheckBoxHiddenCommand = linearLayoutTopForm.FindViewById<AppCompatCheckBox>(Resource.Id.checkBoxHiddenCommand);
            FormEnableTrigger.Text = GetString(Resource.String.command_script_requirement_title);
            TopLayout.AddView(linearLayoutTopForm);

            linearLayoutTypeCommandForm = LayoutInflater.Inflate(Resource.Layout.FormTypeCommand, BottomLayout, false) as LinearLayoutCompat;
            about_selected_command = linearLayoutTypeCommandForm.FindViewById<AppCompatTextView>(Resource.Id.about_selected_command);
            //
            appCompatSpinnerTypesCommand = linearLayoutTypeCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerTypeCommand);
            appCompatSpinnerTypesCommand.ItemSelected += TypesCommand_SpinnerItemSelected;
            BottomLayout.AddView(linearLayoutTypeCommandForm);

            ArrayAdapter adapterTypesCommands = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.commands_types_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterTypesCommands.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            appCompatSpinnerTypesCommand.Adapter = adapterTypesCommands;

            ArrayAdapter adapterPortStatusesCrop = ArrayAdapter<string>.CreateFromResource(this, Resource.Array.script_trigger_port_states_crop_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterPortStatusesCrop.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            StateFormFieldSpinner.Adapter = adapterPortStatusesCrop;
        }


        protected void TypesCommand_SpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            command_executer_id = -1;
            command_executer_parameter = null;
            BottomLayout.RemoveAllViews();
            BottomLayout.AddView(linearLayoutTypeCommandForm);

            int Position = appCompatSpinnerTypesCommand.SelectedItemPosition;

            type_command_name = appCompatSpinnerTypesCommand.GetItemAtPosition(Position).ToString();
            Log.Debug(TAG, $"TypesCommand_SpinnerItemSelected({type_command_name}) - Position:{Position}");

            if (type_command_name == GetString(Resource.String.command_type_array_item_port))
            {
                linearLayoutConfigCommandForm = LayoutInflater.Inflate(Resource.Layout.FormPortCommand, BottomLayout, false) as LinearLayoutCompat;
                AppCompatSpinner spinnerControllerCommand = linearLayoutConfigCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerControllerPortCommand);
                AppCompatSpinner spinnerPortsCommand = linearLayoutConfigCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerPortCommand);
                AppCompatSpinner spinnerStateCommand = linearLayoutConfigCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerStateCommand);

                SettingsManagePort settingsManage = new SettingsManagePort()
                {
                    controllers = spinnerControllerCommand,
                    ports = spinnerPortsCommand,
                    states = spinnerStateCommand,
                    CommandsPorts = new Dictionary<int, string>()
                };
                typeCommand = TypesCommands.Port;
                about_selected_command.Text = GetString(Resource.String.about_selected_command_port_title);

                spinnerControllerCommand.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
                {
                    int hardware_id = Hardwares.Keys.ElementAt(e.Position);
                    settingsManage.CommandsPorts = new Dictionary<int, string>();

                    Log.Debug(TAG, $"delegate update ports list ({hardware_id})");
                    HardwareModel hw = null;

                    if (hardware_id > 0)
                    {
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                hw = db.Hardwares.Include(x => x.Ports).FirstOrDefault(x => x.Id == hardware_id);
                            }
                        }
                    }
                    if (hw == null)
                    {
                        Log.Error(TAG, $"{GetString(Resource.String.err_selected_device_was_not_found_title)} - ControllerCommand_SpinnerItemSelected({hardware_id}) - Position:{e.Position}");
                        return;
                    }
                    else
                    {
                        Log.Debug(TAG, $"ControllerCommand_SpinnerItemSelected(hw:{hw}) - Position:{e.Position}");
                    }
                    hw.Ports.ForEach(portHardware => { settingsManage.CommandsPorts.Add(portHardware.Id, portHardware.ToString()); });
                    //
                    ArrayAdapter<string> adapterPorts = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, settingsManage.CommandsPorts.Values.ToList());
                    adapterPorts.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinnerPortsCommand.Adapter = adapterPorts;
                };

                spinnerPortsCommand.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
                {
                    int port_id = settingsManage.CommandsPorts.Keys.ElementAt(e.Position);
                    command_executer_id = port_id;

                    PortHardwareModel portHardware = null;
                    if (port_id > 0)
                    {
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                portHardware = db.PortsHardwares.FirstOrDefault(x => x.Id == port_id);
                            }
                        }
                    }
                    if (portHardware == null)
                    {
                        Log.Error(TAG, $"{GetString(Resource.String.err_selected_port_was_not_found_title)} - PortCommand_SpinnerItemSelected({port_id}) - Position:{e.Position}");
                    }
                    else
                    {
                        Log.Debug(TAG, $"PortCommand_SpinnerItemSelected(hw:{portHardware}) - Position:{e.Position}");
                    }

                };

                spinnerStateCommand.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
                {
                    string selected_state = Resources.GetStringArray(Resource.Array.script_trigger_port_states_array)[e.Position];
                    Log.Debug(TAG, $"StateCommand_SpinnerItemSelected(hw:{selected_state}) - Position:{e.Position}");
                    if (selected_state == GetString(Resource.String.abc_capital_on))
                    {
                        command_executer_parameter = true;
                    }
                    else if (selected_state == GetString(Resource.String.abc_capital_off))
                    {
                        command_executer_parameter = false;
                    }
                    else
                    {
                        command_executer_parameter = null;
                    }
                };

                spinnerStateCommand.Adapter = adapterPortStatuses;
                UpdateHardwaresListSpinner(spinnerControllerCommand);

                SettingsManageKit = settingsManage;
            }
            else if (type_command_name == GetString(Resource.String.command_type_array_item_controller))
            {
                typeCommand = TypesCommands.Controller;
                about_selected_command.Text = GetString(Resource.String.about_selected_command_controller_title);
                linearLayoutConfigCommandForm = LayoutInflater.Inflate(Resource.Layout.FormControllerCommand, BottomLayout, false) as LinearLayoutCompat;

                AppCompatSpinner controllers = linearLayoutConfigCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerControllerCommand);
                EditText command = linearLayoutConfigCommandForm.FindViewById<EditText>(Resource.Id.editTextParametrControllerCommand);

                controllers.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
                {
                    command_executer_id = Hardwares.Keys.ElementAt(e.Position);
                    HardwareModel hw = null;
                    if (command_executer_id > 0)
                    {
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                hw = db.Hardwares.Include(x => x.Ports).FirstOrDefault(x => x.Id == command_executer_id);
                            }
                        }
                    }
                    if (hw == null)
                    {
                        Log.Error(TAG, $"{GetString(Resource.String.err_selected_device_was_not_found_title)} - ControllerCommand_SpinnerItemSelected({command_executer_id}) - Position:{e.Position}");
                    }
                    else
                    {
                        Log.Debug(TAG, $"ControllerCommand_SpinnerItemSelected(hw:{hw}) - Position:{e.Position}");
                    }
                };

                command.TextChanged += delegate (object sender, Android.Text.TextChangedEventArgs e)
                {
                    command_executer_parameter = command.Text.Trim();
                };
                UpdateHardwaresListSpinner(controllers);

                SettingsManageKit = new SettingsManageController()
                {
                    controllers = controllers,
                    command = command
                };
            }
            else
            {
                typeCommand = TypesCommands.Exit;
                about_selected_command.Text = GetString(Resource.String.about_selected_command_transit_title);
                Dictionary<int, string> Scripts = new Dictionary<int, string>();
                Dictionary<int, string> StepsScripts = new Dictionary<int, string>();
                Scripts.Add(0, GetString(Resource.String.completing_the_script_title));

                linearLayoutConfigCommandForm = LayoutInflater.Inflate(Resource.Layout.FormTransferCommand, BottomLayout, false) as LinearLayoutCompat;
                AppCompatSpinner scriptes = linearLayoutConfigCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandScript);
                AppCompatSpinner steps = linearLayoutConfigCommandForm.FindViewById<AppCompatSpinner>(Resource.Id.spinnerCommandStepScript);

                SettingsManageTransfer settingsManageTransfer = new SettingsManageTransfer()
                {
                    scriptes = scriptes,
                    steps = steps
                };

                scriptes.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
                {
                    int script_id = Scripts.Keys.ElementAt(e.Position);
                    StepsScripts = new Dictionary<int, string>();
                    ScriptHardwareModel scriptHardware = null;
                    if (script_id > 0)
                    {
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                scriptHardware = db.ScriptsHardware.Include(x => x.CommandsScripts).FirstOrDefault(x => x.Id == script_id);
                            }
                        }
                        if (scriptHardware != null)
                        {
                            Log.Debug(TAG, $"ScriptCommand_SpinnerItemSelected(id:{script_id}) - {scriptHardware}");
                            scriptHardware.CommandsScripts.ForEach(x => { StepsScripts.Add(x.Id, x.ToString()); });
                        }

                        ArrayAdapter<string> adapterSteps = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, StepsScripts.Values.ToList());
                        adapterSteps.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                        steps.Adapter = adapterSteps;
                    }
                };

                steps.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
                {
                    command_executer_id = StepsScripts.Keys.ElementAt(e.Position);
                };

                List<ScriptHardwareModel> myScriptes = new List<ScriptHardwareModel>();
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        myScriptes = db.ScriptsHardware.ToList();
                    }
                }
                myScriptes.ForEach(x =>
                {
                    Scripts.Add(x.Id, $"{(x.Id == ScriptId ? "→ " : " ")}{x}");
                });

                settingsManageTransfer.ScriptsList = Scripts;
                settingsManageTransfer.StepsScriptsList = StepsScripts;
                SettingsManageKit = settingsManageTransfer;

                ArrayAdapter<string> adapterScriptes = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Scripts.Values.ToList());
                adapterScriptes.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                scriptes.Adapter = adapterScriptes;
                int index_current_script = Scripts.Keys.ToList().IndexOf(ScriptId);
                scriptes.SetSelection(index_current_script);
            }

            BottomLayout.AddView(linearLayoutConfigCommandForm);
        }

        protected bool PreCardButton_Click(object sender)
        {
            AppCompatButton button = (AppCompatButton)sender;
            Log.Debug(TAG, $"CardButton_Click: {button.Text}");
            Intent CancelIntent;
            if (button.Id == CardButtonCancel.Id)
            {
                CancelIntent = new Intent(this, typeof(CommandsListActivity));
                CancelIntent.PutExtra(nameof(CommandScriptModel.Id), ScriptId);
                StartActivity(CancelIntent);
                return false;
            }

            string errMsg = ReadView();

            if (!string.IsNullOrWhiteSpace(errMsg))
            {
                Log.Error(TAG, $"CardButton_Click - {errMsg}");
                CardSubHeader.Text = errMsg;
                CardSubHeader.SetTextColor(Android.Graphics.Color.Red);
                Toast.MakeText(this, errMsg, ToastLength.Short).Show();
                return false;
            }
            return true;
        }

        protected CommandScriptModel ReadFormToObject(CommandScriptModel my_command)
        {
            Log.Debug(TAG, "ReadFormToObject");
            my_command.TypeCommand = typeCommand;
            my_command.Hidden = appCompatCheckBoxHiddenCommand.Checked;
            my_command.PauseBeforeExecution = double.Parse($"0{appCompatEditTextPauseBeforeStarting.Text}");
            my_command.Execution = command_executer_id;
            my_command.ExecutionParametr = command_executer_parameter?.ToString();

            string PortExecutionConditionAllowingState;
            if (FormEnableTrigger.Checked)
            {
                my_command.PortExecutionConditionId = Ports.Keys.ElementAt(PortFormFieldSpinner.SelectedItemPosition);

                PortExecutionConditionAllowingState = Resources.GetStringArray(Resource.Array.script_trigger_port_states_array)[StateFormFieldSpinner.SelectedItemPosition];
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
            //my_command.;
            //my_command.;
            //my_command.;
            //my_command.;
            //my_command.;
            //my_command.;
            switch (typeCommand)
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
                            PortHardwareModel port = db.PortsHardwares.Include(x => x.Hardware).FirstOrDefault(x => x.Id == command_executer_id);
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
                            CommandScriptModel cmd = db.CommandsScript.Include(x => x.ScriptHardware).FirstOrDefault(x => x.Id == command_executer_id);
                            my_command.Name = $"{cmd.ScriptHardware} ●> {cmd}";
                        }
                    }
                    break;
            }

            return my_command;
        }

        protected override void CardButton_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "CardButton_Click");
            if (!PreCardButton_Click(sender))
            {
                return;
            }

            CommandScriptModel command = ReadFormToObject(new CommandScriptModel() { ScriptHardwareId = ScriptId });

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    if (db.CommandsScript.Any(x => x.ScriptHardwareId == ScriptId))
                    {
                        command.Ordering = db.CommandsScript.Where(x => x.ScriptHardwareId == ScriptId).Max(x => x.Ordering) + 1;
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
                    db.CommandsScript.Add(command);
                    db.SaveChanges();
                }
            }

            Intent intent = new Intent(this, typeof(CommandsListActivity));
            intent.PutExtra(nameof(ScriptHardwareModel.Id), ScriptId);
            StartActivity(intent);
        }

        protected override string ReadView()
        {
            string errMsg = string.Empty;
            switch (typeCommand)
            {
                case TypesCommands.Port:
                    if (command_executer_id < 1)
                    {
                        errMsg = $"{GetString(Resource.String.error_controller_port_is_not_selected_title)}: [{command_executer_id}]";
                        break;
                    }
                    PortHardwareModel selected_port = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            selected_port = db.PortsHardwares.FirstOrDefault(x => x.Id == command_executer_id);
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
                    CommandScriptModel command = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            command = db.CommandsScript.FirstOrDefault(x => x.Id == command_executer_id);
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