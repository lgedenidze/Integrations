using Integrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IKYCRepository
    {
        public Task<KYCResult> UpdateKyc(KYC kyc);
    }
}
