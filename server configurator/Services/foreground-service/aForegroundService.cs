////////////////////////////////////////////////
// © https://github.com/badhitman 
////////////////////////////////////////////////

using ab.Model;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using TelegramBot.TelegramMetadata.AvailableTypes;
using TelegramBot.TelegramMetadata.GettingUpdates;
using TelegramBotMin;
using Xamarin.Essentials;

namespace ab.Services
{
    public abstract class aForegroundService : Service, IForegroundService
    {
        public static readonly string TAG = "● abstract-foreground-service";
        public static readonly string TelegramBotTAG = "● telegram-bot";

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        LogsContext logsDB = new LogsContext();
        public static IForegroundService ForegroundServiceManager;
        private int TelegramBotSurveyInterval = -1;
        private string TelegramBotToken;
        TelegramClientCore telegramClient;
        public IBinder Binder { get; protected set; }

        public bool isStartedForegroundService => ForegroundServiceManager?.isStartedForegroundService ?? false;

        Thread TelegramBotClientThread;

        private void TelegramBotClientAction()
        {
            string log_msg;
            using (LogsContext log = new LogsContext())
            {
                log_msg = "TelegramBotClientAction()";
                Log.Debug(TelegramBotTAG, log_msg);
                log.AddLogRow(LogStatusesEnum.Trac, log_msg, TAG);
            }
            if (TelegramBotSurveyInterval < 1 || string.IsNullOrWhiteSpace(TelegramBotToken))
            {
                log_msg = "TelegramBotSurveyInterval < 1 || string.IsNullOrWhiteSpace(TelegramBotToken)";
                Log.Error(TelegramBotTAG, log_msg);
                using (LogsContext log = new LogsContext())
                {
                    log.AddLogRow(LogStatusesEnum.Error, log_msg, TAG);
                }
                return;
            }
            telegramClient = new TelegramClientCore(TelegramBotToken);
            if (telegramClient?.Me == null)
            {
                log_msg = "telegramClient?.Me == null";
                Log.Error(TelegramBotTAG, log_msg);
                using (LogsContext log = new LogsContext())
                {
                    log.AddLogRow(LogStatusesEnum.Error, log_msg, TAG);
                }
                return;
            }

            Regex get_harware_regex = new Regex(@"^/hw_(\d+)$", RegexOptions.Compiled);
            Regex get_port_regex = new Regex(@"^/port_(\d+)$", RegexOptions.Compiled);
            Regex set_port_regex = new Regex(@"^/port_(\d+)_set_(.+)$", RegexOptions.Compiled);
            Regex view_logs_regex = new Regex(@"^/logs_at_(\d+)$", RegexOptions.Compiled);
            Regex view_script_regex = new Regex(@"^/script_(\d+)$", RegexOptions.Compiled);
            Regex run_script_regex = new Regex(@"^/script_run_(\d+)$", RegexOptions.Compiled);
            Regex confirm_token_regex = new Regex(@"^/token_(\d+)_(\d+)$", RegexOptions.Compiled);

            while (TelegramBotSurveyInterval > 0)
            {
                Log.Debug(TelegramBotTAG, " ~ request telegram updates");
                Update[] updates = null;
                updates = telegramClient.getUpdates(5).Result;

                if (updates == null)
                {
                    log_msg = "telegramClient.getUpdates() == null";
                    Log.Error(TelegramBotTAG, log_msg);
                    using (LogsContext log = new LogsContext())
                    {
                        log.AddLogRow(LogStatusesEnum.Error, log_msg, TAG);
                    }
                    return;
                }

                if (updates.Length > 0)
                {
                    log_msg = $"incoming telegram updates: {updates.Length} items";
                    Log.Info(TelegramBotTAG, log_msg);
                    using (LogsContext log = new LogsContext())
                    {
                        log.AddLogRow(LogStatusesEnum.Info, log_msg, TAG);
                    }

                    TelegramUserModel telegramUser = null;
                    UserModel user = null;
                    updates = updates.Where(x => x.message.chat.type == "private" && !x.message.from.is_bot).ToArray();

                    foreach (var gUser in updates.GroupBy(x => x.message.from.id, (id, users) => new { id, users = users.ToArray() }))
                    {
                        UserClass sender = gUser.users[0].message.from;
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                telegramUser = db.TelegramUsers.FirstOrDefault(x => x.TelegramId == sender.id);
                            }
                        }
                        if (telegramUser == null)
                        {
                            telegramUser = new TelegramUserModel()
                            {
                                Name = $"{sender.first_name.Trim()} {sender.last_name.Trim()}".Trim(),
                                TelegramId = sender.id,
                                UserName = sender.username,
                                TelegramParentBotId = telegramClient.Me.id
                            };
                            log_msg = $"new telegram user: {telegramUser}";
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    db.TelegramUsers.Add(telegramUser);
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            string new_full_name = $"{sender.first_name.Trim()} {sender.last_name.Trim()}".Trim();
                            string new_username = sender.username;
                            long new_telegram_parent_bot_id = telegramClient.Me.id;
                            if (telegramUser.Name != new_full_name || telegramUser.UserName != new_username || telegramUser.TelegramParentBotId != new_telegram_parent_bot_id)
                            {
                                telegramUser.Name = new_full_name;
                                telegramUser.UserName = new_username;
                                telegramUser.TelegramParentBotId = new_telegram_parent_bot_id;
                                log_msg = $"update telegram user: {telegramUser}";
                                lock (DatabaseContext.DbLocker)
                                {
                                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                    {
                                        db.TelegramUsers.Update(telegramUser);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                        Log.Info(TelegramBotTAG, log_msg);
                        using (LogsContext log = new LogsContext())
                        {
                            log.AddLogRow(LogStatusesEnum.Info, log_msg, TAG);
                        }
                    }

                    foreach (Update update in updates)
                    {
                        log_msg = $"incoming telegram message: {update.message.text}";
                        Log.Info(TelegramBotTAG, log_msg);
                        using (LogsContext log = new LogsContext())
                        {
                            log.AddLogRow(LogStatusesEnum.Info, log_msg, TAG);
                        }

                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                db.TelegramMessages.Add(new TelegramMessageModel()
                                {
                                    ChatId = update.message.chat.id,
                                    UpdateId = update.update_id,
                                    MessageId = update.message.message_id,
                                    TelegramBotId = telegramClient.Me.id,
                                    Name = update.message.text
                                });
                                db.SaveChanges();
                            }
                        }


                        if (telegramUser == null || telegramUser.TelegramId != update.message.from.id)
                        {
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    telegramUser = db.TelegramUsers.FirstOrDefault(x => x.TelegramId == update.message.from.id);
                                }
                            }
                        }
                        if (user == null || user.Id != telegramUser.LinkedUserId)
                        {
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    user = telegramUser.LinkedUserId == default ? null : db.Users.FirstOrDefault(x => x.Id == telegramUser.LinkedUserId);
                                }
                            }
                        }



                        if (user?.AlarmSubscriber != true && user?.CommandsAllowed != true)
                        {
                            continue;
                        }

                        string cmd = update.message.text.ToLower();
                        string response_msg = string.Empty;
                        if (cmd == "/hardwares")
                        {
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    foreach (HardwareModel hw in db.Hardwares)
                                    {
                                        response_msg += $"{hw.Name} - /hw_{hw.Id}{System.Environment.NewLine}";
                                    }
                                }
                            }
                        }
                        else if (user.CommandsAllowed && cmd == "/scripts")
                        {
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    foreach (ScriptModel script in db.Scripts)
                                    {
                                        response_msg += $"{script.Name} - /script_{script.Id}{System.Environment.NewLine}";
                                    }
                                }
                            }
                        }
                        else if (user.CommandsAllowed && view_script_regex.IsMatch(cmd))
                        {
                            Match match = view_script_regex.Match(cmd);
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    foreach (CommandModel command in db.Commands.Where(x => x.ScriptId == int.Parse(match.Groups[1].Value)).Include(x => x.PortExecutionCondition).ThenInclude(x => x.Hardware))
                                    {
                                        response_msg +=
                                            $"{command.Name} - {(command.PauseBeforeExecution > 0 ? $"[предв.пауза {command.PauseBeforeExecution} сек.]; " : "")}" +
                                            $"{(command.PortExecutionCondition != null ? $"[предв.пров.порта {command.PortExecutionCondition.Hardware} > {command.PortExecutionCondition}" : "")} > " +
                                            $"{(command.PortExecutionConditionAllowingState == true ? "Вкл" : (command.PortExecutionConditionAllowingState == true ? "Выкл" : "Перекл"))}]" +
                                            $"{System.Environment.NewLine + System.Environment.NewLine}";
                                    }
                                }
                            }
                            response_msg += $"{System.Environment.NewLine}Старт - /script_run_{match.Groups[1].Value}";
                        }
                        else if (user.CommandsAllowed && run_script_regex.IsMatch(cmd))
                        {
                            int script_id = int.Parse(run_script_regex.Match(cmd).Groups[1].Value);
                            ScriptModel scriptHardware = null;
                            if (script_id > 0)
                            {
                                lock (DatabaseContext.DbLocker)
                                {
                                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                    {
                                        scriptHardware = db.Scripts.FirstOrDefault(x => x.Id == script_id);
                                    }
                                }
                            }
                            if (scriptHardware == null)
                            {
                                response_msg += $"Скрипт не найден";
                            }
                            else
                            {
                                Random rnd = new Random();
                                TaskModel taskScript = new TaskModel()
                                {
                                    Name = $"{rnd.Next(1, 13)}{rnd.Next(1, 7)}{rnd.Next(52)}",
                                    ScriptId = script_id,
                                    TaskInitiatorType = TaskInitiatorsTypes.Telegram,
                                    TaskInitiatorId = update.message.from.id
                                };
                                lock (DatabaseContext.DbLocker)
                                {
                                    using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                    {
                                        db.Tasks.Add(taskScript);
                                        db.SaveChanges();
                                        response_msg += $"Токен подтверждения:{System.Environment.NewLine}/token_{taskScript.Id}_{taskScript.Name}";
                                    }
                                }
                            }
                        }
                        else if (user.CommandsAllowed && confirm_token_regex.IsMatch(cmd))
                        {
                            Match match = confirm_token_regex.Match(cmd);
                            int tokenId = int.Parse(match.Groups[1].Value);
                            string tokenSecret = match.Groups[2].Value;
                            TaskModel taskScript = null;
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    db.Tasks.RemoveRange(db.Tasks.Where(x => x.FinishedAt < x.CreatedAt && x.CreatedAt.AddMinutes(1) < DateTime.Now));
                                    db.SaveChanges();

                                    taskScript = db.Tasks
                                        .Include(x => x.Script)
                                        .ThenInclude(x => x.Commands)
                                        .ThenInclude(x => x.PortExecutionCondition)
                                        .ThenInclude(x => x.Hardware)
                                        .FirstOrDefault(
                                        x => x.Id == tokenId &&
                                        x.Name == tokenSecret &&
                                        x.TaskInitiatorType == TaskInitiatorsTypes.Telegram &&
                                        x.TaskInitiatorId == update.message.from.id &&
                                        x.CreatedAt > x.FinishedAt);

                                    if (taskScript == null)
                                    {
                                        response_msg += "Токен подтверждения запуска команды не найден. Попробуйте заново запустить скрипт и воспользуйтесь новым токеном.";
                                    }
                                    else
                                    {
                                        taskScript.FinishedAt = taskScript.CreatedAt;
                                        db.SaveChanges();
                                        response_msg += $"Скрипт \"{taskScript.Script}\" запущен.";
                                    }
                                }
                            }
                            if (taskScript != null)
                            {
                                BackgroundWorker bw = new BackgroundWorker();
                                bw.DoWork += new DoWorkEventHandler(RunScriptAction);
                                bw.ProgressChanged += (object sender, ProgressChangedEventArgs e) => { };
                                bw.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => { };
                                bw.RunWorkerAsync(taskScript);

                                //Thread RunScriptThread = new Thread(RunScriptAction) { IsBackground = false };
                                //RunScriptThread.Start(taskScript);
                            }
                        }
                        else if (view_logs_regex.IsMatch(cmd))
                        {
                            Match match = view_logs_regex.Match(cmd);
                            int read_log_id_at = int.Parse(match.Groups[1].Value);
                            if (read_log_id_at == 0)
                            {
                                lock (LogsContext.DbLocker)
                                {
                                    using (LogsContext logs_db = new LogsContext())
                                    {
                                        read_log_id_at = logs_db.Logs.Max(x => x.Id);
                                    }
                                }
                            }
                            List<LogRowModel> logRows = null;
                            lock (LogsContext.DbLocker)
                            {
                                using (LogsContext logs_db = new LogsContext())
                                {
                                    logRows = logs_db.Logs.OrderByDescending(x => x.Id).Where(x => x.Id <= read_log_id_at).Take(10).ToList();
                                }
                            }

                            if (logRows == null || logRows.Count == 0)
                            {
                                response_msg += $"Нет доступных логов для просмотра!{System.Environment.NewLine}{System.Environment.NewLine}";
                            }
                            else
                            {
                                response_msg += $"Просмотр логов:{System.Environment.NewLine}{System.Environment.NewLine}";
                                logRows.ForEach(x => { response_msg += $"[{x.CreatedAt}]{x.Status}{System.Environment.NewLine}{x.Name}{System.Environment.NewLine}{System.Environment.NewLine}"; });

                                int next_at = logRows.Min(x => x.Id - 1);
                                lock (LogsContext.DbLocker)
                                {
                                    using (LogsContext logs_db = new LogsContext())
                                    {
                                        if (logs_db.Logs.Any(x => x.Id == next_at))
                                        {
                                            response_msg += $"/logs_at_{next_at} - далее";
                                        }
                                    }
                                }
                            }
                        }
                        else if (get_harware_regex.IsMatch(cmd))
                        {
                            Match m = get_harware_regex.Match(cmd);
                            HardwareModel hw = null;
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    hw = db.Hardwares.FirstOrDefault(x => x.Id == int.Parse(m.Groups[1].Value));
                                }
                            }

                            if (hw == null)
                            {
                                response_msg += $"Устройство не найдено в БД. id={m.Groups[1].Value}.{System.Environment.NewLine}";
                            }
                            else
                            {
                                HttpWebRequest request = new HttpWebRequest(new Uri($"http://{hw.Address}/{hw.Password}/?cmd=all"));
                                request.Timeout = 5000;

                                try
                                {
                                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                    {
                                        if (response.StatusCode == HttpStatusCode.OK)
                                        {
                                            string responseFromServer = string.Empty;

                                            using (Stream dataStream = response.GetResponseStream())
                                            using (StreamReader reader = new StreamReader(dataStream))
                                            {
                                                responseFromServer = reader.ReadToEnd();
                                            }
                                            if (string.IsNullOrEmpty(responseFromServer))
                                            {
                                                response_msg += "Пустой ответ устройства.";
                                            }
                                            else if (responseFromServer.ToLower() == "unauthorized")
                                            {
                                                response_msg += "Пароль, указаный в настройкх устройства не подошёл.";
                                            }
                                            else
                                            {
                                                string[] ports_array = responseFromServer.Split(";");
                                                if (ports_array.Length == 0)
                                                {
                                                    response_msg += responseFromServer;
                                                }
                                                else
                                                {
                                                    response_msg += $"Состояние портов:{System.Environment.NewLine}";
                                                    int port_num = 0;
                                                    PortModel portHardware = null;
                                                    foreach (string port_state in ports_array)
                                                    {
                                                        lock (DatabaseContext.DbLocker)
                                                        {
                                                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                                            {
                                                                portHardware = db.Ports.FirstOrDefault(x => x.HardwareId == hw.Id && x.PortNumb == port_num);
                                                            }
                                                        }

                                                        if (portHardware == null)
                                                        {
                                                            portHardware = new PortModel() { HardwareId = hw.Id, PortNumb = port_num };
                                                            lock (DatabaseContext.DbLocker)
                                                            {
                                                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                                                {
                                                                    db.Ports.Add(portHardware);
                                                                    db.SaveChanges();
                                                                }
                                                            }
                                                        }

                                                        string separator = System.Environment.NewLine;
                                                        if (string.IsNullOrWhiteSpace(portHardware.Name))
                                                        {
                                                            response_msg += $"P{port_num}";
                                                        }
                                                        else
                                                        {
                                                            response_msg += $"{System.Environment.NewLine}\"{portHardware.Name}\"(P{port_num})";
                                                            separator += System.Environment.NewLine;
                                                        }
                                                        response_msg += $" => {port_state}";
                                                        if (port_state.Length <= 3)
                                                        {
                                                            response_msg += $" - /port_{portHardware.Id}{separator}";
                                                        }
                                                        else
                                                        {
                                                            response_msg += $"{System.Environment.NewLine}/port_{portHardware.Id}{separator}";
                                                        }
                                                        port_num++;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            response_msg += $"Ошибка выполнения запроса. StatusCode:\"{response.StatusCode}\"; StatusDescription:\"{response.StatusDescription}\"";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    response_msg += $"Сбой обработки команды:{ex.Message}";
                                }
                            }
                        }
                        else if (get_port_regex.IsMatch(cmd))
                        {
                            Match m = get_port_regex.Match(cmd);
                            PortModel port_hw = null;

                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    port_hw = db.Ports.Include(x => x.Hardware).FirstOrDefault(x => x.Id == int.Parse(m.Groups[1].Value));
                                }
                            }

                            if (port_hw == null)
                            {
                                response_msg += $"Порт не найден в БД: id={m.Groups[1].Value}";
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(port_hw.Name))
                                {
                                    response_msg += $"Порт: P{port_hw.PortNumb}";
                                }
                                else
                                {
                                    response_msg += $"Порт: \"{port_hw.Name} (P{port_hw.PortNumb})\"";
                                }

                                response_msg += $" (устр-во:{port_hw.Hardware.Name} - /hw_{port_hw.Hardware.Id}).{System.Environment.NewLine}";
                                HttpWebRequest request = new HttpWebRequest(new Uri($"http://{port_hw.Hardware.Address}/{port_hw.Hardware.Password}/?pt={port_hw.PortNumb}&cmd=get"));
                                request.Timeout = 5000;

                                try
                                {
                                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                    {
                                        if (response.StatusCode == HttpStatusCode.OK)
                                        {
                                            string responseFromServer = string.Empty;

                                            using (Stream dataStream = response.GetResponseStream())
                                            using (StreamReader reader = new StreamReader(dataStream))
                                            {
                                                responseFromServer = reader.ReadToEnd();
                                            }
                                            if (string.IsNullOrEmpty(responseFromServer))
                                            {
                                                response_msg += "Пустой ответ устройства.";
                                            }
                                            else if (responseFromServer.ToLower() == "unauthorized")
                                            {
                                                response_msg += "Пароль, указаный в настройкх устройства не подошёл.";
                                            }
                                            else
                                            {
                                                response_msg += $"Состояние порта: {responseFromServer}; ";
                                                if (responseFromServer.ToLower() == "off" || responseFromServer.ToLower() == "on")
                                                {
                                                    response_msg += $"Включить: /port_{port_hw.Id}_set_ON; Выключить: /port_{port_hw.Id}_set_OFF; ";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            response_msg += $"Ошибка выполнения запроса. StatusCode:\"{response.StatusCode}\"; StatusDescription:\"{response.StatusDescription}\"";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    response_msg += $"Сбой обработки команды:{ex.Message}";
                                }
                            }

                        }
                        else if (set_port_regex.IsMatch(cmd))
                        {
                            if (!user.CommandsAllowed)
                            {
                                response_msg += "У вас нет прав для упрвления портми";
                                goto sendTelegramMessage;
                            }
                            Match m = set_port_regex.Match(cmd);
                            PortModel port_hw = null;
                            lock (DatabaseContext.DbLocker)
                            {
                                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                                {
                                    port_hw = db.Ports.Include(x => x.Hardware).FirstOrDefault(x => x.Id == int.Parse(m.Groups[1].Value));
                                }
                            }
                            if (port_hw == null)
                            {
                                response_msg += $"Порт не найден в БД: id={m.Groups[1].Value}";
                            }
                            else
                            {

                                if (!port_hw.Hardware.CommandsAllowed)
                                {
                                    response_msg = "Для устройства запрещены вызовы команд от удалённых пользователей";
                                    goto sendTelegramMessage;
                                }
                                string setter = m.Groups[2].Value;
                                if (setter == "off")
                                {
                                    response_msg += $"Попытка выключить порт: P{port_hw.PortNumb}; ";
                                    setter = "0";
                                }
                                else if (setter == "on")
                                {
                                    response_msg += $"Попытка включить порт: P{port_hw.PortNumb}; ";
                                    setter = "1";
                                }
                                else if (Regex.IsMatch(setter, @"^\d+$"))
                                {
                                    //response_msg += $"Попытка включить порт: P{port_hw.PortNumb}; ";
                                }
                                HttpWebRequest request = new HttpWebRequest(new Uri($"http://{port_hw.Hardware.Address}/{port_hw.Hardware.Password}/?cmd={port_hw.PortNumb}:{setter}"));
                                request.Timeout = 5000;

                                try
                                {
                                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                                    {
                                        if (response.StatusCode == HttpStatusCode.OK)
                                        {
                                            string responseFromServer = string.Empty;

                                            using (Stream dataStream = response.GetResponseStream())
                                            using (StreamReader reader = new StreamReader(dataStream))
                                            {
                                                responseFromServer = reader.ReadToEnd();
                                            }
                                            if (string.IsNullOrEmpty(responseFromServer))
                                            {
                                                response_msg += "Пустой ответ устройства.";
                                            }
                                            else if (responseFromServer.ToLower() == "unauthorized")
                                            {
                                                response_msg += "Пароль, указаный в настройкх устройства не подошёл.";
                                            }
                                            else
                                            {
                                                response_msg += $"{responseFromServer}; ";
                                            }
                                        }
                                        else
                                        {
                                            response_msg += $"Ошибка выполнения запроса. StatusCode:\"{response.StatusCode}\"; StatusDescription:\"{response.StatusDescription}\"";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    response_msg += $"Сбой обработки команды:{ex.Message}";
                                }
                            }

                        }
                        else
                        {
                            response_msg = $"Доступные команды:{System.Environment.NewLine}" +
                                $"/hardwares - список устройств{System.Environment.NewLine}";

                            if (user.CommandsAllowed)
                            {
                                response_msg += $"/scripts - список скриптов{System.Environment.NewLine}";
                            }
                            response_msg += "/logs_at_0 - просмотр логов";
                        }
                    sendTelegramMessage: telegramClient.sendMessage(update.message.from.id.ToString(), response_msg.Trim());
                    }

                    telegramClient.offset = updates.Max(x => x.update_id);
                }
                if (TelegramBotSurveyInterval > 0)
                {
                    Thread.Sleep(TelegramBotSurveyInterval * 1000);
                }
                else
                {
                    return;
                }
            }
        }

        public static void RunScriptAction(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            TaskModel task = (TaskModel)e.Argument;

            List<CommandModel> commands = task.Script.Commands;

            for (int i = 0; i < commands.Count; i++)
            {
                CommandModel command = commands[i];
                if (command.Hidden)
                {
                    continue;
                }
                if (command.PauseBeforeExecution > 0)
                {
                    Thread.Sleep((int)command.PauseBeforeExecution * 1000);
                }

                if (command.PortExecutionCondition != null)
                {
                    HttpWebRequest request = new HttpWebRequest(new Uri($"http://{command.PortExecutionCondition.Hardware.Address}/{command.PortExecutionCondition.Hardware.Password}/?pt={command.PortExecutionCondition.PortNumb}&cmd=get"));
                    request.Timeout = 5000;

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string responseFromServer = string.Empty;

                                using (Stream dataStream = response.GetResponseStream())
                                using (StreamReader reader = new StreamReader(dataStream))
                                {
                                    responseFromServer = reader.ReadToEnd();
                                }
                                if (string.IsNullOrEmpty(responseFromServer))
                                {

                                }
                                else if (responseFromServer.ToLower() == "unauthorized")
                                {

                                }
                                else
                                {
                                    if ((command.PortExecutionConditionAllowingState == true && responseFromServer.ToLower() != "on") ||
                                        (command.PortExecutionConditionAllowingState == false && responseFromServer.ToLower() != "off"))
                                    {
                                        continue;
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (command.TypeCommand == TypesCommands.Exit)
                {
                    if (command.Execution < 1)
                    {
                        CloseTask(task, "Завершено по команде выхода");
                        break;
                    }

                    int inc_index = commands.FindIndex(x => x.Id == command.Execution);
                    if (inc_index >= 0)
                    {
                        i = inc_index - 1;
                        continue;
                    }
                    else
                    {
                        command = null;
                        lock (DatabaseContext.DbLocker)
                        {
                            using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                            {
                                command = db.Commands.Include(x => x.Script).ThenInclude(x => x.Commands).FirstOrDefault(x => x.Id == command.Execution);
                            }
                        }
                        if (command == null)
                        {
                            CloseTask(task, $"Завершено из-за оибки. Команда перехода не найдена: {command.Execution}");
                            break;
                        }

                        i = commands.FindIndex(x => x.Id == command.Execution) - 1;
                        continue;
                    }
                }
                else if (command.TypeCommand == TypesCommands.Controller)
                {
                    HardwareModel hw = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            hw = db.Hardwares.FirstOrDefault(x => x.Id == command.Execution);
                        }
                    }

                    if (hw == null)
                    {
                        CloseTask(task, $"Завершено из-за оибки. Контроллер не найден в БД: {command.Execution}");
                        break;
                    }
                    else if (!hw.CommandsAllowed)
                    {

                    }
                    else
                    {
                        HttpWebRequest request = new HttpWebRequest(new Uri($"http://{hw.Address}/{hw.Password}/?cmd={command.ExecutionParametr}"));
                        request.Timeout = 5000;

                        try
                        {
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    string responseFromServer = string.Empty;

                                    using (Stream dataStream = response.GetResponseStream())
                                    using (StreamReader reader = new StreamReader(dataStream))
                                    {
                                        responseFromServer = reader.ReadToEnd();
                                    }
                                    if (string.IsNullOrEmpty(responseFromServer))
                                    {

                                    }
                                    else if (responseFromServer.ToLower() == "unauthorized")
                                    {

                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else if (command.TypeCommand == TypesCommands.Port)
                {
                    PortModel port = null;
                    lock (DatabaseContext.DbLocker)
                    {
                        using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                        {
                            port = db.Ports.Include(x => x.Hardware).FirstOrDefault(x => x.Id == command.Execution);
                        }
                    }

                    if (port == null)
                    {
                        CloseTask(task, $"Завершено из-за оибки. Порт не найден в БД: {command.Execution}");
                        break;
                    }

                    string command_url = $"http://{port.Hardware.Address}/{port.Hardware.Password}/?cmd={port.PortNumb}:";

                    if (command.ExecutionParametr.ToLower() == "true")
                    {
                        command_url += "1";
                    }
                    else if (command.ExecutionParametr.ToLower() == "false")
                    {
                        command_url += "0";
                    }
                    else
                    {
                        command_url += "2";
                    }
                    HttpWebRequest request = new HttpWebRequest(new Uri(command_url));
                    request.Timeout = 5000;

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string responseFromServer = string.Empty;

                                using (Stream dataStream = response.GetResponseStream())
                                using (StreamReader reader = new StreamReader(dataStream))
                                {
                                    responseFromServer = reader.ReadToEnd();
                                }
                                if (string.IsNullOrEmpty(responseFromServer))
                                {

                                }
                                else if (responseFromServer.ToLower() == "unauthorized")
                                {

                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            CloseTask(task, "Скрипт выполнен");
        }

        private static void CloseTask(TaskModel task, string name)
        {
            if (task == null)
                return;

            if (name != null)
            {
                task.Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name;
            }

            lock (DatabaseContext.DbLocker)
            {
                using (DatabaseContext db = new DatabaseContext(gs.DatabasePathBase))
                {
                    task.FinishedAt = DateTime.Now;
                    db.Tasks.Update(task);
                    db.SaveChanges();
                }
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override bool OnUnbind(Intent intent)
        {
            Log.Debug(TAG, "OnUnbind");
            return base.OnUnbind(intent);
        }

        public override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy");
            StopForegroundService();
            Binder = null;
            ForegroundServiceManager = null;
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Debug(TAG, $"OnStartCommand - start id: {startId}");

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            int listener_port = Preferences.Get(base.Resources.GetResourceEntryName(Resource.Id.service_port), 8080);

            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStartedForegroundService)
                {
                    Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_already_running_title)}");
                }
                else
                {
                    TelegramBotSurveyInterval = intent.GetIntExtra(Constants.TELEGRAM_BOT_SURVEY_INTERVAL, -1);
                    TelegramBotToken = intent.GetStringExtra(Constants.TELEGRAM_BOT_TOKEN);

                    Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_being_started_title)}");
                    RegisterForegroundService(ipAddress + ":" + listener_port);
                    StartForegroundService(listener_port);
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.the_service_is_stopped_title)}");
                StopForegroundService();
                StopForeground(true);
                StopSelf();
            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_SERVICE))
            {
                Log.Info(TAG, $"OnStartCommand: {GetText(Resource.String.restarting_the_service_title)}");
                StopForegroundService();
                StartForegroundService(listener_port);
            }

            return StartCommandResult.Sticky;
        }

        private void RegisterForegroundService(string content_text)
        {
            Log.Info(TAG, $"RegisterForegroundService: {content_text}");
            var notification = new Notification.Builder(this)
                .SetContentTitle(base.Resources.GetString(Resource.String.app_name))
                .SetContentText(content_text)
                .SetSmallIcon(Resource.Drawable.ic_stat_name)
                .SetContentIntent(BuildIntentToShowServicesActivity())
                .SetOngoing(true)
                .Build();

            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        private PendingIntent BuildIntentToShowServicesActivity()
        {
            Log.Debug(TAG, "BuildIntentToShowServicesActivity()");

            Intent notificationIntent = new Intent(this, typeof(ServicesActivity));
            notificationIntent.SetAction(Constants.ACTION_SERVICE_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        public void StartForegroundService(int foreground_service_port)
        {
            string msg = $"StartForegroundService(port={foreground_service_port})";
            Log.Debug(TAG, msg);

            if (ForegroundServiceManager == null)
            {
                msg = "Error starting foreground service: ForegroundServiceManager == null";
                Log.Error(TAG, msg);
                logsDB.AddLogRow(LogStatusesEnum.Error, msg, TAG);
                return;
            }
            ForegroundServiceManager.StartForegroundService(foreground_service_port);

            TelegramBotClientThread?.Abort();
            TelegramBotClientThread = new Thread(TelegramBotClientAction);
            TelegramBotClientThread.Start();
        }

        public void StopForegroundService()
        {
            string msg = "StopForegroundService()";
            Log.Info(TAG, msg);

            TelegramBotSurveyInterval = -1;
            TelegramBotClientThread.Abort();

            if (ForegroundServiceManager == null)
            {
                msg = "Error stopping foreground service: ForegroundServiceManager == null";
                Log.Error(TAG, msg);
                logsDB.AddLogRow(LogStatusesEnum.Error, msg, TAG);
                return;
            }
            ForegroundServiceManager.StopForegroundService();
        }
    }
}