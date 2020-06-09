////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;

namespace ab.Services
{
    public abstract class aViewHolder : RecyclerView.ViewHolder
    {
        protected abstract int _nameId { get; }
        protected abstract int _alarmId { get; }
        protected abstract int _commandId { get; }

        public TextView Name { get; protected set; }
        //
        public TextView AlarmSubscriber { get; protected set; }
        public TextView CommandsAllowed { get; protected set; }

        public aViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Name = itemView.FindViewById<TextView>(_nameId);

            AlarmSubscriber = itemView.FindViewById<TextView>(_alarmId);
            CommandsAllowed = itemView.FindViewById<TextView>(_commandId);

            itemView.Click += (sender, e) => listener(LayoutPosition);
        }
    }
}