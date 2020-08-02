////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using ab.Model;
using ab.Services;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.Widget;

namespace ab
{
    public abstract class aSpecialScript : AbstractActivity
    {
        public static new readonly string TAG = "● abstract-special-script";

        public Dictionary<int, string> Hardwares;
        public Dictionary<int, string> Ports;

        protected AppCompatTextView CardTitle;
        protected AppCompatTextView CardSubtitle;

        protected LinearLayoutCompat FooterLayout;

        protected AppCompatButton ButtonOk;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);

            Hardwares = new Dictionary<int, string>();

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    foreach (HardwareModel hw in db.Hardwares)
                    {
                        Hardwares.Add(hw.Id, hw.ToString());
                    }
                }
            }
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();

            ButtonOk.Click += ButtonOk_Click;
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            base.OnPause();

            ButtonOk.Click -= ButtonOk_Click;
        }

        public void HardwaresListSpinnerLoad(ref AppCompatSpinner appCompatSpinnerHardwares, int selected_hardware_id = 0)
        {
            Log.Debug(TAG, $"HardwaresListSpinnerLoad(selected_hardware_id={selected_hardware_id})");

            ArrayAdapter<string> adapterHardwares = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Hardwares.Values.ToList());
            adapterHardwares.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            appCompatSpinnerHardwares.Adapter = adapterHardwares;
            if (selected_hardware_id > 0)
            {
                appCompatSpinnerHardwares.SetSelection(Hardwares.Keys.ToList().IndexOf(selected_hardware_id));
            }
        }

        protected void PortsList_UpdateSpinner(int hardware_id, ref AppCompatSpinner portsSpinner, int selected_port_id = 0)
        {
            Log.Debug(TAG, $"PortsList_UpdateSpinner(hardware_id={hardware_id})");

            if (portsSpinner.SelectedItemPosition > -1 && selected_port_id == 0 && Ports?.Count > 0)
            {
                selected_port_id = Ports.Keys.ToList()[portsSpinner.SelectedItemPosition];
            }

            Ports = new Dictionary<int, string>();
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    foreach (PortModel portHardware in db.Ports.Where(x => x.HardwareId == hardware_id))
                    {
                        Ports.Add(portHardware.Id, portHardware.ToString());
                    }
                }
            }

            ArrayAdapter<string> adapterPorts = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, Ports.Values.ToList());
            adapterPorts.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            portsSpinner.Adapter = adapterPorts;

            if (selected_port_id > 0)
            {
                portsSpinner.SetSelection(Ports.Keys.ToList().IndexOf(selected_port_id));
            }
        }

        protected int GetIndexPortState(bool? port_state_as_bool, List<string> port_states_list)
        {
            if (port_state_as_bool == true)
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_on));
            }
            else if (port_state_as_bool == false)
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_off));
            }
            else
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_switch));
            }
        }

        protected int GetIndexPortState(string port_state_as_string)
        {
            Log.Debug(TAG, $"GetIndexPortState({port_state_as_string})");

            List<string> port_states_list = new List<string>(Resources.GetStringArray(Resource.Array.script_trigger_port_states_array));
            if (string.IsNullOrWhiteSpace(port_state_as_string) || !bool.TryParse(port_state_as_string, out bool port_state))
            {
                return port_states_list.IndexOf(GetString(Resource.String.abc_capital_switch));
            }

            return GetIndexPortState(port_state, port_states_list);
        }

        protected abstract void ButtonOk_Click(object sender, EventArgs e);
        protected abstract string ReadView(int scriptId = 0);
    }
}