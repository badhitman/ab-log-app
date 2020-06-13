////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class CloudActivity : aActivity
    {
        protected override int ViewId => Resource.Layout.cloud_activity;
        protected override int ToolbarId => Resource.Id.cloud_toolbar;
        protected override int DrawerLayoutId => Resource.Id.cloud_drawer_layout;
        protected override int NavId => Resource.Id.cloud_nav_view;

        private Switch switch_enable_first_cloud;
        private EditText cloud_first_email_login;
        private EditText cloud_first_pass;
        private EditText cloud_first_pop3;
        private EditText cloud_first_smtp;

        private Spinner second_cloud_mode_spinner;

        private Switch switch_enable_second_cloud;
        private EditText cloud_second_email_login;
        private EditText cloud_second_pass;
        private EditText cloud_second_pop3;
        private EditText cloud_second_smtp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.second_cloud_mode_spinner);

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, Resources.GetStringArray(Resource.Array.cloud_modes));

            spinner.Adapter = adapter;

            ////////////////////////////////////////////////////////////////////////////////
            switch_enable_first_cloud = FindViewById<Switch>(Resource.Id.switch_enable_first_cloud);
            switch_enable_first_cloud.Checked = Preferences.Get(Resources.GetResourceEntryName(switch_enable_first_cloud.Id), false);
            //
            cloud_first_email_login = FindViewById<EditText>(Resource.Id.cloud_first_email_login);
            cloud_first_email_login.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_email_login.Id), "");
            //
            cloud_first_pass = FindViewById<EditText>(Resource.Id.cloud_first_pass);
            cloud_first_pass.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_pass.Id), "");
            //
            cloud_first_pop3 = FindViewById<EditText>(Resource.Id.cloud_first_pop3);
            cloud_first_pop3.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_pop3.Id), "");
            //
            cloud_first_smtp = FindViewById<EditText>(Resource.Id.cloud_first_smtp);
            cloud_first_smtp.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_first_smtp.Id), "");

            second_cloud_mode_spinner = FindViewById<Spinner>(Resource.Id.second_cloud_mode_spinner);
            second_cloud_mode_spinner.SetSelection(Preferences.Get(Resources.GetResourceEntryName(second_cloud_mode_spinner.Id), 0), true);

            switch_enable_second_cloud = FindViewById<Switch>(Resource.Id.switch_enable_second_cloud);
            switch_enable_second_cloud.Checked = Preferences.Get(Resources.GetResourceEntryName(switch_enable_second_cloud.Id), false);
            //
            cloud_second_email_login = FindViewById<EditText>(Resource.Id.cloud_second_email_login);
            cloud_second_email_login.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_email_login.Id), "");
            //
            cloud_second_pass = FindViewById<EditText>(Resource.Id.cloud_second_pass);
            cloud_second_pass.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_pass.Id), "");
            //
            cloud_second_pop3 = FindViewById<EditText>(Resource.Id.cloud_second_pop3);
            cloud_second_pop3.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_pop3.Id), "");
            //
            cloud_second_smtp = FindViewById<EditText>(Resource.Id.cloud_second_smtp);
            cloud_second_smtp.Text = Preferences.Get(Resources.GetResourceEntryName(cloud_second_smtp.Id), "");
        }

        protected override void OnResume()
        {
            base.OnResume();

            switch_enable_first_cloud.CheckedChange += CloudEnable_CheckedChange;

            cloud_first_email_login.FocusChange += CloudPref_FocusChange;
            cloud_first_pass.FocusChange += CloudPref_FocusChange;
            cloud_first_pop3.FocusChange += CloudPref_FocusChange;
            cloud_first_smtp.FocusChange += CloudPref_FocusChange;

            second_cloud_mode_spinner.ItemSelected += SecondCloudMode_ItemSelected;

            switch_enable_second_cloud.CheckedChange += CloudEnable_CheckedChange;

            cloud_second_email_login.FocusChange += CloudPref_FocusChange;
            cloud_second_pass.FocusChange += CloudPref_FocusChange;
            cloud_second_pop3.FocusChange += CloudPref_FocusChange;
            cloud_second_smtp.FocusChange += CloudPref_FocusChange;
        }

        protected override void OnPause()
        {
            base.OnPause();

            switch_enable_first_cloud.CheckedChange -= CloudEnable_CheckedChange;

            cloud_first_email_login.FocusChange -= CloudPref_FocusChange;
            cloud_first_pass.FocusChange -= CloudPref_FocusChange;
            cloud_first_pop3.FocusChange -= CloudPref_FocusChange;
            cloud_first_smtp.FocusChange -= CloudPref_FocusChange;

            second_cloud_mode_spinner.ItemSelected -= SecondCloudMode_ItemSelected;

            switch_enable_second_cloud.CheckedChange -= CloudEnable_CheckedChange;

            cloud_second_email_login.FocusChange -= CloudPref_FocusChange;
            cloud_second_pass.FocusChange -= CloudPref_FocusChange;
            cloud_second_pop3.FocusChange -= CloudPref_FocusChange;
            cloud_second_smtp.FocusChange -= CloudPref_FocusChange;
        }

        private void CloudPref_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            EditText editText = (EditText)sender;
            if (!e.HasFocus)
            {
                Preferences.Set(Resources.GetResourceEntryName(editText.Id), editText.Text);
                LinearLayout linearParent = editText.Parent as LinearLayout;
                if (linearParent.Tag.ToString() == Resources.GetString(Resource.String.main_cloud_layout))
                {
                    switch_enable_first_cloud.Checked = false;
                }
                else
                {
                    switch_enable_second_cloud.Checked = false;
                }
            }
        }

        private void CloudEnable_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            Switch togglerSwitch = (Switch)sender;

            LinearLayout linearParentLayout = togglerSwitch.Parent.Parent as LinearLayout;

            AppCompatEditText login = linearParentLayout.GetChildAt(1) as AppCompatEditText;
            AppCompatEditText pass = linearParentLayout.GetChildAt(2) as AppCompatEditText;
            AppCompatEditText pop3 = linearParentLayout.GetChildAt(3) as AppCompatEditText;
            AppCompatEditText smtp = linearParentLayout.GetChildAt(4) as AppCompatEditText;

            Preferences.Set(Resources.GetResourceEntryName(togglerSwitch.Id), togglerSwitch.Checked);
            if (togglerSwitch.Checked)
            {
                (togglerSwitch.Parent as LinearLayout).AddView(new ProgressBar(this) { ScaleX = 0.4f, ScaleY = 0.4f }, 1);
                togglerSwitch.Enabled = false;

                Task tester = TestEmailConnect(login.Text, pass.Text, pop3.Text, smtp.Text, togglerSwitch);
            }
        }

        private async Task TestEmailConnect(string login, string pass, string pop3, string smtp, Switch switchToggler)
        {
            MimeMessage emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(login, login));
            emailMessage.To.Add(new MailboxAddress(login, login));
            emailMessage.Subject = GetType().Name;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "test connect"
            };

            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(smtp, 465, true);
                    await client.AuthenticateAsync("ru.usa@mail.ru", pass);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
                //catch (MailKit.Security.AuthenticationException e)
                //{
                //    int i = 0;
                //}
                catch (Exception e)
                {
                    RunOnUiThread(() =>
                    {
                        switchToggler.Checked = false;
                    });
                }
            }

            using (Pop3Client client = new Pop3Client())
            {

            }

            switchToggler.Enabled = true;
            (switchToggler.Parent as LinearLayout).RemoveViewAt(1);
        }

        private void SecondCloudMode_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            Preferences.Set(Resources.GetResourceEntryName(spinner.Id), spinner.SelectedItemPosition);
        }
    }
}