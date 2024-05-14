namespace Domain.SeedWork.Notification
{
    public static class NotificationsWrapper
    {
        private static INotification GetContainer()
            => ServiceLocator.Container!.GetService<INotification>();

        public static void AddNotification(string message)
            => GetContainer().AddNotification(message);

        public static bool HasNotification()
            => GetContainer().HasNotification;
    }
}
