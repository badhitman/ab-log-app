////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace ab.Services
{
    public class CommandsListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> MoveOrderingCommand;

        Context mContext;
        int ScriptId;

        public override int ItemCount
        {
            get
            {
                lock (DatabaseContext.DbLocker)
                {
                    using DatabaseContext db = new DatabaseContext(gs.DatabasePathBase);
                    return db.CommandsScript.Where(x => x.ScriptHardwareId == ScriptId).Count();
                }
            }
        }

        public CommandsListAdapter(Context _mContext, int script_id)
        {
            mContext = _mContext;
            ScriptId = script_id;
        }

        void OnClick(int command_id)
        {
            ItemClick?.Invoke(this, command_id);
        }

        void OnMoveOrderingCommand(object sender, int command_id)
        {
            MoveOrderingCommand?.Invoke(sender, command_id);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            CommandsListItemViewHolder commandListItemViewHolder = holder as CommandsListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    CommandScriptModel command = db.CommandsScript.Where(x => x.ScriptHardwareId == ScriptId).OrderBy(x => x.Ordering).Skip(position).FirstOrDefault();
                    commandListItemViewHolder.CommandId = command.Id;
                    //
                    commandListItemViewHolder.Name.Text = command.Name;
                    commandListItemViewHolder.Info.Text = $"{command.TypeCommand}";
                    commandListItemViewHolder.Info2.Text = $"{(command.Hidden ? "[hide] " : "")}";
                    commandListItemViewHolder.Info3.Text = $"{(command.PauseBeforeExecution > 0 ? $"[p:{command.PauseBeforeExecution}s] " : "")}";
                    commandListItemViewHolder.Info4.Text = $"{(command.PortExecutionConditionId > 0 ? $"[req] " : "")}";
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.commands_list_item, parent, false);
            CommandsListItemViewHolder commandListItemViewHolder = new CommandsListItemViewHolder(itemView, OnClick, OnMoveOrderingCommand);
            return commandListItemViewHolder;
        }
    }
}