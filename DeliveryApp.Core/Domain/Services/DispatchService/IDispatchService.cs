using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services.DispatchService;

/// <summary>
/// Сервис поиска самого подходящего курьера для заказа
/// </summary>
public interface IDispatchService
{
   /// <summary>
   /// Метод делает скоринг и возвращает победившего курьера
   /// </summary>
   /// <param name="order">Заказ</param>
   /// <param name="couriers">Список курьеров</param>
   /// <returns>Победивший курьер</returns>
   public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers);
}