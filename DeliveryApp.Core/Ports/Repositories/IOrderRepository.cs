using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports.Repositories;

/// <summary>
/// Репозиторий закаов
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Добавить заказ
    /// </summary>
    /// <param name="order">Заказ</param>
    Task AddAsync(Order order);
    
    /// <summary>
    /// Обновить заказ
    /// </summary>
    /// <param name="order">Заказ</param>
    void Update(Order order);
    
    /// <summary>
    /// Получить заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>Заказ</returns>
    Task<Order> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Получить все заказы в статусе Created
    /// </summary>
    /// <returns>Заказы</returns>
    Task<ICollection<Order>> GetAllNewOrdersAsync();
    
    /// <summary>
    /// Получить все заказы в статусе Assigned
    /// </summary>
    /// <returns>Заказы</returns>
    Task<ICollection<Order>> GetAllAssignedOrdersAsync();
}