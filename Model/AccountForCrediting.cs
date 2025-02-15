using Microsoft.AspNetCore.Identity;

namespace Integrations.Model
{

    public class AccountForCrediting : AccountsBase
    {
        public string phoneNumber { get; set; }

        public int customerId { get; set; }

        public string customerName { get; set; }

        public string customerNameEn { get; set; }

        public string personalNo { get; set; }

    }
}
