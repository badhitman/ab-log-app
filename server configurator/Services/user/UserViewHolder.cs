////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Views;
using Android.Widget;

namespace ab.Services
{
    public class UserViewHolder : aViewHolder
    {
        public TextView Email { get; private set; }
        public TextView Phone { get; private set; }
        public TextView TelegramId { get; private set; }

        protected override int _nameId => Resource.Id.user_list_item;
        protected override int _alarmId => Resource.Id.user_list_item_alarm_subscribed;
        protected override int _commandId => Resource.Id.user_list_item_command_allowed;

        public UserViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
        {
            Email = itemView.FindViewById<TextView>(Resource.Id.user_list_item_email);
            Phone = itemView.FindViewById<TextView>(Resource.Id.user_list_item_phone);
            TelegramId = itemView.FindViewById<TextView>(Resource.Id.user_list_item_telegram);
        }
    }
}