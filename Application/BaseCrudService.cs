using Domain;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class BaseCrudService<T>(IBaseRepository<T> baseRepository) : IBaseCrudService<T> where T : class
{
    public async Task<BaseResponse<T>> GetByIdAsync(Guid id)
    {
        var response = await baseRepository.GetByIDAsync(id);
        return new GenericResponse<T>(response);
    }

    public async Task<BaseResponse<IEnumerable<T>>> GetListAsync(int pageIndex, int pageSize)
    {
        return new GenericResponse<IEnumerable<T>>(await baseRepository.GetAll().AsNoTracking().Skip(pageIndex * pageSize).Take(pageSize).ToListAsync());
    }

    public async Task<BaseResponse<T>> CreateAsync(T entity)
    {
        await baseRepository.InsertWithSaveChangesAsync(entity);
        return new GenericResponse<T>(entity);
    }

    public async Task<BaseResponse<T>> UpdateAsync(T entity)
    {
        await baseRepository.UpdateWithSaveChangesAsync(entity);
        return new GenericResponse<T>(entity);
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        await baseRepository.DeleteAsync(id);
        return new GenericResponse<object>(null);
    }
}