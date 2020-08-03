////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using System;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.EntityFrameworkCore;
using Android.Content;
using System.Collections.Generic;
using Android.Util;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", NoHistory = true)]
    public class CommandEditActivity : CommandAddActivity
    {
        public static new readonly string TAG = "● command-edit-activity";

        CommandModel command = null;

        AppCompatButton DeleteCommand;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);

            int CommandId = Intent.Extras.GetInt(nameof(CommandModel.Id), 0);

            if (CommandId > 0)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        command = db.Commands.Include(x => x.Script).Include(x => x.PortExecutionCondition).ThenInclude(x => x.Hardware).FirstOrDefault(x => x.Id == CommandId);
                    }
                }
            }

            AbstractSettingsManage.Command = command;
            List<string> commands_types_array = new List<string>(Resources.GetStringArray(Resource.Array.commands_types_array));

            switch (command.TypeCommand)
            {
                case TypesCommands.Port:
                    TypesCommand.SetSelection(commands_types_array.IndexOf(GetString(Resource.String.command_type_array_item_port)));
                    break;
                case TypesCommands.Controller:
                    TypesCommand.SetSelection(commands_types_array.IndexOf(GetString(Resource.String.command_type_array_item_controller)));
                    break;
                case TypesCommands.Exit:
                    TypesCommand.SetSelection(commands_types_array.IndexOf(GetString(Resource.String.command_type_array_item_transit)));
                    break;
            }

            ScriptId = command.ScriptId;

            DeleteCommand = new AppCompatButton(this) { Text = GetText(Resource.String.delete_title) };
            DeleteCommand.SetTextColor(Color.DarkRed);
            DeleteCommand.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            FooterLayout.AddView(DeleteCommand);

            HiddenCommandCheckBox.Checked = command.Hidden;
            CardTitle.Text = command.Script.ToString();
            CardSubtitle.Text = command.ToString();
            PauseSecondsBeforeStarting.Text = command.PauseBeforeExecution.ToString();

            if (command.PortExecutionCondition != null)
            {
                RequiredCondition.Checked = true;

                HardwareCondition.Enabled = true;
                HardwareCondition.SetSelection(Hardwares.Keys.ToList().IndexOf(command.PortExecutionCondition.HardwareId));

                PortCondition.Enabled = true;
                PortsList_UpdateSpinner(command.PortExecutionCondition.HardwareId, ref PortCondition, command.PortExecutionCondition.Id);
                //PortCondition.SetSelection(Ports.Keys.ToList().IndexOf(command.PortExecutionCondition.Id));

                StateCondition.Enabled = true;
                StateCondition.SetSelection(GetIndexPortState(command.PortExecutionConditionAllowingState, new List<string>(Resources.GetStringArray(Resource.Array.required_condition_port_states_array))));
            }
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();

            DeleteCommand.Click += ButtonDeleteCommands_Click;
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            base.OnPause();
            DeleteCommand.Click -= ButtonDeleteCommands_Click;
        }

        protected override void ButtonOk_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "CardButton_Click");

            command = ReadFormToObject(command);

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    db.Commands.Update(command);
                    db.SaveChanges();
                }
            }

            Intent intent = new Intent(this, typeof(CommandsListActivity));
            intent.PutExtra(nameof(ScriptModel.Id), command.ScriptId);
            StartActivity(intent);
        }

        private void ButtonDeleteCommands_Click(object sender, EventArgs e)
        {
            Log.Debug(TAG, "ButtonDeleteCommands_Click");
            CardTitle.Text = GetText(Resource.String.delete_command_card_title);

            CardSubtitle.Text = GetText(Resource.String.delete_command_card_sub_title);
            CardSubtitle.SetTextColor(Color.IndianRed);

            ButtonOk.Enabled = false;
            ButtonOk.Text = GetText(Resource.String.ok_mute_button_with_remove_command);

            DeleteCommand.Enabled = false;
            DeleteCommand.SetTextColor(Color.Gray);
            DeleteCommand.Click -= ButtonDeleteCommands_Click;

            AppCompatTextView appCompatTextView = new AppCompatTextView(this) { Text = GetText(Resource.String.footer_text_with_remove_command), TextSize = 15 };
            appCompatTextView.SetTextColor(Color.Red);
            appCompatTextView.SetWidth(3);
            FooterLayout.AddView(appCompatTextView);

            AppCompatButton ButtonConfirmDeleteCommand = new AppCompatButton(this) { Text = GetText(Resource.String.button_confirm_remove) };
            ButtonConfirmDeleteCommand.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            ButtonConfirmDeleteCommand.SetTextColor(Color.DarkRed);
            ButtonConfirmDeleteCommand.Click += new EventHandler((sender, eventArg) =>
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        db.Commands.Remove(command);
                        db.SaveChanges();

                        Intent CancelIntent = new Intent(this, typeof(CommandsListActivity));
                        CancelIntent.PutExtra(nameof(CommandModel.Id), ScriptId);
                        StartActivity(CancelIntent);
                    }
                }
            });
            FooterLayout.AddView(ButtonConfirmDeleteCommand);
        }
    }
}