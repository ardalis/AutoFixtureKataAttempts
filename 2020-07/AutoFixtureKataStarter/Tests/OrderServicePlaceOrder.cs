using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using AutoFixtureKataStarter.Exceptions;
using AutoFixtureKataStarter.Model;
using AutoFixtureKataStarter.Services;
using Moq;
using Xunit;

namespace AutoFixtureKataStarter.Tests
{
    public class OrderServicePlaceOrder
    {
        private readonly Mock<IFileLogger> _mockLogger = new Mock<IFileLogger>();
        private readonly Fixture _fixture = new Fixture();
        private readonly OrderService _orderService;

        public OrderServicePlaceOrder()
        {
            _fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });
            _fixture.Register(() => _mockLogger.Object);
            _orderService = _fixture.Create<OrderService>();
        }

        [Fact]
        public void ThrowsExceptionGivenOrderWithExistingId()
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Id, 123)
                .With(o => o.Customer, _fixture.Build<Customer>()
                                        .Without(c => c.OrderHistory).Create)
                .Create();

            Assert.Throws<InvalidOrderException>(() => _orderService.PlaceOrder(order));

            _mockLogger.Verify(_ => _.Log(It.IsAny<string>()), "asdf");
        }

        private void MakeOrderValid(Order order)
        {
            order.Id = 0;
            order.Customer.CreditRating = 300;
        }

        [Theory, AutoMoqData]
        public void LogsOrderVerifiedForValidOrder(Order order, [Frozen] Mock<IFileLogger> mockLogger, OrderService sut)
        {
            MakeOrderValid(order);

            sut.PlaceOrder(order);

            string expectedMessage = $"Order {order.Id} validated and saved.";

            mockLogger.Verify(l => l.Log(expectedMessage));
        }

        [Theory, AutoMoqData]
        public void LogsOrderExpeditedForValidOrderAndCustomer([Frozen] Mock<IFileLogger> mockLogger, OrderService sut)
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Id, 0)
                .With(o => o.TotalAmount, 5001)
                .With(o => o.Customer, _fixture.Build<Customer>()
                                        .With(c => c.CreditRating, 501)
                                        .Without(c => c.OrderHistory).Create)
                .Create();

            sut.PlaceOrder(order);

            string expectedMessage = $"Order {order.Id} expedited.";

            mockLogger.Verify(l => l.Log(expectedMessage));
            Assert.True(order.IsExpedited);
        }

        [Theory, AutoMoqData]
        public void LogsOrderExpeditedForValidOrderAndBigCustomer([Frozen] Mock<IFileLogger> mockLogger, OrderService sut)
        {
            var order = _fixture.Build<Order>()
                .With(o => o.Id, 0)
                .With(o => o.TotalAmount, 100)
                .With(o => o.Customer, _fixture.Build<Customer>()
                                        .With(c => c.CreditRating, 301)
                                        .With(c => c.OrderHistory, new System.Collections.Generic.List<Order> { new Order { TotalAmount = 10001 } }).Create)
                .Create();

            sut.PlaceOrder(order);

            string expectedMessage = $"Order {order.Id} expedited.";

            mockLogger.Verify(l => l.Log(expectedMessage));
            Assert.True(order.IsExpedited);

        }
    }
}
