using System.Data;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class CourierRepository : ICourierRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public CourierRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;   
    }
    
    public async Task AddAsync(Courier courier)
    {
        if (courier.Status != null)
        {
            _dbContext.Attach(courier.Status);
        }
        if (courier.Transport != null)
        {
            _dbContext.Attach(courier.Transport);
        }
        await _dbContext.Couriers.AddAsync(courier);
    }

    public void Update(Courier courier)
    {
        if (courier.Status != null)
        {
            _dbContext.Attach(courier.Status);
        }
        if (courier.Transport != null)
        {
            _dbContext.Attach(courier.Transport);
        }
        _dbContext.Couriers.Update(courier);
    }

    public async Task<Courier> GetByIdAsync(Guid id)
    {
        return await _dbContext.Couriers.FindAsync(id);
    }

    public async Task<IEnumerable<Courier>> GetAllReadyCouriersAsync()
    {
        return await _dbContext.Couriers.Where(courier => courier.Status == CourierStatus.Free)
            .ToListAsync();
    }
}