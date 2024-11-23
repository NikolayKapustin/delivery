using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DeliveryApp.Core.Domain.Model.SharedKernel;

/// <summary>
/// Координаты на доске
/// </summary>
public class Location : ValueObject
{
    /// <summary>
    /// Случайная координата
    /// </summary>
    public static Location Random
    {
        get
        {
            var random = new Random();
            return new Location(random.Next(1,10), random.Next(1,10));
        }
    }
        
    /// <summary>
    /// Ctr
    /// </summary>
    [ExcludeFromCodeCoverage]
    private Location()
    {
    }

    /// <summary>
    /// Ctr
    /// </summary>
    /// <param name="x">Горизонталь</param>
    /// <param name="y">Вертикаль</param>
    private Location(int x, int y) : this()
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Горизонталь
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Вертикаль
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Factory Method
    /// </summary>
    /// <param name="x">Горизонталь</param>
    /// <param name="y">Вертикаль</param>
    /// <returns>Результат</returns>
    public static Result<Location, Error> Create(int x, int y)
    {
        if (x is < 1 or > 10) return GeneralErrors.ValueIsInvalid(nameof(x));
        if (y is < 1 or > 10) return GeneralErrors.ValueIsInvalid(nameof(y));
       
        return new Location(x,y);
    }

    /// <summary>
    /// Расстояние до другой координаты
    /// </summary>
    /// <param name="otherLocation">Другая координата</param>
    /// <returns>Результат</returns>
    public Result<int, Error> DistanceTo(Location otherLocation)
    {
        return Math.Abs(X - otherLocation.X) + Math.Abs(Y - otherLocation.Y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
    }
}