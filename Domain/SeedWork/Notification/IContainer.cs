namespace Domain.SeedWork.Notification
{
    public interface IContainer
    {
        T GetService<T>();
    }
}
