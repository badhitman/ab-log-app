////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using System;
using System.Linq;
using ab.Model;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.EntityFrameworkCore;

namespace ab.Services
{
    public class ScriptsListAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        Context mContext;

        public override int ItemCount { get { lock (DatabaseContext.DbLocker) { using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase)) { return db.ScriptsHardware.Count(); } } } }

        public ScriptsListAdapter(Context _mContext)
        {
            mContext = _mContext;
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ScriptListItemViewHolder scriptListItemViewHolder = holder as ScriptListItemViewHolder;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    ScriptHardwareModel script = db.ScriptsHardware.Skip(position).Include(x => x.TriggerPort).ThenInclude(x => x.Hardware).FirstOrDefault();
                    scriptListItemViewHolder.Name.Text = script.Name;
                    if (script.TriggerPort != null)
                    {
                        scriptListItemViewHolder.Name.Text += $" (tg: {script.TriggerPort.Hardware.Name} > {script.TriggerPort} > {(script.TriggerPortState == true ? "ON" : (script.TriggerPortState == false ? "OFF" : "Switch"))})";
                    }
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the photo:
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.scripts_list_item, parent, false);

            // Create a ViewHolder to hold view references inside the CardView:
            ScriptListItemViewHolder scriptListItemViewHolder = new ScriptListItemViewHolder(itemView, OnClick);
            return scriptListItemViewHolder;
        }
    }
}