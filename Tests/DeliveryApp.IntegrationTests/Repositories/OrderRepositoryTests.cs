using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
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
    public OrderRepositoryShould()
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
    public async Task CanAddOrder()
    {
        //Arrange
        var location = Location.Create(1, 1).Value;
        var order = Order.Create(Guid.NewGuid(), location).Value;

        //Act
        var orderRepository = new OrderRepository(_dbContext);
        await orderRepository.AddAsync(order);
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Assert
        var orderFromDb = await orderRepository.GetByIdAsync(order.Id);
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async Task CanUpdateOrder()
    {
        //Arrange
        var location = Location.Create(1, 1).Value;
        var order = Order.Create(Guid.NewGuid(), location).Value;
        var courier = Courier.Create("courier", Transport.Bicycle, location).Value;

        var orderRepository = new OrderRepository(_dbContext);
        await orderRepository.AddAsync(order);
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        order.Assign(courier);
        orderRepository.Update(order);

        //Assert
        var orderFromDb = await orderRepository.GetByIdAsync(order.Id);
        order.Should().BeEquivalentTo(orderFromDb);
        order.CourierId.Should().Be(courier.Id);
        order.Status.Should().Be(OrderStatus.Assigned);
    }


    [Fact]
    public async Task CanGetById()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(1, 1).Value;
        var order = Order.Create(orderId, location).Value;

        var orderRepository = new OrderRepository(_dbContext);
        await orderRepository.AddAsync(order);
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        var orderById = await orderRepository.GetByIdAsync(orderId);

        //Assert
        orderById.Should().BeEquivalentTo(order);
    }

    [Fact]
    public async Task CanGetAllNewOrdersAsync()
    {
        //Arrange
        var orderRepository = new OrderRepository(_dbContext);
        var newOrdersTestData = GetNewOrdersTestData();
        var assignedOrdersTestData = GetAssignedOrdersTestData();
        foreach (var order in newOrdersTestData)
        {
            await orderRepository.AddAsync(order);
        }
        foreach (var order in assignedOrdersTestData)
        {
            await orderRepository.AddAsync(order);
        }
        
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        var newOrders = await orderRepository.GetAllNewOrdersAsync();

        //Assert
        newOrders.Should().NotBeEmpty();
        newOrders.Should().OnlyContain(order => order.Status == OrderStatus.Created);
    }
    
    [Fact]
    public async Task CanGetAllAssignedOrdersAsync()
    {
        //Arrange
        var orderRepository = new OrderRepository(_dbContext);
        var newOrdersTestData = GetNewOrdersTestData();
        var assignedOrdersTestData = GetAssignedOrdersTestData();
        foreach (var order in newOrdersTestData)
        {
            await orderRepository.AddAsync(order);
        }
        foreach (var order in assignedOrdersTestData)
        {
            await orderRepository.AddAsync(order);
        }
        
        var unitOfWork = new UnitOfWork(_dbContext);
        await unitOfWork.SaveChangesAsync();

        //Act
        var assignedOrders = await orderRepository.GetAllAssignedOrdersAsync();

        //Assert
        assignedOrders.Should().NotBeEmpty();
        assignedOrders.Should().OnlyContain(order => order.Status == OrderStatus.Assigned);
    }

    private IEnumerable<Order> GetNewOrdersTestData()
    {
        var location = Location.Create(1, 1).Value;
        return
        [
            Order.Create(Guid.NewGuid(), location).Value,
            Order.Create(Guid.NewGuid(), location).Value
        ];
    }

    private IEnumerable<Order> GetAssignedOrdersTestData()
    {
        var location = Location.Create(1, 1).Value;

        Order[] assignedOrders =
        [
            Order.Create(Guid.NewGuid(), location).Value,
            Order.Create(Guid.NewGuid(), location).Value
        ];
        foreach (var order in assignedOrders)
        {
            order.Assign(Courier.Create("courier1", Transport.Bicycle, location).Value);
        }

        return assignedOrders;
    }
}