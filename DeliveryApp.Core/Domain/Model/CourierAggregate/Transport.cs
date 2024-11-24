using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.CourierAggregate;

/// <summary>
/// Транспорт
/// </summary>
public class Transport : Entity<int>
{
    public static readonly Transport Pedestrian = new(1, nameof(Pedestrian).ToLowerInvariant(), 1);
    public static readonly Transport Bicycle = new(2, nameof(Bicycle).ToLowerInvariant(), 2);
    public static readonly Transport Car = new(3, nameof(Car).ToLowerInvariant(), 3);

    /// <summary>
    /// Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private Transport()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="name">Наименование</param>
    /// <param name="speed">Скорость</param>
    private Transport(int id, string name, int speed) : this()
    {
        Id = id;
        Name = name;
        Speed = speed;
    }

    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Скорость
    /// </summary>
    public int Speed { get; private set; }

    /// <summary>
    ///     Список всех значений списка
    /// </summary>
    /// <returns>Значения списка</returns>
    public static IEnumerable<Transport> List()
    {
        yield return Pedestrian;
        yield return Bicycle;
        yield return Car;
    }

    /// <summary>
    /// Получить транспорт по наименованию
    /// </summary>
    /// <param name="name">Наименование</param>
    /// <returns>Транспорт</returns>
    public static Result<Transport, Error> FromName(string name)
    {
        var transport = List().SingleOrDefault(transport =>
            string.Equals(transport.Name, name, StringComparison.CurrentCultureIgnoreCase));
        if (transport is null)
        {
            return Errors.NameIsWrong(name);
        }

        return transport;
    }

    /// <summary>
    /// Получить транспорт по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <returns>Транспорт</returns>
    public static Result<Transport, Error> FromId(int id)
    {
        var transport = List().SingleOrDefault(transport => transport.Id == id);
        if (transport is null)
        {
            return Errors.IdIsWrong(id);
        }

        return transport;
    }

    /// <summary>
    /// Ошибки, которые может возвращать сущность
    /// </summary>
    public static class Errors
    {
        public static Error NameIsWrong(string name)
        {
            return new Error($"{nameof(Transport).ToLowerInvariant()}.name.is.wrong",
                $"Некорректное наименование '{name}'." +
                $" Допустимые значения: {nameof(Transport).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Name))}");
        }

        public static Error IdIsWrong(int id)
        {
            return new Error($"{nameof(Transport).ToLowerInvariant()}.id.is.wrong",
                $"Некорректный идентификатор '{id}'." +
                $" Допустимые значения: {nameof(Transport).ToLowerInvariant()}: {string.Join(",", List().Select(s => s.Id))}");
        }
    }
}