////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;

namespace ab
{
    public abstract class AbstractSettingsManage
    {
        public static readonly string TAG = "● abstract-settings-manage";

        public static CommandModel Command { get; internal set; }
        //
        protected CommandAddActivity ParentActivity;
        public abstract void OnPause();
        public abstract void OnResume();
    }
}