﻿////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System.Linq;

namespace ab.Services
{
    public class TelegramUserListItemViewHolder : RecyclerView.ViewHolder
    {
        public static readonly string TAG = "● telegram-user-list-item-view-holder";

        public TextView TelegramFirstSecondName { get; private set; }
        public TextView TelegramId { get; private set; }
        public AppCompatSpinner LinkedUserSpinner { get; private set; }

        public TelegramUserListItemViewHolder(View itemView) : base(itemView)
        {
            Log.Debug(TAG, "~ constructor");

            TelegramFirstSecondName = itemView.FindViewById<TextView>(Resource.Id.textViewTelegramFirstSecondName);
            TelegramId = itemView.FindViewById<TextView>(Resource.Id.textViewTelegramId);
            LinkedUserSpinner = itemView.FindViewById<AppCompatSpinner>(Resource.Id.spinnerLinkedUser);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(itemView.Context, Android.Resource.Layout.SimpleSpinnerItem, TelegramUsersListAdapter.LinkedUsers.Values.ToList());
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            LinkedUserSpinner.Adapter = adapter;
        }
    }
}