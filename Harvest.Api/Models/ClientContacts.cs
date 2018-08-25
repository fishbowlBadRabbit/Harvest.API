using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class ClientContacts : BaseModel
    {
        public Client Client { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneOffice { get; set; }
        public string PhoneMobile { get; set; }
        public string Fax { get; set; }
    }
}
