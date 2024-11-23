using System.Runtime.Serialization;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel;

public class LocationShould
{
    [Theory]
    [InlineData(1,1)]
    [InlineData(10,10)]
    public void BeCorrectWhenParamsIsCorrectOnCreated(int x, int y)
    {
        //Arrange

        //Act
        var location = Location.Create(1,10);

        //Assert
        location.IsSuccess.Should().BeTrue();
        location.Value.X.Should().Be(1);
        location.Value.Y.Should().Be(10);
    }
    
    [Theory]
    [InlineData(0,5)]
    [InlineData(11,5)]
    [InlineData(5,0)]
    [InlineData(5,11)]
    public void ReturnErrorWhenParamsIsInCorrectOnCreated(int x, int y)
    {
        //Arrange

        //Act
        var location = Location.Create(x,y);

        //Assert
        location.IsSuccess.Should().BeFalse();
        location.Error.Should().NotBeNull();
    }
    
    [Fact]
    public void BeEqualWhenAllPropertiesIsEqual()
    {
        //Arrange
        var location1 = Location.Create(1,1).Value;
        var location2 = Location.Create(1,1).Value;

        //Act
        var result = location1 == location2;

        //Assert
        result.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(NotEqualTestData))]
    public void BeNotEqualWhenAllPropertiesIsNotEqual(Location otherLocation)
    {
        //Arrange
        var location = Location.Create(1,1).Value;

        //Act
        var result = location == otherLocation;

        //Assert
        result.Should().BeFalse();
    }
    

    [Theory]
    [ClassData(typeof(DistanceTestData))]
    public void CanCalculateDistanceToLocation(Location otherLocation, int checkDistance)
    {
        //Arrange
        var location = Location.Create(1,1).Value;
        
        //Act
        var distance = location.DistanceTo(otherLocation);
        var reverseDistance = otherLocation.DistanceTo(location);
        
        //Assert
        distance.IsSuccess.Should().BeTrue();
        distance.Value.Should().Be(checkDistance);
        reverseDistance.IsSuccess.Should().BeTrue();
        distance.Value.Should().Be(reverseDistance.Value);
    }

    private class NotEqualTestData : TheoryData<Location>
    {
        public NotEqualTestData()
        {
            Add(Location.Create(1,2).Value);
            Add(Location.Create(2,1).Value);
            Add(Location.Create(2,2).Value);
        }
    }

    private class DistanceTestData : TheoryData<Location, int>
    {
        public DistanceTestData()
        {
            Add(Location.Create(1,1).Value, 0);
            Add(Location.Create(1,10).Value, 9);
            Add(Location.Create(10,1).Value, 9);
            Add(Location.Create(10,10).Value, 18);
        }
    };
}