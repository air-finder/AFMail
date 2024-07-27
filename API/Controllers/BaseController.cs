using Application;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[Route("api/[controller]")]
public abstract class BaseController<T>(IBaseCrudService<T> baseService) : Controller where T : class
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        => Ok(await baseService.GetByIdAsync(id));

    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] int pageIndex, int pageSize)
        => Ok(await baseService.GetListAsync(pageIndex, pageSize));

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] T request)
        => Ok(await baseService.CreateAsync(request));

    [HttpPut]
    public async Task<IActionResult> PutAsync([FromBody] T request)
        => Ok(await baseService.UpdateAsync(request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        => Ok(await baseService.DeleteAsync(id));
}