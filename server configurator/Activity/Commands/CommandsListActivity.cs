////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class CommandsListActivity : aListActivity
    {
        protected override int ViewId => Resource.Layout.commands_list_activity;
        protected override int ToolbarId => Resource.Id.commands_list_toolbar;
        protected override int ButtonAdd => Resource.Id.commands_list_add_button;
        protected override int DrawerLayoutId => Resource.Id.commands_list_app_drawer_layout;
        protected override int NavId => Resource.Id.commands_list_app_nav_view;

        protected TextView commands_list_card_sub_header;
        ScriptHardwareModel script;

        RecyclerView mRecyclerView;
        RecyclerView.LayoutManager mLayoutManager;
        CommandsListAdapter mAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            commands_list_card_sub_header = FindViewById<TextView>(Resource.Id.commands_list_card_sub_header);
            int scriptHardwareId = Intent.Extras.GetInt(nameof(ScriptHardwareModel.Id), 0);
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    script = db.ScriptsHardware.Include(x => x.CommandsScripts).FirstOrDefault(x => x.Id == scriptHardwareId);
                }
            }
            commands_list_card_sub_header.Text = $"[{script.Name}]";

            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewCommandsList);


        }

        protected override void OnResume()
        {
            base.OnResume();
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mAdapter = new CommandsListAdapter(this, script.Id);

            mRecyclerView.SetAdapter(mAdapter);
            mAdapter.ItemClick += OnItemClick;
            mAdapter.MoveOrderingCommand += MoveOrderingCommand;
        }

        private void MoveOrderingCommand(object sender, int command_id)
        {
            ImageButton button = (ImageButton)sender;
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    CommandScriptModel command = db.CommandsScript.Find(command_id);
                    if (button.Id == Resource.Id.UpCommandOrdering)
                    {
                        if (db.CommandsScript.Any(x => x.ScriptHardwareId == command.ScriptHardwareId && x.Ordering < command.Ordering))
                        {
                            int exchange_order = db.CommandsScript.Where(x => x.ScriptHardwareId == command.ScriptHardwareId && x.Ordering < command.Ordering).Max(x=>x.Ordering);
                            CommandScriptModel command2 = db.CommandsScript.FirstOrDefault(x => x.ScriptHardwareId == command.ScriptHardwareId && x.Ordering== exchange_order);
                            command2.Ordering = command.Ordering;
                            command.Ordering = exchange_order;
                            //
                            db.CommandsScript.UpdateRange(command, command2);
                            db.SaveChanges();
                            //mAdapter.NotifyAll();
                        }
                    }
                    else
                    {
                        if (db.CommandsScript.Any(x => x.ScriptHardwareId == command.ScriptHardwareId && x.Ordering > command.Ordering))
                        {
                            int exchange_order = db.CommandsScript.Where(x => x.ScriptHardwareId == command.ScriptHardwareId && x.Ordering > command.Ordering).Min(x => x.Ordering);
                            CommandScriptModel command2 = db.CommandsScript.FirstOrDefault(x => x.ScriptHardwareId == command.ScriptHardwareId && x.Ordering == exchange_order);
                            command2.Ordering = command.Ordering;
                            command.Ordering = exchange_order;
                            //
                            db.CommandsScript.UpdateRange(command, command2);
                            db.SaveChanges();
                            //mAdapter.Notify();
                        }
                    }
                    mAdapter = new CommandsListAdapter(this, script.Id);
                    mRecyclerView.SetAdapter(mAdapter);
                    mAdapter.ItemClick += OnItemClick;
                    mAdapter.MoveOrderingCommand += MoveOrderingCommand;
                }
            }
        }

        private void OnItemClick(object sender, int CommandId)
        {
            Intent CancelIntent = new Intent(this, typeof(CommandEditActivity));
            CancelIntent.PutExtra(nameof(CommandScriptModel.Id), CommandId);
            StartActivity(CancelIntent);
        }

        protected override void OnPause()
        {
            base.OnPause();
            mAdapter.ItemClick -= OnItemClick;
            mAdapter.MoveOrderingCommand -= MoveOrderingCommand;
        }

        protected override void ButtonAddOnClick(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(this, typeof(CommandAddActivity));
            intent.PutExtra(nameof(script.Id), script.Id);
            StartActivity(intent);
        }
    }
}

