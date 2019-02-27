using System;
using System.Collections.Generic;
using System.Linq;
using EmailSender.BusinessLogic.Configs;
using EmailSender.BusinessLogic.Enums;
using EmailSender.BusinessLogic.Interfaces;
using EmailSender.BusinessLogic.Resources;
using EmailSender.BusinessLogic.TemplateRenderer;

namespace EmailSender.BusinessLogic
{
    /// <summary>
    /// Business logic for mail sending.
    /// </summary>
    public class MailService
    {
        private readonly IDictionary<MailType, MailTypeConfiguration> _mailTypeConfigurations;
        public List<string> Errors { get; } = new List<string>();
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IMailService Sender { get; set; }
        public IMailTemplateRenderer MailTemplateRenderer { get; set; }

        private static readonly string OurEmailAddress = EmailTemplates.OurEmailAddress;

        /// <summary>
        /// Constructor - Instantiates required Objects and Mail Templates
        /// </summary>
        public MailService()
        {
            //List all customers
            Customers = DataLayer.ListCustomers();

            //List all orders
            Orders = DataLayer.ListOrders();

            //Instantiate Sender
            Sender = new SmtpMailSender();

            //Default template renderer
            MailTemplateRenderer = new HandlebarsTemplateRenderer();

            _mailTypeConfigurations = new Dictionary<MailType, MailTypeConfiguration>()
            {
                {
                    //Sets up the welcome email config.
                    MailType.Welcome, new MailTypeConfiguration
                    {
                        GetCustomers = GetNewCustomers,
                        Subject = EmailTemplates.WelcomeEMail_Subject,
                        BodyTemplate = EmailTemplates.WelcomeEmail
                    }
                },
                {
                    //Sets up the comeback email config.
                    MailType.ComeBack, new MailTypeConfiguration
                    {
                        GetCustomers = GetCustomersWithoutRecentOrders,
                        Subject = EmailTemplates.ComeBackEMail_Subject,
                        BodyTemplate = EmailTemplates.ComeBackEmail
                    }
                }
            };
        }

        /// <summary>
        /// Get All the customers.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Customer> GetNewCustomers()
        {
            if (Customers == null)
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
            if (Orders == null)
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
        /// Calls the Send function based on MailType Enum.
        /// </summary>
        /// <param name="mailType"></param>
        public void Send(MailType mailType)
        {
            if (!_mailTypeConfigurations.TryGetValue(mailType, out var mailTypeConfiguration))
            {
                throw new InvalidOperationException("Unsupported mail type");
            }

            var customers = mailTypeConfiguration.GetCustomers();

            Send(customers, mailType == MailType.ComeBack ? "CDONComebackToUs" : "", mailTypeConfiguration.Subject, mailTypeConfiguration.BodyTemplate);
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
            if (Sender == null)
                throw new InvalidOperationException("Cannot send e-mails without specifying the Sender property");

            Errors.Clear();

            foreach (var customer in customers)
            {
                string body = MailTemplateRenderer.Render(template, new
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
