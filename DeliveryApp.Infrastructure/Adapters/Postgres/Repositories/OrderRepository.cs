using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports.Repositories;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(Order order)
    {
        if (order.Status != null)
        {
            _dbContext.Attach(order.Status);
        }

        await _dbContext.Orders.AddAsync(order);
    }

    public void Update(Order order)
    {
        if (order.Status != null)
        {
            _dbContext.Attach(order.Status);
        }

        _dbContext.Orders.Update(order);
    }

    public async Task<Order> GetByIdAsync(Guid id)
    {
        return await _dbContext.Orders.SingleOrDefaultAsync(oder => oder.Id == id);
    }

    public async Task<ICollection<Order>> GetAllNewOrdersAsync()
    {
        return await _dbContext.Orders.Where(order => order.Status == OrderStatus.Created)
            .ToListAsync();
    }

    public async Task<ICollection<Order>> GetAllAssignedOrdersAsync()
    {
        return await _dbContext.Orders.Where(order => order.Status == OrderStatus.Assigned)
            .ToListAsync();
    }
}