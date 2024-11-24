using System;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate;

public class OrderShould
{
    [Fact]
    public void CanBeCreatedWhenParamsIsCorrect()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;

        //Act
        var createOrderResult = Order.Create(orderId, location);

        //Assert
        createOrderResult.IsSuccess.Should().BeTrue();
        createOrderResult.Value.Id.Should().NotBeEmpty();
        createOrderResult.Value.Location.Should().Be(location);
        createOrderResult.Value.Status.Should().Be(OrderStatus.Created);
    }

    [Fact]
    public void CanBeAssigned()
    {
        //Arrange
        var courier = Courier.Create("courier", Transport.Bicycle, Location.Create(1, 1).Value).Value;
        var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value).Value;

        //Act
        var assignResult = order.Assign(courier);

        //Assert
        assignResult.IsSuccess.Should().BeTrue();
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
    }
}