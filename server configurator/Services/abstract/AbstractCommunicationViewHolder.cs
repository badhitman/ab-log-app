////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;

namespace ab.Services
{
    public abstract class AbstractCommunicationViewHolder : RecyclerView.ViewHolder
    {
        public static readonly string TAG = "● abstract-communication-view-holder";

        protected abstract int NameId { get; }
        protected abstract int AlarmId { get; }
        protected abstract int CommandId { get; }

        public TextView Name { get; protected set; }
        //
        public TextView AlarmSubscriber { get; protected set; }
        public TextView CommandsAllowed { get; protected set; }

        public int ObjectId { get; set; }

        protected AbstractCommunicationViewHolder(View ItemView, Action<int> onClickItemListener) : base(ItemView)
        {
            Log.Debug(TAG, "~ constructor");

            Name = ItemView.FindViewById<TextView>(NameId);

            AlarmSubscriber = ItemView.FindViewById<TextView>(AlarmId);
            CommandsAllowed = ItemView.FindViewById<TextView>(CommandId);

            ItemView.Click += (sender, e) =>
            {
                Log.Debug(TAG, $"ItemView.Click - ObjectId:{ObjectId}");
                onClickItemListener(ObjectId);
            };
        }
    }
}