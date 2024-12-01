using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate;

public class CourierShould
{
    [Fact]
    public void CanBeCreatedWhenParamsIsCorrect()
    {
        //Arrange
        var name = "name";
        var transport = Transport.Bicycle;
        var location = Location.Create(1, 1).Value;

        //Act
        var createCourierResult = Courier.Create(name, transport, location);

        //Assert
        createCourierResult.IsSuccess.Should().BeTrue();
        createCourierResult.Value.Id.Should().NotBeEmpty();
        createCourierResult.Value.Name.Should().Be(name);
        createCourierResult.Value.Transport.Should().Be(transport);
        createCourierResult.Value.Location.Should().Be(location);
        createCourierResult.Value.Status.Should().Be(CourierStatus.Free);
    }

    [Fact]
    public void CanSetStatusBusy()
    {
        //Arrange
        var courier = Courier.Create("name", Transport.Bicycle, Location.Create(1, 1).Value).Value;

        //Act
        var setBusyResult = courier.SetBusy();

        //Assert
        setBusyResult.IsSuccess.Should().BeTrue();
        courier.Status.Should().Be(CourierStatus.Busy);
    }

    [Fact]
    public void CanSetStatusFree()
    {
        //Arrange
        var courier = Courier.Create("name", Transport.Bicycle, Location.Create(1, 1).Value).Value;

        //Act
        var setBusyResult = courier.SetFree();

        //Assert
        setBusyResult.IsSuccess.Should().BeTrue();
        courier.Status.Should().Be(CourierStatus.Free);
    }

    [Theory]
    [ClassData(typeof(CalculateTimeToPointTestData))]
    public void CanCalculateTimeToPoint(Location startLocation, Location targetLocation, Transport transport, double expectedTimeToPoint)
    {
        //Arrange
        var courier = Courier.Create("name", transport, startLocation).Value;
        
        //Act
        var calculateResult = courier.CalculateTimeToPoint(targetLocation);
        
        //Assert
        calculateResult.IsSuccess.Should().BeTrue();
        calculateResult.Value.Should().Be(expectedTimeToPoint);
    }

    [Theory]
    [ClassData(typeof(MoveTestData))]
    public void CanBeMoved(Location startLocation, Location targetLocation, Transport transport,
        Location expectedLocation)
    {
        //Arrange
        var courier = Courier.Create("name", transport, startLocation).Value;
        
        //Act
        var calculateResult = courier.Move(targetLocation);
        
        //Assert
        calculateResult.IsSuccess.Should().BeTrue();
        courier.Location.Should().Be(expectedLocation);
    }
    
    private class CalculateTimeToPointTestData : TheoryData<Location, Location, Transport, double>
    {
        public CalculateTimeToPointTestData()
        {
            Add(Location.Create(1,1).Value, Location.Create(1,1).Value, Transport.Pedestrian, 0);
            Add(Location.Create(1,1).Value, Location.Create(10,10).Value, Transport.Pedestrian, 18);
            Add(Location.Create(1,1).Value, Location.Create(10,10).Value, Transport.Bicycle, 9);
            Add(Location.Create(1,1).Value, Location.Create(10,10).Value, Transport.Car, 6);
            Add(Location.Create(10,10).Value, Location.Create(1,1).Value, Transport.Car, 6);
            Add(Location.Create(1,1).Value, Location.Create(2,1).Value, Transport.Pedestrian, 1);
            Add(Location.Create(1,1).Value, Location.Create(2,1).Value, Transport.Bicycle, 1);
            Add(Location.Create(1,1).Value, Location.Create(2,1).Value, Transport.Car, 1);
        }
    };
    
    private class MoveTestData : TheoryData<Location, Location, Transport, Location>
    {
        public MoveTestData()
        {
            Add(Location.Create(1,1).Value, Location.Create(1,1).Value, Transport.Pedestrian, Location.Create(1,1).Value);
            Add(Location.Create(1,1).Value, Location.Create(10,10).Value, Transport.Pedestrian, Location.Create(2,1).Value);
            Add(Location.Create(1,1).Value, Location.Create(10,10).Value, Transport.Bicycle, Location.Create(3,1).Value);
            Add(Location.Create(1,1).Value, Location.Create(10,10).Value, Transport.Car, Location.Create(4,1).Value);
            Add(Location.Create(1,1).Value, Location.Create(1,10).Value, Transport.Pedestrian, Location.Create(1,2).Value);
            Add(Location.Create(1,1).Value, Location.Create(1,10).Value, Transport.Bicycle, Location.Create(1,3).Value);
            Add(Location.Create(1,1).Value, Location.Create(1,10).Value, Transport.Car, Location.Create(1,4).Value);
            Add(Location.Create(10,10).Value, Location.Create(1,1).Value, Transport.Car, Location.Create(7,10).Value);
            Add(Location.Create(1,1).Value, Location.Create(2,1).Value, Transport.Car, Location.Create(2,1).Value);
            Add(Location.Create(1,1).Value, Location.Create(1,2).Value, Transport.Car, Location.Create(1,2).Value);
            Add(Location.Create(1,1).Value, Location.Create(2,2).Value, Transport.Car, Location.Create(2,2).Value);
            Add(Location.Create(10,10).Value, Location.Create(9,9).Value, Transport.Car, Location.Create(9,9).Value);
        }
    };
}