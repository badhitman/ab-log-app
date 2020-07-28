////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System.Collections.Generic;

namespace ab
{
    public interface ISettingsManage
    {

    }

    public class SettingsManagePort : ISettingsManage
    {
        public AppCompatSpinner controllers { get; set; }
        public AppCompatSpinner ports { get; set; }
        public AppCompatSpinner states { get; set; }
        public Dictionary<int, string> CommandsPorts { get; set; }
    }

    public class SettingsManageController : ISettingsManage
    {
        public AppCompatSpinner controllers { get; set; }
        public EditText command { get; set; }
    }

    public class SettingsManageTransfer : ISettingsManage
    {
        public Dictionary<int, string> StepsScriptsList { get; set; }
        public AppCompatSpinner scriptes { get; set; }
        public AppCompatSpinner steps { get; set; }
        public Dictionary<int, string> ScriptsList { get; set; }
    }
}