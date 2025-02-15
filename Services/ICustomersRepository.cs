using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface ICustomersRepository
    {
        public Task<CheckCustomerResult> CustomerExists(string personalNumber, string financialNumber);

        public Task<CreateCustomerResult> CreateCustomer(Customer customer);

        public Task<Customer> GetCustomer(int? customerNo, string personalNo);

        public Task<UpdateCustomerResult> UpdateCustomer(Customer customer);
    }
}
