////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class ScriptListItemViewHolder : RecyclerView.ViewHolder
    {
        public static readonly string TAG = "● script-list-item-view-holder";

        public TextView Name { get; private set; }
        public int ScriptId { get; set; }

        public ScriptListItemViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Log.Debug(TAG,"~ constructor");

            Name = itemView.FindViewById<TextView>(Resource.Id.script_name_list_item);
            itemView.Click += (sender, e) => listener(ScriptId);
        }
    }
}