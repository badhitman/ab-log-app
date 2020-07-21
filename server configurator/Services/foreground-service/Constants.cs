namespace ab.Services
{
    public static class Constants
    {
        public const int DELAY_BETWEEN_LOG_MESSAGES = 5000; // milliseconds
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        public const string SERVICE_STARTED_KEY = "has_service_been_started";
        public const string BROADCAST_MESSAGE_KEY = "broadcast_message";
        public const string NOTIFICATION_BROADCAST_ACTION = "ab.Notification.Action";

        public const string ACTION_START_SERVICE = "ab.action.START_SERVICE";
        public const string ACTION_STOP_SERVICE = "ab.action.STOP_SERVICE";
        public const string ACTION_RESTART_SERVICE = "ab.action.RESTART_SERVICE";
        public const string ACTION_SERVICE_ACTIVITY = "ab.action.SERVICE_ACTIVITY";

        public const string TELEGRAM_BOT_TOKEN = "ab.action.TELEGRAM_BOT_TOKEN";
        public const string TELEGRAM_BOT_SURVEY_INTERVAL = "ab.action.TELEGRAM_BOT_SURVEY_INTERVAL";
        public const string TELEGRAM_BOT_ID = "ab.action.TELEGRAM_BOT_ID";
    }
}