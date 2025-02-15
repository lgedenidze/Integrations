using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class IdentDocument
    {
        public DocumentType DocumentType { get; set; }

        public string DocumentNo { get; set; }

        public DateTime DocumentIssueDate { get; set; }

        public DateTime? DocumentExpireDate { get; set; }

        public string DocumentIssuer { get; set; }

        public string DocumentIssuerCountry { get; set; }

    }
}
