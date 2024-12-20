using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Xellarium.BusinessLogic.Models;
using Xellarium.BusinessLogic.Repository;
using Xellarium.DataAccess.Models;
using Xellarium.Tracing;

namespace Xellarium.DataAccess.Repository;

public abstract class GenericRepository<T>(XellariumContext context, ILogger logger) : IGenericRepository<T>
    where T : BaseModel
{
    protected readonly XellariumContext _context = context;

    public async Task<IEnumerable<T>> GetAll(bool includeDeleted = false)
    {
        using var activity = XellariumTracing.StartActivity();
        return includeDeleted
            ? await _context.Set<T>().ToListAsync()
            : await _context.Set<T>().Where(e => !e.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllByIds(IEnumerable<int> ids, bool includeDeleted = false)
    {
        using var activity = XellariumTracing.StartActivity();
        var idSet = ids.ToHashSet();
        return await _context.Set<T>().Where(e => (includeDeleted || !e.IsDeleted) && idSet.Contains(e.Id)).ToListAsync();
    }

    public async Task<T?> Get(int id, bool includeDeleted = false)
    {
        using var activity = XellariumTracing.StartActivity();
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null) return null;
        if (includeDeleted) return entity;
        return entity.IsDeleted == false ? entity : null;
    }

    public async Task Add(T entity)
    {
        using var activity = XellariumTracing.StartActivity();
        entity.MarkCreated();
        await _context.Set<T>().AddAsync(entity);
    }

    public async Task Update(T entity)
    {
        using var activity = XellariumTracing.StartActivity();
        entity.MarkUpdated();
        _context.Set<T>().Update(entity);
    }

    public async Task SoftDelete(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        var entity = await Get(id);
        if (entity == null) return;
        entity.Delete();
        await Update(entity);
    }
    
    public async Task HardDelete(int id)
    {
        using var activity = XellariumTracing.StartActivity();
        var entity = await Get(id);
        if (entity == null) return;
        _context.Set<T>().Remove(entity);
    }

    public Task<bool> Exists(int id, bool includeDeleted = false)
    {
        using var activity = XellariumTracing.StartActivity();
        return includeDeleted
            ? _context.Set<T>().AnyAsync(e => e.Id == id)
            : _context.Set<T>().Where(e => !e.IsDeleted).AnyAsync(e => e.Id == id);
    }
}