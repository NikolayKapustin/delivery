using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate;

public class Order : Aggregate
{
    /// <summary>
    /// Ctr
    /// </summary>
    private Order()
    {
        
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="orderId">Идентификатор</param>
    /// <param name="location">Местоположение, куда нужно доставить</param>
    private Order(Guid orderId, Location location) : base(orderId)
    {
        Location = location;
        Status = OrderStatus.Created;
    }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="orderId">Идентификатор</param>
    /// <param name="location">Местоположение, куда нужно доставить</param>
    /// <returns>Результат</returns>
    public static Result<Order, Error> Create(Guid orderId, Location location)
    {
        if (orderId == Guid.Empty)
        {
            return GeneralErrors.ValueIsRequired(nameof(orderId));
        }

        if (location is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(location));
        }
        
        return new Order(orderId, location);
    }
    
    /// <summary>
    /// Местоположение, куда нужно доставить
    /// </summary>
    public Location Location { get; private set; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Идентификатор назначенного курьера
    /// </summary>
    public Guid? CourierId { get; private set; }

    /// <summary>
    /// Назначить заказ на курьера
    /// </summary>
    /// <param name="courier">Курьер</param>
    /// <returns>Результат</returns>
    public UnitResult<Error> Assign(Courier courier)
    {
        if (courier is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(courier));
        }
        
        CourierId = courier.Id;
        Status = OrderStatus.Assigned;
        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Complete()
    {
        if (Status != OrderStatus.Assigned)
        {
            return Errors.InvalidStatus(Status);
        }
        
        Status = OrderStatus.Completed;
        return UnitResult.Success<Error>();
    }
    
    /// <summary>
    /// Ошибки, которые может возвращать сущность
    /// </summary>
    public static class Errors
    {
        public static Error InvalidStatus(OrderStatus status)
        {
            return new Error($"{nameof(Order).ToLowerInvariant()}.{nameof(Status).ToLowerInvariant()}.is.wrong",
                $"Некорректный идентификатор '{status.Name}'." +
                $" Завершить можно только заказ со статусом '{OrderStatus.Assigned.Name}'");
        }
    }
}