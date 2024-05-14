namespace Domain.SeedWork.Notification
{
    public class ServiceLocator
    {
        public static IContainer? Container { get; set; }
        public static void Initialize(IContainer container) => Container = container;
    }
}   
