////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Services;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System.Threading.Tasks;
using TelegramBotMin;
using Xamarin.Essentials;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class TelegramBotActivity : AbstractActivity
    {
        public static new readonly string TAG = "● telegram-bot-activity";

        protected override int ViewId => Resource.Layout.telegram_activity;
        protected override int ToolbarId => Resource.Id.telegram_toolbar;
        protected override int DrawerLayoutId => Resource.Id.telegram_app_drawer_layout;
        protected override int NavId => Resource.Id.telegram_app_nav_view;

        EditText editTextToken;
        Button buttonCheckTelegramBotToken;
        LinearLayout linearLayoutStatusCheckTelegramBotToken;
        RecyclerView recyclerViewUsersList;
        RecyclerView.LayoutManager mLayoutManager;
        TelegramUsersListAdapter telegramUsersListAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");
            base.OnCreate(savedInstanceState);
            editTextToken = FindViewById<EditText>(Resource.Id.editTextTelegramBotToken);
            buttonCheckTelegramBotToken = FindViewById<Button>(Resource.Id.button_check_telegram_bot_token);
            linearLayoutStatusCheckTelegramBotToken = FindViewById<LinearLayout>(Resource.Id.linearLayoutCheckTelegramBotToken);
            recyclerViewUsersList = FindViewById<RecyclerView>(Resource.Id.recyclerViewTelegramUsersList);
            string token = Preferences.Get(Constants.TELEGRAM_TOKEN, string.Empty);//Constants.TELEGRAM_TOKEN
            editTextToken.Text = token;
            buttonCheckTelegramBotToken.Enabled = !string.IsNullOrEmpty(token);

            mLayoutManager = new LinearLayoutManager(this);
            recyclerViewUsersList.SetLayoutManager(mLayoutManager);

            telegramUsersListAdapter = new TelegramUsersListAdapter(this);
            recyclerViewUsersList.SetAdapter(telegramUsersListAdapter);
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();
            editTextToken.TextChanged += EditTextToken_TextChanged;
            buttonCheckTelegramBotToken.Click += ButtonCheckTelegramBotToken_Click;
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause");
            base.OnPause();
            editTextToken.TextChanged -= EditTextToken_TextChanged;
            buttonCheckTelegramBotToken.Click -= ButtonCheckTelegramBotToken_Click;
        }

        private void EditTextToken_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            Log.Debug(TAG, "EditTextToken_TextChanged");
            EditText editText = sender as EditText;
            Preferences.Set(Constants.TELEGRAM_TOKEN, editText.Text);
            buttonCheckTelegramBotToken.Enabled = !string.IsNullOrEmpty(editText.Text);
            Preferences.Set(Constants.TELEGRAM_BOT_ID, (long)0);
        }

        private void ButtonCheckTelegramBotToken_Click(object sender, System.EventArgs e)
        {
            Log.Debug(TAG, "ButtonCheckTelegramBotToken_Click");
            buttonCheckTelegramBotToken.Enabled = false;
            editTextToken.Enabled = false;
            CheckTelegramBotToken();
        }

        private async void CheckTelegramBotToken()
        {
            Log.Debug(TAG, "CheckTelegramBotToken");
            Preferences.Set(Constants.TELEGRAM_BOT_ID, (long)0);

            linearLayoutStatusCheckTelegramBotToken.RemoveAllViews();
            linearLayoutStatusCheckTelegramBotToken.AddView(new ProgressBar(this) { Indeterminate = true });
            string token = Preferences.Get(Constants.TELEGRAM_TOKEN, string.Empty);

            TelegramClientCore telegramClient = null;
            await Task.Run(() =>
            {
                telegramClient = new TelegramClientCore(token);
                TextView textViewResultTelegramBot = new TextView(this);

                RunOnUiThread(() =>
                {
                    if (telegramClient?.Me == null)
                    {
                        textViewResultTelegramBot.Text = $"http status: {telegramClient.HttpRrequestStatus}";
                        textViewResultTelegramBot.SetTextColor(Color.Red);
                    }
                    else
                    {
                        textViewResultTelegramBot.Text = $"Telegram Bot:{System.Environment.NewLine}{telegramClient.Me.ToString().Replace("[IS_BOT] ", string.Empty).Replace("] [", $"]{System.Environment.NewLine}[")}";
                        textViewResultTelegramBot.SetTextColor(Color.ForestGreen);
                        Preferences.Set(Constants.TELEGRAM_BOT_ID, telegramClient.Me.id);
                    }
                    linearLayoutStatusCheckTelegramBotToken.RemoveAllViews();
                    linearLayoutStatusCheckTelegramBotToken.AddView(textViewResultTelegramBot);

                    buttonCheckTelegramBotToken.Enabled = true;
                    editTextToken.Enabled = true;
                });
            });
        }
    }
}