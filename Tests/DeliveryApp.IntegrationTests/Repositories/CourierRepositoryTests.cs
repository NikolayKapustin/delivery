using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class CourierRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("delivery")
        .WithUsername("user")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    private ApplicationDbContext _dbContext;

    /// <summary>
    /// Ctr
    /// </summary>
    public CourierRepositoryTests()
    {
    }

    /// <summary>
    /// Инициализация окружения
    /// </summary>
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName); })
            .Options;
        _dbContext = new ApplicationDbContext(contextOptions);
        await _dbContext.Database.MigrateAsync();
    }

    /// <summary>
    /// Уничтожение окружения
    /// </summary>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanAddCourier()
    {
        //Arrange
        var location = Location.Create(1, 1).Value;
        var courier = Courier.Create("test_courier", Transport.Pedestrian, location).Value;

        //Act
        var courierRepository = new CourierRepository(_dbContext);
        await courierRepository.AddAsync(courier);
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var courierFromDb = await courierRepository.GetByIdAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
    }

    [Fact]
    public async Task CanUpdateCourier()
    {
        //Arrange
        var location = Location.Create(1, 1).Value;
        var courier = Courier.Create("test_courier", Transport.Pedestrian, location).Value;

        var courierRepository = new CourierRepository(_dbContext);
        await courierRepository.AddAsync(courier);
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        courier.SetBusy();
        courierRepository.Update(courier);

        //Assert
        var courierFromDb = await courierRepository.GetByIdAsync(courier.Id);
        courier.Should().BeEquivalentTo(courierFromDb);
        courier.Status.Should().Be(CourierStatus.Busy);
    }


    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var location = Location.Create(1, 1).Value;
        var courier = Courier.Create("test_courier", Transport.Pedestrian, location).Value;
        var courierId = courier.Id;

        var courierRepository = new CourierRepository(_dbContext);
        await courierRepository.AddAsync(courier);
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        var courierById = await courierRepository.GetByIdAsync(courierId);

        //Assert
        courierById.Should().BeEquivalentTo(courier);
    }

    [Fact]
    public async Task CanGetAllReadyCouriersAsync()
    {
        //Arrange
        var courierRepository = new CourierRepository(_dbContext);
        var readyCouriersTestData = GetReadyCouriersTestData();
        var busyCouriersTestData = GetBusyCouriersTestData();
        foreach (var courier in readyCouriersTestData)
        {
            await courierRepository.AddAsync(courier);
        }

        foreach (var courier in busyCouriersTestData)
        {
            await courierRepository.AddAsync(courier);
        }

        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        var readyCouriers = await courierRepository.GetAllReadyCouriersAsync();

        //Assert
        readyCouriers.Should().NotBeEmpty();
        readyCouriers.Should().OnlyContain(courier => courier.Status == CourierStatus.Free);
    }
    
    private IEnumerable<Courier> GetReadyCouriersTestData()
    {
        var location = Location.Create(1, 1).Value;
        return
        [
            Courier.Create("test_courier1", Transport.Pedestrian, location).Value,
            Courier.Create("test_courier2", Transport.Pedestrian, location).Value
        ];
    }

    private IEnumerable<Courier> GetBusyCouriersTestData()
    {
        var location = Location.Create(1, 1).Value;

        Courier[] busyCouriers =
        [
            Courier.Create("test_courier3", Transport.Pedestrian, location).Value,
            Courier.Create("test_courier4", Transport.Pedestrian, location).Value
        ];
        foreach (var courier in busyCouriers)
        {
            courier.SetBusy();
        }

        return busyCouriers;
    }
}