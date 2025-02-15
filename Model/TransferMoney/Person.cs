using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Model
{
    public class Person
    {
        public string PersonalNo { get; set; }

        public string FirstNameGe { get; set; }

        public string LastNameGe { get; set; }

        public string FirstNameEn { get; set; }

        public string LastNameEn { get; set; }

        public DateTime BirthDate { get; set; }

        public Sex? Sex { get; set; }

        public string Citizenship { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public IdentDocument Document { get; set; }
    }
}
