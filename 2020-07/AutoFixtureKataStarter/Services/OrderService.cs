using AutoFixtureKataStarter.Exceptions;
using AutoFixtureKataStarter.Model;
using System;

namespace AutoFixtureKataStarter.Services
{
    /// <summary>
    /// Borrowing from Brendan: https://github.com/DevChatter/BuilderTestSample/blob/master/src/BuilderTestSample/Services/OrderService.cs
    /// </summary>
    public class OrderService
    {
        private readonly IFileLogger _logger;

        public OrderService(IFileLogger logger)
        {
            _logger = logger;
            _logger.Log("OrderService created.");
        }

        public void PlaceOrder(Order order)
        {
            ValidateOrder(order);

            SaveOrder(order);
            _logger.Log($"Order {order.Id} validated and saved.");

            ExpediteOrder(order);

            AddOrderToCustomerHistory(order);
        }

        private void ValidateOrder(Order order)
        {
            if (order.Id != 0) throw new InvalidOrderException("Order ID must be 0.");

            if (order.TotalAmount <= 0) throw new InvalidOrderException("Amount must be greater than 0.");

            if (order.Customer == null) throw new InvalidOrderException("Order requires a customer.");

            ValidateCustomer(order.Customer);
        }

        private void SaveOrder(Order order)
        {
            order.Id = new Random().Next(1000, 10000);
        }

        private void ValidateCustomer(Customer customer)
        {
            if (customer.Id <= 0) throw new InvalidCustomerException("Customer must have an ID greater than 0.");
            if (customer.HomeAddress == null) throw new InvalidCustomerException("Customer must have an address.");
            if (string.IsNullOrEmpty(customer.FirstName)
                || string.IsNullOrEmpty(customer.LastName)) throw new InvalidCustomerException("Customer must have first and last name.");
            if (customer.CreditRating <= 200)
                throw new InsufficientCreditException("Customer must have credit above 200.");
            if (customer.TotalPurchases < 0) throw new InvalidCustomerException("Customers cannot have negative purchase total.");

            ValidateAddress(customer.HomeAddress);
        }

        private void ValidateAddress(Address homeAddress)
        {
            if (string.IsNullOrEmpty(homeAddress.Street1))
                throw new InvalidAddressException("Street 1 is required.");
            if (string.IsNullOrEmpty(homeAddress.City))
                throw new InvalidAddressException("City is required.");
            if (string.IsNullOrEmpty(homeAddress.State))
                throw new InvalidAddressException("State is required.");
            if (string.IsNullOrEmpty(homeAddress.PostalCode))
                throw new InvalidAddressException("Postal code is required.");
            if (string.IsNullOrEmpty(homeAddress.Country))
                throw new InvalidAddressException("Country is required.");
        }

        private void ExpediteOrder(Order order)
        {
            if(BigCustomer(order.Customer) ||
                BigOrder(order))
            {
                order.IsExpedited = true;
                _logger.Log($"Order {order.Id} expedited.");
            }
        }

        private bool BigCustomer(Customer c) => c.TotalPurchases > 10000;
        private bool BigOrder(Order o) => o.TotalAmount > 5000m && o.Customer.CreditRating > 500;

        private void AddOrderToCustomerHistory(Order order)
        {
            order.Customer.OrderHistory.Add(order);
        }
    }
}
