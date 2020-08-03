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
    public class SettingsManagePort : AbstractSettingsManage
    {
        public static new readonly string TAG = "● settings-manage-port";

        public Dictionary<int, string> PortsList { get; private set; } = new Dictionary<int, string>();

        public AppCompatSpinner Controllers { get; private set; }
        public AppCompatSpinner Ports { get; private set; }
        public AppCompatSpinner States { get; private set; }

        public SettingsManagePort(CommandAddActivity command_add_activity, AppCompatSpinner controllers, AppCompatSpinner ports, AppCompatSpinner states)
        {
            Log.Debug(TAG, "~ constructor");

            command_add_activity.HardwaresListSpinnerLoad(ref controllers);
            ParentActivity = command_add_activity;

            Controllers = controllers;
            Ports = ports;
            States = states;

            string[] script_trigger_port_states_array = ParentActivity.Resources.GetStringArray(Resource.Array.script_trigger_port_states_array);
            ArrayAdapter adapterPortStatuses = new ArrayAdapter<string>(ParentActivity, Android.Resource.Layout.SimpleSpinnerItem, script_trigger_port_states_array);
            adapterPortStatuses.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            States.Adapter = adapterPortStatuses;

            if (Command != null)
            {
                if(Command.ExecutionParametr == "true")
                {
                    States.SetSelection(Array.IndexOf(script_trigger_port_states_array, ParentActivity.GetString(Resource.String.abc_capital_on)));
                }
                else if(Command.ExecutionParametr == "false")
                {
                    States.SetSelection(Array.IndexOf(script_trigger_port_states_array, ParentActivity.GetString(Resource.String.abc_capital_off)));
                }
                else
                {
                    States.SetSelection(Array.IndexOf(script_trigger_port_states_array, ParentActivity.GetString(Resource.String.abc_capital_switch)));
                }
            }

            OnResume();
        }

        public void ControllerSelect(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int hardware_id = ParentActivity.Hardwares.Keys.ElementAt(e.Position);
            if (Command != null)
            {
                PortModel port = null;
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        port = db.Ports.FirstOrDefault(x => x.Id == Command.Execution);
                    }
                }
                if (port.HardwareId != hardware_id)
                {
                    Controllers.SetSelection(ParentActivity.Hardwares.Keys.ToList().IndexOf(port.HardwareId));
                    return;
                }
            }

            PortsList = new Dictionary<int, string>();

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
                Log.Error(TAG, $"{ParentActivity.GetString(Resource.String.err_selected_device_was_not_found_title)}; ControllerSelect(); hardware_id:{hardware_id}; Position:{e.Position};");
                return;
            }
            else
            {
                Log.Debug(TAG, $"ControllerSelect(); for command; hw:{hw}; Position:{e.Position};");
            }
            hw.Ports.ForEach(portHardware => { PortsList.Add(portHardware.Id, portHardware.ToString()); });
            //
            ArrayAdapter<string> adapterPorts = new ArrayAdapter<string>(ParentActivity, Android.Resource.Layout.SimpleSpinnerItem, PortsList.Values.ToList());
            adapterPorts.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            Ports.Adapter = adapterPorts;
        }

        public void PortSelect(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int port_id = PortsList.Keys.ElementAt(e.Position);

            if (Command?.Execution != port_id)
            {
                Ports.SetSelection(PortsList.Keys.ToList().IndexOf(Command.Execution));
                return;
            }
            Command = null;

            ParentActivity.command_executer_id = port_id;

            PortModel port = null;
            if (ParentActivity.command_executer_id > 0)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        port = db.Ports.FirstOrDefault(x => x.Id == ParentActivity.command_executer_id);
                    }
                }
            }
            if (port == null)
            {
                Log.Error(TAG, $"PortSelect(); for command; portId:{ParentActivity.command_executer_id}; Position:{e.Position};");
            }
            else
            {
                Log.Debug(TAG, $"PortSelect(); for command; hw:{port}; portId:{ParentActivity.command_executer_id}; Position:{e.Position}");
            }
        }

        public void StateSelect(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string selected_state = ParentActivity.Resources.GetStringArray(Resource.Array.script_trigger_port_states_array)[e.Position];
            Log.Debug(TAG, $"StateSelect(); for command; selected_state:{selected_state}; Position:{e.Position};");

            if (selected_state == ParentActivity.GetString(Resource.String.abc_capital_on))
            {
                ParentActivity.command_executer_parameter = true;
            }
            else if (selected_state == ParentActivity.GetString(Resource.String.abc_capital_off))
            {
                ParentActivity.command_executer_parameter = false;
            }
            else
            {
                ParentActivity.command_executer_parameter = null;
            }
        }

        public override void OnResume()
        {
            Log.Debug(TAG, "OnResume");

            Controllers.ItemSelected += ControllerSelect;
            Ports.ItemSelected += PortSelect;
            States.ItemSelected += StateSelect;
        }

        public override void OnPause()
        {
            Log.Debug(TAG, "OnPause");

            Controllers.ItemSelected -= ControllerSelect;
            Ports.ItemSelected -= PortSelect;
            States.ItemSelected -= StateSelect;
        }
    }
}