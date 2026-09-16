using MaterialDesignThemes.Wpf;
using Notifications.Wpf;

namespace eSSLAttendanceDesktopClient.Utility
{
    public static class SnackbarService
    {
        public static SnackbarMessageQueue GlobalMessageQueue { get; set; } = new SnackbarMessageQueue();
        public static string MessageType { get; set; } = "Error";
        
        public static void ShowError(string message)
        {
            MessageType = "Error";
            GlobalMessageQueue.Enqueue($"❌ Error: {message}");
        }
        public static void ShowSuccess(string message)
        {
            MessageType = "Success";
            GlobalMessageQueue.Enqueue($"✅ Success: {message}");
        }
        public static void ShowWarning(string message)
        {
            MessageType = "Warning";
            GlobalMessageQueue.Enqueue($"⚠ Warning: {message}");
        }

        public static void DisplayErrorMessage(string message)
        {
            var notificationManager = new NotificationManager();
            var notificationContent = new NotificationContent
            {
                Title = "Error",
                Message = message,
                Type = NotificationType.Error,
            };
            notificationManager.Show(notificationContent);
        }
        public static void DisplayWarningMessage(string message)
        {
            var notificationManager = new NotificationManager();
            var notificationContent = new NotificationContent
            {
                Title = "Warning",
                Message = message,
                Type = NotificationType.Warning,
            };
            notificationManager.Show(notificationContent);
        }
        public static void DisplaySuccessMessage(string message)
        {
            var notificationManager = new NotificationManager();
            var notificationContent = new NotificationContent
            {
                Title = "Success",
                Message = message,
                Type = NotificationType.Success,
            };
            notificationManager.Show(notificationContent);
        }
        public static void DisplayInfoMessage(string message)
        {
            var notificationManager = new NotificationManager();
            var notificationContent = new NotificationContent
            {
                Title = "Information",
                Message = message,
                Type = NotificationType.Information,
            };
            notificationManager.Show(notificationContent);
        }
    }
}
