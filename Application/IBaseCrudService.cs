using Domain.Common;

namespace Application;

public interface IBaseCrudService<T> where T : class
{
    public Task<BaseResponse<T>> GetByIdAsync(Guid id);
    public Task<BaseResponse<IEnumerable<T>>> GetListAsync(int pageIndex, int pageSize);
    public Task<BaseResponse<T>> CreateAsync(T entity);
    public Task<BaseResponse<T>> UpdateAsync(T entity);
    public Task<BaseResponse<object>> DeleteAsync(Guid id);
}