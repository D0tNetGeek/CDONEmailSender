using System;
using System.Collections.Generic;
using System.Linq;
using EmailSender.BusinessLogic.Interfaces;
using EmailSender.DataLayer;

namespace EmailSender.BusinessLogic
{
    /// <summary>
    /// Business logic for mail sending.
    /// </summary>
    public class MailSender
    {
        public List<string> Errors { get; } = new List<string>();
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IMailSender Sender { get; set; }

        private const string OurEmailAddress = "infor@cdon.com";

        /// <summary>
        /// 
        /// </summary>
        public MailSender()
        {
            //List all customers
            Customers = DataLayer1.ListCustomers;

            //List all orders
            Orders = DataLayer1.ListOrders();

            //Instantiate Sender
            Sender = new SmtpMailSender();
        }

        /// <summary>
        /// Send Welcome mail.
        /// </summary>
        public void SendWelcomeEmails()
        {
            //Get the Welcome Email Template.
            var template = EmailTemplates.WelcomeEmail;

            //Send the mail.
            Send(GetNewCustomers(), "", "Welcome as a new customer at CDON!", template);
        }

        /// <summary>
        /// Send ComeBack mail.
        /// </summary>
        public void SendComeBackEmail(string voucher)
        {
            //Get the Come Back Email Template.
            var template = EmailTemplates.ComeBackEmail;

            //Send the mail.
#if DEBUG
            Send(GetCustomersWithoutRecentOrders(), voucher, "We miss you as a customer", template);
#else
            //Every sunday run Comeback mail.
            if(DateTime.Now.DayOfWeek.Equals(DayOfWeek.Monday))
                Send(GetCustomersWithoutRecentOrders(), "CDONComebackToUs", "We miss you as a customer", template);
#endif
        }

        /// <summary>
        /// Get All the customers.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Customer> GetNewCustomers()
        {
            if(Customers==null)
                throw new InvalidOperationException("Cannot search for a new new customers if you do not specify a list of customers in Customers");

            //If the customer is newly registered, one day back in time.
            var yesterday = DateTime.Now.Date.AddDays(-1);
            
            return Customers.Where(x => x.CreatedDateTime >= yesterday);
        }

        /// <summary>
        /// Get the customers without recent orders.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Customer> GetCustomersWithoutRecentOrders()
        {
            //Raise exception if no customers found.
            if (Customers == null)
                throw new InvalidOperationException("Cannot search for inactive customers if you do not specify a list of customers in Customers");

            //Raise exception if no orders found.
            if(Orders ==null)
                throw new InvalidOperationException("Cannot search for inactive customers if you do not specify a list of orders in Orders");

            //Check the orders and see if any customer exist in the list.
            var oneMonthAgo = DateTime.Now.Date.AddMonths(-1);

            return Customers.Where(c =>
            {
                var latestOrder = Orders
                    .Where(o => o.CustomerEmail == c.Email)
                    .OrderByDescending(o => o.OrderDatetime)
                    .FirstOrDefault();

                return latestOrder != null && latestOrder.OrderDatetime < oneMonthAgo;
            });
        }

        /// <summary>
        /// Prepare to send the email. Render the template.
        /// </summary>
        /// <param name="customers"></param>
        /// <param name="subject"></param>
        /// <param name="voucher"></param>
        /// <param name="template"></param>
        private void Send(IEnumerable<Customer> customers, string voucher, string subject, string template)
        {
            if(Sender ==null)
                throw new InvalidOperationException("Cannot send e-mails without specifying the Sender property");

            Errors.Clear();

            foreach (var customer in customers)
            {
                var compiledTemplate = HandlebarsDotNet.Handlebars.Compile(template);

                string body = compiledTemplate(new
                {
                    CompanyName = "CDON",
                    Customer = customer,
                    Voucher = voucher
                });
                
                //Send the email.
                Sender.Send(Errors, OurEmailAddress, subject, customer.Email, body);
            }
        }
    }
}
