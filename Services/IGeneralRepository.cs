using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IGeneralRepository
    {
        public Task<Countries> GetCountries();
        public Task<Districts> GetDistricts();
        public Task<Regions> GetRegions();


    }
}
