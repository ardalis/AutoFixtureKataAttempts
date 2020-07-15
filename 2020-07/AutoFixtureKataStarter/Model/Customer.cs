using System.Collections.Generic;
using System.Linq;

namespace AutoFixtureKataStarter.Model
{
    public class Customer
    {
        public Customer(int id)
        {
            Id = id;
        }
        
        public int Id { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address HomeAddress { get; set; }
        public int CreditRating { get; set; }
        public decimal TotalPurchases => OrderHistory.Sum(o => o.TotalAmount);

        public List<Order> OrderHistory { get; set; } = new List<Order>();
    }
}
