////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ab.Services
{
    public class HardwareListItemViewHolder : AbstractCommunicationViewHolder
    {
        public static new readonly string TAG = "● hardware-list-item-view-holder";

        public TextView Address { get; private set; }

        protected override int NameId => Resource.Id.hardware_name_list_item;
        protected override int AlarmId => Resource.Id.hardware_alarm_subscriber_list_item;
        protected override int CommandId => Resource.Id.hardware_commands_allowed_list_item;

        public HardwareListItemViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
        {
            Log.Debug(TAG, "~ constructor");

            Address = itemView.FindViewById<TextView>(Resource.Id.hardware_address_list_item);
        }
    }
}