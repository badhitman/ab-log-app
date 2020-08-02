////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using ab.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Webkit;
using Android.Widget;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace ab
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class HardwareSystemSettingsActivity : AbstractActivity
    {
        public static readonly string TAG = "● hardware-system-settings-activity";

        protected override int ViewId => Resource.Layout.hardware_system_settings_activity;
        protected override int ToolbarId => Resource.Id.hardware_system_settings_toolbar;
        protected override int DrawerLayoutId => Resource.Id.hardware_system_settings_app_drawer_layout;
        protected override int NavId => Resource.Id.hardware_system_settings_app_nav_view;
        string[] supported_firmwares = new string[] { "(fw: 4.45b5)" };

        WebView webView;
        TextView hardwareCardSubHeader;
        HardwareModel hardware;
        LinearLayout HardwareSystemSettingsLayout;

        Regex a_href_regex = new Regex(@"<a\s+href=([^>]+)>([^<]+)</a>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex form_regex = new Regex(@"<form\s+action=([^>]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        Regex input_regex = new Regex(@"<input[^>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex input_type_regex = new Regex(@"\s+type=(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex input_name_regex = new Regex(@"\s+name=(\w+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        bool external_web_mode = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(TAG, "OnCreate");

            base.OnCreate(savedInstanceState);
            int id = Intent.Extras.GetInt(nameof(HardwareModel.Id), 0);
            hardwareCardSubHeader = FindViewById<TextView>(Resource.Id.hardware_system_settings_card_sub_header);
            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    hardware = db.Hardwares.Include(x => x.Ports).FirstOrDefault(x => x.Id == id);
                }
            }
            hardwareCardSubHeader.Text = hardware.Name;
            HardwareSystemSettingsLayout = FindViewById<LinearLayout>(Resource.Id.hardware_system_settings_layout);

            GetHttp($"http://{hardware.Address}/{hardware.Password}/");
        }

        public async void GetHttp(string url)
        {
            Log.Debug(TAG, $"GetHttp - {url}");

            Android.Net.Uri uri = Android.Net.Uri.Parse(url);
            if (uri.Host == "ab-log.ru")
            {
                if (!url.EndsWith("/"))
                {
                    url += "/";
                }
                url += "?ref=https://github.com/badhitman/ab-log-app";
                await Browser.OpenAsync(url, BrowserLaunchMode.External);
                return;
            }

            HardwareSystemSettingsLayout.RemoveAllViews();
            HardwareSystemSettingsLayout.AddView(new ProgressBar(this) { Indeterminate = true });

            webView = new WebView(this);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
            webView.Settings.DomStorageEnabled = true;

            MyWebViewClient myWebViewClient = new MyWebViewClient();

            myWebViewClient.ShouldUrlLoading += delegate (string new_url) { GetHttp(new_url); };

            webView.SetWebViewClient(myWebViewClient);
            string html_raw = string.Empty;

            string cf = uri.GetQueryParameter("cf") ?? string.Empty;
            string pt = uri.GetQueryParameter("pt") ?? string.Empty;
            string eip = uri.GetQueryParameter("eip") ?? string.Empty;
            string pwd = uri.GetQueryParameter("pwd") ?? string.Empty;

            string pn = uri.GetQueryParameter("pn") ?? string.Empty;
            string pty = uri.GetQueryParameter("pty") ?? string.Empty;
            string set_port_name = uri.GetQueryParameter("set_port_name");
            if (set_port_name != null)
            {
                url = url
                    .Replace($"set_port_name={WebUtility.UrlEncode(set_port_name)}", string.Empty)
                    .Replace("?&", "?");

                if (!string.IsNullOrEmpty(pn))
                {
                    PortModel portHardware = GetPortHardware(pn);
                    if (portHardware == null)
                    {
                        return;
                    }

                    if (portHardware.Name != set_port_name)
                    {
                        portHardware.Name = set_port_name;
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                db.Ports.Update(portHardware);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            using (HttpClient client = new HttpClient() { Timeout = new TimeSpan(0, 0, 5) })
            {
                try
                {
                    HttpResponseMessage httpResponseMessage = await client.GetAsync(url);
                    html_raw = await httpResponseMessage.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                    StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
                    return;
                }
            }



            await Task.Run(() =>
            {
                html_raw = html_raw.Replace("<br>", $"<br>{System.Environment.NewLine}");

                string onload_js;

                /////////////////////////////////////////////////////////////
                /// настройки: основные (ip,gw и т.п.)
                if (cf == "1")
                {
                    onload_js = MyWebViewClient.onload_cf1_js;

                    bool set_ip_address = !string.IsNullOrWhiteSpace(eip);
                    bool set_password = !string.IsNullOrWhiteSpace(pwd);

                    if (set_ip_address || set_password)
                    {
                        Log.Debug(TAG, "save data to DB");
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                List<string> messages = new List<string>();
                                if (set_ip_address && hardware.Address != eip)
                                {
                                    hardware.Address = eip;
                                    messages.Add(GetText(Resource.String.hardware_ip_are_saved_title));
                                }
                                if (set_password && hardware.Password != pwd)
                                {
                                    hardware.Password = pwd;
                                    messages.Add(GetText(Resource.String.hardware_password_are_saved_title));
                                }
                                if (messages.Count > 0)
                                {
                                    db.Hardwares.Update(hardware);
                                    db.SaveChanges();
                                    RunOnUiThread(() =>
                                    {
                                        Toast.MakeText(this, $" • {string.Join($"{System.Environment.NewLine} • ", messages)}", ToastLength.Short).Show();
                                    });
                                    StartActivity(new Intent(Application.Context, typeof(HardwaresListActivity)));
                                    return;
                                }
                            }
                        }
                    }

                    if (external_web_mode)
                    {
                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });

                        html_raw = html_raw
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}")
                        .Replace("<option", $"{System.Environment.NewLine}<option")
                        .Replace("<select", "<select class=\"form-control\"");

                        html_raw = a_href_regex.Replace(html_raw, (Match match) =>
                        {
                            switch (match.Groups[2].Value.ToLower())
                            {
                                case "back":
                                    return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>";
                                default:
                                    return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-outline-info btn-sm\" role=\"button\">{match.Groups[2].Value}</a>";
                            }
                        });

                        html_raw = input_regex.Replace(html_raw, (Match match) =>
                        {
                            string input_raw = match.Groups[0].Value;
                            bool input_with_out_type = true;
                            input_raw = input_type_regex.Replace(input_raw, (Match sub_match) =>
                            {
                                input_with_out_type = false;
                                string input_type_raw = sub_match.Groups[0].Value;
                                switch (sub_match.Groups[1].Value.ToLower())
                                {
                                    case "checkbox":
                                        return $" class=\"form-check-input pb-4 mb-4 ml-1\" {input_type_raw}";
                                    case "submit":
                                        return $" class=\"btn btn-outline-primary btn-block\" {input_type_raw}";
                                    case "hidden":
                                        return input_type_raw;
                                    default:
                                        return $" class=\"form-control\" {input_type_raw}";
                                }
                            });

                            if (input_with_out_type)
                            {
                                input_raw = input_raw.Replace("<input", "<input class=\"form-control\"");
                            }

                            input_raw = input_name_regex.Replace(input_raw, (Match sub_match) =>
                            {
                                string input_name_raw = sub_match.Groups[1].Value;
                                switch (input_name_raw.ToLower())
                                {
                                    case "eip":
                                        return $" placeholder=\"IP адрес устройства\" name=\"{input_name_raw}\"";
                                    case "pwd":
                                        return $" placeholder=\"Пароль\" name=\"{input_name_raw}\"";
                                    case "gw":
                                        return $" placeholder=\"Шлюз\" name=\"{input_name_raw}\"";
                                    case "sip":
                                        return $" placeholder=\"IP-адрес главного сервера\" name=\"{input_name_raw}\"";
                                    case "auth":
                                        return $" placeholder=\"MQTT пароль\" name=\"{input_name_raw}\"";
                                    case "sct":
                                        return $" placeholder=\"Скрипт обработки сообщений\" name=\"{input_name_raw}\"";
                                    //case "rtf":
                                    //    return $"{sub_match.Groups[0].Value} <small class=\"form-text text-muted\">Принудительная доставка MQTT</small>";
                                    case "pr":
                                        return $" placeholder=\"Сторожевой сценарий\" name=\"{input_name_raw}\"";
                                    default:
                                        return $" name=\"{input_name_raw}\"";
                                }
                            });

                            return input_raw;
                        });
                        int connected_status = html_raw.ToLower().IndexOf("disconnected");
                        if (connected_status > 0)
                        {
                            html_raw = $"{html_raw.Substring(0, connected_status)}<div class=\"d-flex justify-content-end pt-3\"><span class=\"badge badge-secondary\">Disconnected</span></div>{html_raw.Substring(connected_status + 12)}";
                        }
                        else
                        {
                            connected_status = html_raw.ToLower().IndexOf("connected");
                            if (connected_status > 0)
                            {
                                html_raw = $"{html_raw.Substring(0, connected_status)}<div class=\"d-flex justify-content-end pt-3\"><span class=\"badge badge-success\">Connected</span></div>{html_raw.Substring(connected_status + 9)}";
                            }
                        }

                        html_raw += $"{System.Environment.NewLine}<div class=\"alert alert-info mt-3\" role=\"alert\"><h6>Информация</h6>" +
                        "<p><strong>IP:</strong> адрес устройства (MAC-адрес устройства генерируется динамически на основе IP-адреса)</p>" +
                        "<p><strong>Pwd:</strong> пароль для доступа к устройству (максимально 3 символа)</p>" +
                        "<p><strong>GW:</strong> шлюз. Имеет смысл указывать только если сервер находится за пределами текущей IP-сети. Если не указан, то в поле отображается значение 255.255.255.255</p>" +
                        "<p><strong>SRV:</strong> IP-адрес главного сервера (HTTP/MQTT), на который MegaD-2561 будет отправлять сообщения о сработавших входах. После IP-адреса возможно указать порт. По умолчанию 80.</p>" +
                        "<p><strong>Script:</strong> <mark>(в случае использования HTTP)</mark> скрипт на сервере, который обрабатывает сообщения от устройства и формирует ответы (максимально 15 символов).</p>" +
                        "<p><strong>Wdog:</strong> функция слежения за сервером. Если используется сервер (указан его IP-адрес и скрипт), то устройство примерно раз в 2 минуты проверяет его доступность и в случае, если сервер не отвечает выполняет сценарий порта, который указывается в поле Wdog. Подробнее о сценариях описано ниже.</p>" +
                        "<p><strong>UART:</strong> выбор режима UART-портов (P32/P33) - работа с GSM-модулем типа SIM800L; работа в режиме RS485-Modbus RTU.</p>" +
                        "<p><strong>Uptime:</strong> время работы устройства после старта</p>" +
                        "<p><strong>Temp:</strong> температура микросхемы часов реального времени DS3231, температура внутри корпуса (только для версии MegaD-2561-RTC)</p>" +
                        "</div>";
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: Megad-ID
                else if (cf == "2")
                {
                    onload_js = MyWebViewClient.onload_cf2_js;

                    if (external_web_mode)
                    {
                        html_raw = html_raw
                        .Replace("Megad-ID:", $"{System.Environment.NewLine}<label>Megad-ID:</label>{System.Environment.NewLine}")
                        .Replace("<input type=hidden name=cf value=2>", $"<input type=\"hidden\" name=\"cf\" value=\"2\"/>")
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}")
                        .Replace("<input type=submit value=Save>", "<input class=\"btn btn-outline-primary btn-block\" type=\"submit\" value=\"Save\">")
                        .Replace("<input name=mdid", "<input class=\"form-control\" name=\"mdid\" placeholder=\"Идентификатор\"");

                        html_raw = Regex.Replace(html_raw, @"srv loop:\s+(<input[^>]+>)", (Match match) =>
                        {
                            string input_raw = match.Groups[1].Value
                            .Replace("type=checkbox", "type=\"checkbox\"")
                            .Replace(">", " />")
                            .Replace("<input", "<input id=\"srv-loop\" class=\"custom-control-input\"");
                            return $"<div class=\"custom-control custom-checkbox\">{System.Environment.NewLine}{input_raw}{System.Environment.NewLine}<label for=\"srv-loop\" class=\"custom-control-label\">srv loop</label>{System.Environment.NewLine}</div>{System.Environment.NewLine}";
                        });

                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });
                        html_raw = a_href_regex.Replace(html_raw, (Match match) => { return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>"; });

                        html_raw += "<div class=\"alert alert-info mt-3\" role=\"alert\"><h4>Информация</h4><p><strong>Megad-ID</strong> используется в MQTT для формирования подписки <code>Topic</code>, а тк же <code>ClientId</code>. Например, если назначить это значение <u>droid</u>, то упрвляющий блок подпишется в MQTT брокере на топик <u>droid/cmd</u> с идентификатором клиента <u>megad-droid</u>. В то же время: состояние портов управляющий блок будет публиковать в топиках с именами подстать номерам портов. Например для порта №30(P30) сообщения от блока будут в топике <u>droid/30</u>. Информация о всех событиях MQTT подписок/сообщений в брокере доступна в логах приложения.</p><p><strong>srv loop</strong> включает режим регулярной отправки (раз в минуту) на сервер состояние всех портов</p></div>";
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: XT1
                else if (cf == "3")
                {
                    onload_js = MyWebViewClient.onload_cf3_js;

                    if (external_web_mode)
                    {
                        html_raw = a_href_regex.Replace(html_raw, (Match m) =>
                        {
                            switch (m.Groups[2].Value.ToLower())
                            {
                                case "back":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{m.Groups[2].Value}</a>";
                                default:
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-info btn-block\" role=\"button\">{m.Groups[2].Value}</a>";
                            }
                        });
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: XT2
                else if (cf == "4")
                {
                    onload_js = MyWebViewClient.onload_cf4_js;

                    if (external_web_mode)
                    {
                        html_raw = a_href_regex.Replace(html_raw, (Match m) =>
                        {
                            switch (m.Groups[2].Value.ToLower())
                            {
                                case "back":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{m.Groups[2].Value}</a>";
                                default:
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-info btn-block\" role=\"button\">{m.Groups[2].Value}</a>";
                            }
                        });
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: cron
                else if (cf == "7")
                {
                    onload_js = MyWebViewClient.onload_cf7_js;

                    if (external_web_mode)
                    {
                        html_raw = html_raw
                        .Replace("<input type=hidden name=cf value=7>", $"<input type=\"hidden\" name=\"cf\" value=\"7\"/>")
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}")
                        .Replace("<input type=submit value=Save>", "<input class=\"btn btn-outline-primary btn-block\" type=\"submit\" value=\"Save\"/>");

                        html_raw = Regex.Replace(html_raw, @"<input name=([\w\d]+)", (Match match) =>
                         {
                             return $"<input class=\"form-control\" name=\"{match.Groups[1].Value}\"";
                         });

                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });
                        html_raw = a_href_regex.Replace(html_raw, (Match match) => { return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>"; });

                        html_raw += "<div class=\"alert alert-info mt-3\" role=\"alert\"><h4>Информация</h4>" +
                        "<p><strong>Cur time:</strong> текущее время. В квадратных скобках указан день недели (1-7). Например [5] - пятница.</p>" +
                        "<p><strong>Set time:</strong> здесь можно задать время. Формат ЧЧ:ММ:СС:ДН, то есть 15:30:00:5 - последняя цифра - день недели</p>" +
                        "<p><strong>SCL/SDA:</strong> порты микроконтроллера, к которым подключены часы. Здесь указывается не номер порта (как обычно), а его индекс (можно посмотреть в документации). Это сделано для того, чтобы имелась возможность подключить часы не только к разъему XT2 (зеленые клеммники внизу) и исполнительным модулям MegaD-14-IOR, но и к внутреннему 16-пиновому разъему XP4.</p>" +
                        "<div class=\"shadow p-3 mb-4 bg-white rounded\">В случае использования контроллера в исполнении MegaD-2561-RTC можно оставить эти поля пустыми. Контроллер сам определит наличие платы часов, подключенных к служебному разъему XP4 и синхронизируется с ними.</div>" +
                        "<p><strong>T/Act:</strong>  Сами задания. Их может быть 5 шт.</p>" +
                        "<p><strong>T:</strong> Расписание в формате ЧЧ:ММ:ДН</p>" +
                        "<div class=\"shadow p-3 mb-4 bg-white rounded\"><code>14:30:0</code> - выполнять в 14:30 каждый день (последний 0 - означает каждый день)<br/>" +
                        "<code>08:00:3</code> - выполнить в 8:00 в среду(3 - среда)<br/>" +
                        "<code>03:15:3-7</code> - выполнять в 03:15 со среды по воскресенье включительно(3-7)<br/>" +
                        "<code>*:/03:0</code> - выполняется каждые 3 минуты.Вместо значения \"час\" необходимо задать '*'.Последний ':0' - дни недели. 0 - каждый день.День недели учитывается.Можно задать выполнение циклической операции в определенные дни недели.<br/>" +
                        "<code>/02:15:0</code> - выполняется каждые 2 часа в 15 минут.То есть в 2:15; 4:15; 6:15 и т.д.</div>" +
                        "<p><strong>Act:</strong> стандартное поле сценария. Важно, что здесь работают паузы (команды p). То есть, если необходимо включить, например, автополив на 30 минут, то не обязательно разносить это на два задания. Можно ограничится одним.</p>" +
                        "<div class=\"shadow p-3 mb-4 bg-white rounded\">" +
                        "<code>7:1;P10;7:0</code> - включить седьмой порт, далее следует пауза 10 секунд, после чего порт будет выключен.<br/>" +
                        "<code>10:1;P600;10:0</code> - включить десятый порт, далее следует пауза 600 секунд, после чего порт будет выключен.<br/>" +
                        "<code>8:1;9:0;12:2</code> - включить восьмой порт и выключить девятый, а двендцтый переключить в противоположное состояние.</div>" +
                        "</div>";
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: rogram
                else if (cf == "9")
                {
                    onload_js = MyWebViewClient.onload_cf9_js;

                    if (external_web_mode)
                    {
                        html_raw = html_raw
                        .Replace("<input type=hidden name=cf value=9>", $"<input type=\"hidden\" name=\"cf\" value=\"9\"/>")
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}") +
                        $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>";

                        html_raw = a_href_regex.Replace(html_raw, (Match match) =>
                        {
                            if (match.Groups[2].Value.ToLower() == "back")
                            {
                                return $"<a href=\"{match.Groups[1].Value}\" class=\"btn btn-primary btn-sm\" role=\"button\">{match.Groups[2].Value}</a>";
                            }
                            else
                            {
                                return $"<a href=\"{match.Groups[1].Value}\" class=\"badge badge-info mt-2\" role=\"button\">{match.Groups[2].Value}</a>";
                            }
                        });

                        html_raw += "<div class=\"alert alert-info mt-3\" role=\"alert\"><h6>Программирование условий для выполнения сценариев</h6>" +
                        "<p>Контроллер MegaD-2561 может выполнять заданные команды (сценарии) в случае возникновения какого-то события: нажата кнопка, температура выросла выше определенного значения и т.д. Для этого в настройках соответствующих портов присутствует поле \"Act\". Но у этого штатного механизма есть ряд ограничений: он работает для входов (IN, ADC, DSen/1W), но не работает для выходов (OUT); нельзя задать дополнительное условие для выполнение сценария (например: нажата кнопка -> включить свет, но только в том случае, если датчик освещенности не выше определенного заданного значения).</p><p>Для решения подобных задач предназначен данный раздел <strong>Program</strong>, который позволяет задавать сценарии в том числе для выходов и строить цепочки подчиненных условий. Таким образом, в каких-то не очень сложных задачах можно обойтись только средствами контроллера без использования сервера.</p>" +
                        "</div>";
                    }
                }
                /////////////////////////////////////////////////////////////
                /// конфигурация условий/сценариев в разделе rogram
                else if (cf == "10")
                {
                    onload_js = MyWebViewClient.onload_cf10_js;
                    string prn = uri.GetQueryParameter("prn") ?? string.Empty;
                    if (external_web_mode)
                    {
                        html_raw = html_raw
                        .Replace($"<a href=/{hardware.Password}/?cf=9>Back</a><br>", $"<a class=\"btn btn-primary btn-sm\" role=\"button\" href=\"/{hardware.Password}/?cf=9\">Back</a><br>")
                        .Replace("<input type=hidden name=cf value=10>", $"<input type=\"hidden\" name=\"cf\" value=\"10\"/>")
                        .Replace("<input type=submit name=pcl value=clear><br>", "<input type=submit name=pcl value=clear>")
                        .Replace("<form", $"<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form")
                        .Replace("<select", "<label>тип сравнения: </label><select class=\"custom-select\"")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>")
                        .Replace(" name=prp ", " name=prp type=\"number\"")
                        .Replace("</style>", $"</style>{System.Environment.NewLine}") +
                        $"{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>";

                        html_raw = form_regex.Replace(html_raw, (Match match) => { return $"<form action=\"{match.Groups[1].Value}\">{System.Environment.NewLine}"; });

                        html_raw = input_regex.Replace(html_raw, (Match match) =>
                        {
                            string input_raw = match.Groups[0].Value;
                            bool input_with_out_type = true;
                            input_raw = input_type_regex.Replace(input_raw, (Match sub_match) =>
                            {
                                input_with_out_type = false;
                                string input_type_raw = sub_match.Groups[0].Value;
                                switch (sub_match.Groups[1].Value.ToLower())
                                {
                                    case "checkbox":
                                        return $" class=\"form-check-input pb-4 mb-4 ml-1\" {input_type_raw}";
                                    case "submit":
                                        return $" class=\"btn btn-outline-primary btn-block my-2\" {input_type_raw}";
                                    case "hidden":
                                        return input_type_raw;
                                    default:
                                        return $" class=\"form-control\" {input_type_raw}";
                                }
                            });

                            if (input_with_out_type)
                            {
                                input_raw = input_raw.Replace("<input", "<input class=\"form-control\"");
                            }

                            return input_raw;
                        });

                        html_raw += $"{System.Environment.NewLine}<div class=\"alert alert-info mt-3\" role=\"alert\"><h6>Программирование условий для выполнения сценариев</h6>" +
                        "<p>Задается номер порта, тип сравнения (больше / меньше / равно), значение, сценарий, тип условия (основное / подчиненное).</p>" +
                        "</div>";
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройка порта
                else if (!string.IsNullOrWhiteSpace(pt))
                {
                    onload_js = MyWebViewClient.onload_pt_js;

                    PortModel portHardware = GetPortHardware(pt);
                    if (portHardware == null)
                    {
                        return;
                    }

                    if (external_web_mode)
                    {
                        if (html_raw.Contains("selected>Out<option"))
                        {
                            if (html_raw.Contains($"P{pt}/ON<br>"))
                            {
                                html_raw = html_raw.Replace($"<a href=/{hardware.Password}/?pt={pt}&cmd={pt}:1>ON</a>", $"<nav class=\"nav nav-pills nav-fill\"><a class=\"nav-item nav-link active\" href=\"/{hardware.Password}/?pt={pt}&cmd={pt}:1\">ON</a>");
                                html_raw = html_raw.Replace($"<a href=/{hardware.Password}/?pt={pt}&cmd={pt}:0>OFF</a>", $"<a class=\"nav-item nav-link btn btn-outline-secondary ml-2\" href=\"/{hardware.Password}/?pt={pt}&cmd={pt}:0\">OFF</a></nav>");
                            }
                            else
                            {
                                html_raw = html_raw.Replace($"<a href=/{hardware.Password}/?pt={pt}&cmd={pt}:1>ON</a>", $"<nav class=\"nav nav-pills nav-fill\"><a class=\"nav-item nav-link btn btn-outline-secondary mr-2\" href=\"/{hardware.Password}/?pt={pt}&cmd={pt}:1\">ON</a>");
                                html_raw = html_raw.Replace($"<a href=/{hardware.Password}/?pt={pt}&cmd={pt}:0>OFF</a>", $"<a class=\"nav-item nav-link active\" href=\"/{hardware.Password}/?pt={pt}&cmd={pt}:0\">OFF</a></nav>");
                            }
                        }

                        html_raw = a_href_regex.Replace(html_raw, (Match match) =>
                        {
                            return $"<a class=\"btn btn-primary btn-sm\" role=\"button\" href=\"{match.Groups[1].Value}\">Back</a> <a class=\"btn btn-info btn-sm\" role=\"button\" onclick=\"window.location.reload()\">Reload</a>";
                        });

                        html_raw = html_raw
                        .Replace("<style>input,select{margin:1px}</style>", string.Empty)
                        .Replace($"<form action=/{hardware.Password}/>", $"{System.Environment.NewLine}<div class=\"card mt-2\">{System.Environment.NewLine}<div class=\"card-body\">{System.Environment.NewLine}<form action=\"/{hardware.Password}/\">{System.Environment.NewLine}<div class=\"form-group\"><label>Name</label><input type=\"text\" name=\"set_port_name\" class=\"form-control\" placeholder=\"Наименовние\" value=\"{portHardware.Name}\"></div>")
                        .Replace("<input type=submit value=Save>", "<input class=\"btn btn-outline-primary btn-block\" type=\"submit\" value=\"Save\"/>")
                        .Replace("</form>", $"{System.Environment.NewLine}</form>{System.Environment.NewLine}</div>{System.Environment.NewLine}</div>")
                        .Replace("Type <select", "Type <select class=\"form-control\"")
                        .Replace("Val <input", "Val <input type=\"number\" class=\"form-control\"")
                        .Replace("Hst <input", "Hst <input type=\"number\" class=\"form-control\"")
                        .Replace("Sen <select name=d>", "Sen <select class=\"form-control\" name=d>")
                        .Replace("<input name=misc", "<input name=\"misc\" class=\"form-control\"")
                        .Replace("Group <input", "Group <input class=\"form-control\"")
                        .Replace($"<a href=/{hardware.Password}/?pt={pt}&cmd=list>Device List</a>", $"<a class=\"btn btn-info btn-block\" href=\"/{hardware.Password}/?pt={pt}&cmd=list\" role=\"button\">Device List</a>")
                        .Replace($"<input type=hidden name=pn value={pt}>", $"<input type=\"hidden\" name=\"pn\" value=\"{pt}\"/>{System.Environment.NewLine}");

                        //
                        html_raw = Regex.Replace(html_raw, @"Act\s+(<input[^>]+>)\s+(<input[^>]+>)<br>", (Match match) =>
                        {
                            return "<div class=\"input-group mb-3\">" +
                            "<div class=\"input-group-prepend\">" +
                            "<span class=\"input-group-text\">Act</span>" +
                            "</div>" +
                            match.Groups[1].Value.Replace("<input", "<input type=\"text\" class=\"form-control\"") +
                            "<div class=\"input-group-append\">" +
                            "<div class=\"input-group-text\">" +
                            match.Groups[2].Value +
                            "</div></div></div>";
                        });
                        //
                        html_raw = Regex.Replace(html_raw, @"Net\s+(<input[^>]+>)\s+(<input[^>]+>)<br>", (Match match) =>
                        {
                            return "<div class=\"input-group mb-3\">" +
                            "<div class=\"input-group-prepend\">" +
                            "<span class=\"input-group-text\">Net</span>" +
                            "</div>" +
                            match.Groups[1].Value.Replace("<input", "<input type=\"text\" class=\"form-control\"") +
                            "<div class=\"input-group-append\">" +
                            "<div class=\"input-group-text\">" +
                            match.Groups[2].Value +
                            "</div></div></div>";
                        });
                        //
                        html_raw = Regex.Replace(html_raw, @"Mode\s+(<select name=m.+</select>)\s+(<input[^>]+>)<br>", (Match match) =>
                        {
                            return "<div class=\"input-group mb-3\">" +
                            "<div class=\"input-group-prepend\">" +
                            "<span class=\"input-group-text\">Mode</span>" +
                            "</div>" +
                            match.Groups[1].Value.Replace("<select", "<select class=\"custom-select\"") +
                            "<div class=\"input-group-append\">" +
                            "<div class=\"input-group-text\">" +
                            match.Groups[2].Value +
                            "</div></div></div>";
                        });

                        html_raw = html_raw.Replace("Mode <select name=m>", "Mode <select class=\"form-control\" name=m>")
                        .Replace("Default: <select name=d", "Default: <select class=\"form-control\" name=d");

                        string config_port_help_html;
                        using (StreamReader sr = new StreamReader(Assets.Open("config-port-help.html")))
                        {
                            config_port_help_html = sr.ReadToEnd();
                            //using (StreamWriter sw = new StreamWriter(MyWebViewClient.config_port_help_html, false))
                            //{
                            //    await sw.WriteAsync(await sr.ReadToEndAsync());
                            //}
                        }

                        html_raw += $"{System.Environment.NewLine}<div class=\"alert alert-info mt-3\" role=\"alert\"><h6>Информация</h6>" +
                        "<p>Поле <strong>Type</strong> может принимать следующие значения:<br/>" +
                        "<code>IN</code> - Вход(например, \"сухой контакт, выключатели света\", U - Sensor, датчик протечки, охранные датчики и т.д.)<br/>" +
                        "<code>OUT</code> - Выход(например, включение электроприборов)<br/>" +
                        "<code>DSen</code> - Цифровой датчик(например, датчики температуры DS18B20, температуры - влажности DHT22, считыватели iButton, Wiegand - 26)<br/>" +
                        "<code>I2C</code> - Датчики или иные устройств, подключаемые по шине I2C<br/>" +
                        "<code>ADC</code> - АЦП, аналого - цифровой преобразователь(например, подключение аналоговых датчиков освещенности, давления, газа и т.д.) Доступен не для всех портов.</p>" +
                        "</div>" + config_port_help_html;
                        config_port_help_html = null;
                    }
                }
                /////////////////////////////////////////////////////////////
                /// настройки: начальное навигационное окно
                else
                {
                    onload_js = MyWebViewClient.onload_root_js;
                    if (!external_web_mode)
                    {
                        foreach (string s in supported_firmwares)
                        {
                            external_web_mode = html_raw.Contains(s);
                            if (external_web_mode)
                            {
                                break;
                            }
                        }
                    }

                    if (external_web_mode)
                    {
                        html_raw = a_href_regex.Replace(html_raw.Replace("<br>", string.Empty), (Match m) =>
                        {
                            switch (m.Groups[2].Value.ToLower())
                            {
                                case "config":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-primary btn-block mt-2\" role=\"button\">{m.Groups[2].Value}</a>";
                                case "xp1":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-info btn-block\" role=\"button\">{m.Groups[2].Value} // P0-P13 & P14</a>";
                                case "xp2":
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-info btn-block\" role=\"button\">{m.Groups[2].Value} // P15(0)-P28(13) & P29(14)</a>";
                                case "ab-log.ru":
                                    return $"<a href=\"{m.Groups[1].Value}\"><u>{m.Groups[2].Value}</u></a>";
                                default:
                                    return $"<a href=\"{m.Groups[1].Value}\" class=\"btn btn-outline-primary btn-block\" role=\"button\">{m.Groups[2].Value}</a>";
                            }
                        });
                    }
                }

                if (html_raw == "Unauthorized")
                {
                    html_raw +=
                    "<div style=\"color: #721c24; background-color: #f8d7da; border-color: #f5c6cb; margin-top: 1rem; padding: .75rem 1.25rem; margin-bottom: 1rem; border: 1px solid transparent; box-sizing: border-box; font-size: 1rem; font-weight: 400; line-height: 1.5;\">" +
                    $"Пароль доступа неверный</div>";
                }
                else if (!external_web_mode && html_raw.Length > 12 && html_raw != "Unauthorized")
                {
                    html_raw +=
                    "<div style=\"color: #856404; background-color: #fff3cd; border-color: #ffeeba; margin-top: 1rem; padding: .75rem 1.25rem; margin-bottom: 1rem; border: 1px solid transparent; box-sizing: border-box; font-size: 1rem; font-weight: 400; line-height: 1.5;\">" +
                    $"Режим модифицированого WEB интерфейса отключён. Версия вашей прошивки не поддерживается.<hr/>Поддерживаются прошивки: <span style=\"color: #fff;background-color: #007bff;display: inline-block;padding: .25em .4em;font-size: 75%;font-weight: 700;line-height: 1;text-align: center;white-space: nowrap;vertical-align: baseline;border-radius: .25rem;box-sizing: border-box;\">{String.Join("</span><span style=\"color: #fff;background-color: #007bff;display: inline-block;padding: .25em .4em;font-size: 75%;font-weight: 700;line-height: 1;text-align: center;white-space: nowrap;vertical-align: baseline;border-radius: .25rem;box-sizing: border-box;\">", supported_firmwares)}</span></div>";
                }
                else
                {
                    html_raw =
                    $"<link rel=\"stylesheet\" href=\"file:///{MyWebViewClient.bootstrap_min_css}\">" + System.Environment.NewLine +
                    $"<script src=\"file:///{MyWebViewClient.jquery_slim_min_js}\"></script>" + System.Environment.NewLine +
                    $"<script src=\"file:///{MyWebViewClient.popper_min_js}\" ></script>" + System.Environment.NewLine +
                    $"<script src=\"file:///{MyWebViewClient.bootstrap_min_js}\"></script>" + System.Environment.NewLine +
                    $"<script src=\"file:///{onload_js}\"></script>" + System.Environment.NewLine + html_raw;
                }
            });
            webView.LoadDataWithBaseURL(url, html_raw, "text/html", "utf-8", url);

            HardwareSystemSettingsLayout.RemoveAllViews();
            HardwareSystemSettingsLayout.AddView(webView);
        }

        private PortModel GetPortHardware(string port)
        {
            Log.Debug(TAG, $"GetPortHardware - {port}");

            int port_num = int.Parse(port);
            PortModel portHardware = hardware.Ports.FirstOrDefault(x => x.PortNumb == port_num);
            if (portHardware == null)
            {
                lock (DatabaseContext.DbLocker)
                {
                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                    {
                        portHardware = new PortModel()
                        {
                            HardwareId = hardware.Id,
                            PortNumb = port_num
                        };
                        db.Ports.Add(portHardware);
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            string err_msg = ex.Message;
                            if (ex.InnerException != null)
                            {
                                err_msg += System.Environment.NewLine + ex.InnerException.Message;
                            }
                            Log.Error(TAG, err_msg);
                            Toast.MakeText(this, err_msg, ToastLength.Long);
                            return null;
                        }
                        hardware.Ports.Add(portHardware);
                    }
                }
            }
            return portHardware;
        }
    }
}