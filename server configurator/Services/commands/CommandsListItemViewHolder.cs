////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class CommandsListItemViewHolder : RecyclerView.ViewHolder
    {
        public static readonly string TAG = "● commands-list-item-view-holder";

        public int CommandId { get; set; }
        //
        public TextView Name { get; private set; }
        public TextView Info { get; private set; }
        public TextView Info2 { get; private set; }
        public TextView Info3 { get; private set; }
        public TextView Info4 { get; private set; }

        public ImageButton buttonUp { get; private set; }
        public ImageButton buttonDown { get; private set; }

        public CommandsListItemViewHolder(View itemView, Action<int> OnClickItemView, Action<object, int> OnMoveOrderingCommand) : base(itemView)
        {
            Name = itemView.FindViewById<TextView>(Resource.Id.command_name_list_item);
            Info = itemView.FindViewById<TextView>(Resource.Id.command_info_list_item);
            Info2 = itemView.FindViewById<TextView>(Resource.Id.command_info2_list_item);
            Info3 = itemView.FindViewById<TextView>(Resource.Id.command_info3_list_item);
            Info4 = itemView.FindViewById<TextView>(Resource.Id.command_info4_list_item);

            buttonUp = itemView.FindViewById<ImageButton>(Resource.Id.UpCommandOrdering);
            buttonUp.Click += (object sender, EventArgs e) => OnMoveOrderingCommand(sender, CommandId);
            //
            buttonDown = itemView.FindViewById<ImageButton>(Resource.Id.DownCommandOrdering);
            buttonDown.Click += (object sender, EventArgs e) => OnMoveOrderingCommand(sender, CommandId);

            itemView.Click += (object sender, EventArgs e) => OnClickItemView(CommandId);
        }
    }
}