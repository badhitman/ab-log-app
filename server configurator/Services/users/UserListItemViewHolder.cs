////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ab.Services
{
    public class UserListItemViewHolder : AbstractCommunicationViewHolder
    {
        public new readonly string TAG = "users-list-adapter";

        //public TextView Email { get; private set; }
        //public TextView Phone { get; private set; }

        protected override int NameId => Resource.Id.user_list_item;
        protected override int AlarmId => Resource.Id.user_list_item_alarm_subscribed;
        protected override int CommandId => Resource.Id.user_list_item_command_allowed;

        public UserListItemViewHolder(View itemView, Action<int> listener) : base(itemView, listener)
        {
            Log.Debug(TAG, "~ constructor");
            //Email = itemView.FindViewById<TextView>(Resource.Id.user_list_item_email);
            //Phone = itemView.FindViewById<TextView>(Resource.Id.user_list_item_phone);
        }
    }
}