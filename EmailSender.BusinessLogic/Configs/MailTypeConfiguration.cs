using System;
using System.Collections.Generic;

namespace EmailSender.BusinessLogic.Configs
{
    /// <summary>
    /// Configuaration class
    /// </summary>
    public class MailTypeConfiguration
    {
        public Func<IEnumerable<Customer>> GetCustomers { get; set; }
        public string BodyTemplate { get; set; }
        public string Subject { get; set; }
    }
}