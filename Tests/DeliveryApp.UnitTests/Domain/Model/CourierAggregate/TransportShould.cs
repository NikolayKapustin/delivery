using System.Linq;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate;

public class TransportShould
{
    [Fact]
    public void ReturnCorrectEntity()
    {
        //Arrange
        
        //Act
        var pedestrian = Transport.Pedestrian;
        var bicycle = Transport.Bicycle;
        var car = Transport.Car;
        
        //Assert
        pedestrian.Id.Should().Be(1);
        pedestrian.Name.Should().Be("pedestrian");
        pedestrian.Speed.Should().Be(1);
        
        bicycle.Id.Should().Be(2);
        bicycle.Name.Should().Be("bicycle");
        bicycle.Speed.Should().Be(2);
        
        car.Id.Should().Be(3);
        car.Name.Should().Be("car");
        car.Speed.Should().Be(3);
    }

    [Theory]
    [InlineData("pedestrian", 1)]
    [InlineData("bicycle", 2)]
    [InlineData("car", 3)]
    public void CanBeFoundByName(string name, int id)
    {
        //Arrange
        
        //Act
        var transport = Transport.FromName(name);
        
        //Assert
        transport.IsSuccess.Should().BeTrue();
        transport.Value.Name.Should().Be(name);
        transport.Value.Id.Should().Be(id);
    }

    [Fact]
    public void ReturnErrorWhenNotFoundByName()
    {
        //Arrange
        var wrongName = "wrong-name";
        
        //Act
        var transport = Transport.FromName(wrongName);
        
        //Assert
        transport.IsSuccess.Should().BeFalse();
        transport.Error.Should().NotBeNull();
    }
    
    [Theory]
    [InlineData("pedestrian", 1)]
    [InlineData("bicycle", 2)]
    [InlineData("car", 3)]
    public void CanBeFoundById(string name, int id)
    {
        //Arrange
        
        //Act
        var transport = Transport.FromId(id);
        
        //Assert
        transport.IsSuccess.Should().BeTrue();
        transport.Value.Name.Should().Be(name);
        transport.Value.Id.Should().Be(id);
    }

    [Fact]
    public void ReturnErrorWhenNotFoundById()
    {
        //Arrange
        var wrongId = 4;
        
        //Act
        var transport = Transport.FromId(wrongId);
        
        //Assert
        transport.IsSuccess.Should().BeFalse();
        transport.Error.Should().NotBeNull();
    }
    
    
    [Fact]
    public void ReturnListOfTransports()
    {
        //Arrange

        //Act
        var transports = Transport.List();

        //Assert
        transports.Should().NotBeEmpty();
        transports.Count().Should().Be(3);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void BeEqualWhenIdsIsEqual(int id1, int id2)
    {
        //Arrange
        var transport1 = Transport.FromId(id1);
        var transport2 = Transport.FromId(id2);
        
        //Act
        var isEqual = Equals(transport1, transport2);
        
        //Assert
        isEqual.Should().BeTrue();
    }
    
    [Fact]
    public void BeNotEqualWhenIdsIsNotEqual()
    {
        //Arrange
        var transport1 = Transport.FromId(1);
        var transport2 = Transport.FromId(2);
        
        //Act
        var isEqual = Equals(transport1, transport2);
        
        //Assert
        isEqual.Should().BeFalse();
    }
}