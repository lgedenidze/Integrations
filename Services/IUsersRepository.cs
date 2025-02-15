using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IUsersRepository
    {
        //public User GetByUserNameAndPassword(string userName, string password);

        public Task<bool> AreCredentialsValid(string userName, string password);

        public Task<List<UserRole>> GetUserRoles(string userName);
    }
}
