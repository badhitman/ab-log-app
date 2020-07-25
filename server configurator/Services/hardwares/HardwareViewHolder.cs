////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Views;
using Android.Widget;

namespace ab.Services
{
    public class HardwareViewHolder : aViewHolder
    {
        public TextView Address { get; private set; }

        protected override int NameId => Resource.Id.hardware_name_list_item;
        protected override int AlarmId => Resource.Id.hardware_alarm_subscriber_list_item;
        protected override int CommandId => Resource.Id.hardware_commands_allowed_list_item;

        public HardwareViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
        {
            Address = itemView.FindViewById<TextView>(Resource.Id.hardware_address_list_item);
        }
    }
}