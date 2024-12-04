using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports.Repositories;

/// <summary>
/// Репозиторий курьеров 
/// </summary>
public interface ICourierRepository : IRepository<Courier>
{
    /// <summary>
    /// Добавить курьера
    /// </summary>
    /// <param name="courier">Курьер</param>
    Task AddAsync(Courier courier);
    
    /// <summary>
    /// Обновить курьера
    /// </summary>
    /// <param name="courier">Курьер</param>
    void Update(Courier courier);
    
    /// <summary>
    /// Поучить курьера по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <returns>Курьер</returns>
    Task<Courier> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Получить всех свободных курьеров
    /// </summary>
    /// <returns>Курьеры</returns>
    Task<IEnumerable<Courier>> GetAllReadyCouriersAsync();
}