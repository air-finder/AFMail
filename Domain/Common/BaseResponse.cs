namespace Domain.Common;

public abstract class BaseResponse<T>(T? result = null)
    where T : class
{
    public List<string> Errors { get; set; } = [];
    public bool Success => Errors.Count == 0;
    public T? Result { get; set; } = result;

    public void AddError(string error)
        => Errors.Add(error);
}