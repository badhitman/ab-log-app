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
        protected abstract int NameId { get; }
        protected abstract int AlarmId { get; }
        protected abstract int CommandId { get; }

        public TextView Name { get; protected set; }
        //
        public TextView AlarmSubscriber { get; protected set; }
        public TextView CommandsAllowed { get; protected set; }

        protected aViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Name = itemView.FindViewById<TextView>(NameId);

            AlarmSubscriber = itemView.FindViewById<TextView>(AlarmId);
            CommandsAllowed = itemView.FindViewById<TextView>(CommandId);

            itemView.Click += (sender, e) => listener(LayoutPosition);
        }
    }
}