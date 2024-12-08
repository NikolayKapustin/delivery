using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services.DispatchService;

/// <inheritdoc />
public class DispatchService : IDispatchService
{
    /// <inheritdoc />
    public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
    {
        if (order is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(order));
        }

        if (couriers is null || couriers.Count == 0)
        {
            return GeneralErrors.ValueIsRequired(nameof(couriers));
        }
        
        Courier nearestCourier = couriers.First();
        
        foreach (var courier in couriers)
        {
            if (Equals(nearestCourier, courier))
            {
                continue;
            }

            if (courier.CalculateTimeToPoint(order.Location).Value < nearestCourier.CalculateTimeToPoint(order.Location).Value)
            {
                nearestCourier = courier;
            }
        }
        
        return nearestCourier;
    }
}