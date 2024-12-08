using System;
using System.Collections.Generic;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services.DispatchService;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services;

public class DispatchServiceShould
{
    [Theory]
    [ClassData(typeof(DispatchTestData))]
    public void Dispatch(Order order, List<Courier> couriers, int estimateDistanceToCourier)
    {
        //Arrange
        var dispatchService = new DispatchService();

        //Act
        var dispatchResult = dispatchService.Dispatch(order, couriers);

        //Assert
        dispatchResult.IsSuccess.Should().BeTrue();
        dispatchResult.Value.Should().NotBeNull();
        dispatchResult.Value.CalculateTimeToPoint(order.Location).Value.Should().Be(estimateDistanceToCourier);
    }
    
    
    private class DispatchTestData : TheoryData<Order, List<Courier>, int>
    {
        public DispatchTestData()
        {
            Add(Order.Create(Guid.NewGuid(), Location.Create(5,5).Value).Value, [
                Courier.Create("courier1", Transport.Pedestrian, Location.Create(4, 4).Value).Value,
                Courier.Create("courier2", Transport.Pedestrian, Location.Create(6, 6).Value).Value
            ], 2);
            Add(Order.Create(Guid.NewGuid(), Location.Create(5,5).Value).Value, [
                Courier.Create("courier1", Transport.Pedestrian, Location.Create(6, 6).Value).Value,
                Courier.Create("courier2", Transport.Pedestrian, Location.Create(4, 4).Value).Value
            ], 2);
            Add(Order.Create(Guid.NewGuid(), Location.Create(5,5).Value).Value, [
                Courier.Create("courier1", Transport.Pedestrian, Location.Create(6, 6).Value).Value,
                Courier.Create("courier2", Transport.Pedestrian, Location.Create(5, 5).Value).Value
            ], 0);
            Add(Order.Create(Guid.NewGuid(), Location.Create(5,5).Value).Value, [
                Courier.Create("courier1", Transport.Pedestrian, Location.Create(4, 4).Value).Value,
                Courier.Create("courier2", Transport.Pedestrian, Location.Create(7, 7).Value).Value
            ], 2);
            Add(Order.Create(Guid.NewGuid(), Location.Create(5,5).Value).Value, [
                Courier.Create("courier1", Transport.Pedestrian, Location.Create(4, 4).Value).Value,
                Courier.Create("courier2", Transport.Bicycle, Location.Create(4, 4).Value).Value
            ], 1);
        }
    };
}