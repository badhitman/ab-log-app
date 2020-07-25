////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class ScriptListItemViewHolder : RecyclerView.ViewHolder
    {
        public TextView Name { get; private set; }

        public ScriptListItemViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.script_name_list_item);
            itemView.Click += (sender, e) => listener(LayoutPosition);
        }
    }
}