////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;

namespace ab
{
    public abstract class AbstractSettingsManage
    {
        public static readonly string TAG = "● abstract-settings-manage";
        //
        protected CommandAddActivity ParentActivity;

        public static CommandModel Command { get; internal set; }

        public abstract void OnPause();
        public abstract void OnResume();
    }
}