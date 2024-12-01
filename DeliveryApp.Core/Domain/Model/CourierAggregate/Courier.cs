using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

/// <summary>
/// Курьер
/// </summary>
public class Courier : Aggregate
{
    /// <summary>
    /// Ctr
    /// </summary>
    private Courier()
    {
    }

    private Courier(string name, Transport transport, Location location) : base(Guid.NewGuid())
    {
        Name = name;
        Transport = transport;
        Location = location;
        Status = CourierStatus.Free;
    }

    /// <summary>
    /// Имя
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Транспорт
    /// </summary>
    public Transport Transport { get; private set; }

    /// <summary>
    /// Текущее местоположение
    /// </summary>
    public Location Location { get; private set; }

    /// <summary>
    /// Статус
    /// </summary>
    public CourierStatus Status { get; private set; }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Имя курьера</param>
    /// <param name="transport">Транспорт</param>
    /// <param name="location">Местоположение</param>
    /// <returns>Результат</returns>
    public static Result<Courier, Error> Create(string name, Transport transport, Location location)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return GeneralErrors.ValueIsRequired(nameof(name));
        }

        if (transport is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(transport));
        }

        if (location is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(location));
        }

        return new Courier(name, transport, location);
    }

    /// <summary>
    /// Установить статус занят
    /// </summary>
    /// <returns>Результат</returns>
    public Result SetBusy()
    {
        Status = CourierStatus.Busy;
        return Result.Success();
    }

    /// <summary>
    /// Установить статус свободен
    /// </summary>
    /// <returns>Результат</returns>
    public Result SetFree()
    {
        Status = CourierStatus.Free;
        return Result.Success();
    }

    /// <summary>
    /// Количество шагов до местоположения назначения
    /// </summary>
    /// <param name="targetLocation">Местоположение назначения</param>
    /// <returns>Результат</returns>
    public Result<double, Error> CalculateTimeToPoint(Location targetLocation)
    {
        if (targetLocation is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(targetLocation));
        }

        var distance = Math.Abs(targetLocation.X - Location.X) + Math.Abs(targetLocation.Y - Location.Y);
        return distance / Transport.Speed + (distance % Transport.Speed > 0 ? 1 : 0);
    }

    /// <summary>
    /// Переместить курьера на один шаг в сторону назначения
    /// </summary>
    /// <param name="targetLocation">Местоположение назначения</param>
    /// <returns></returns>
    public UnitResult<Error> Move(Location targetLocation)
    {
        int newX;
        int newY;

        if (targetLocation is null)
        {
            return GeneralErrors.ValueIsRequired(nameof(targetLocation));
        }

        var distanceX = Math.Abs(targetLocation.X - Location.X);
        if (distanceX >= Transport.Speed)
        {
            if (targetLocation.X > Location.X)
            {
                newX = Location.X + Transport.Speed;
            }
            else
            {
                newX = Location.X - Transport.Speed;
            }

            newY = Location.Y;
        }
        else
        {
            newX = targetLocation.X;
            var distanceY = Math.Abs(targetLocation.Y - Location.Y);
            if (distanceY >= Transport.Speed)
            {
                if (targetLocation.Y > Location.Y)
                {
                    newY = Location.Y + (Transport.Speed - distanceX);
                }
                else
                {
                    newY = Location.Y - (Transport.Speed - distanceX);
                }
            }
            else
            {
                newY = targetLocation.Y;
            }
        }

        Location = Location.Create(newX, newY).Value;

        return UnitResult.Success<Error>();
    }
}