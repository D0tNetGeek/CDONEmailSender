using System;
using System.Collections.Generic;
using System.Linq;
using EmailSender.BusinessLogic;

namespace EmailSender.Console
{
    internal class Program
    {
        /// <summary>
        /// This console app sends email to customers
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            MailSender sender = new MailSender();

            sender.Errors.Clear();

            System.Console.WriteLine("Send Welcomemail");
            sender.SendWelcomeEmails();

            System.Console.WriteLine("Send Comebackmail");
            sender.SendComeBackEmail();

            if (sender.Errors.Any())
            {
                System.Console.WriteLine("Oops, something went wrong when sending mail (I think...)");
                System.Console.WriteLine(string.Join("\n", sender.Errors));
            }
            else
                System.Console.WriteLine("All mails are sent, I hope...");

            System.Console.ReadKey();
        }
//            sender.Errors.Clear();
//            sender.SendComeBackEmail("this is voucher code");

//            if(sender.Errors.Any())
//                System.Console.WriteLine("All mails are sent, I hope....");
//            //Call the method that do the work for me. I.E. sending the emails.
//            System.Console.WriteLine("Send Welcomemail");

//            bool welcomeEmailSuccess = SendWelcomeMail();

//#if DEBUG
//            //Debug mode, always send Comeback email
//            System.Console.WriteLine("Send Comebackmail");
//            bool comebackEmailSucces = SendComeBackEmail("CDONComebackToUs");
//#else
//            //Every Sunday run Comeback mail
//            if(DateTime.Now.DayOfWeek.Equals(DayOfWeek.Monday))
//            {
//                Console.WriteLine("Send Comebackmail");
//                comebackEmailSuccess = SendComeBackEmail("CDONComebackToUs");
//            }
//#endif

//            //Check if the sending went OK
//            if (comebackEmailSucces == true)
//            {
//                System.Console.WriteLine("All mails are send, I hope...");
//            }

//            //Check if the sending was not going well...
//            if (comebackEmailSucces == false)
//            {
//                System.Console.WriteLine("Oops, something went wrong when sending email (I think...)");
//            }

//            System.Console.ReadKey();
//        }

//        /// <summary>
//        /// Send Welcome Email
//        /// </summary>
//        private static List<Customer> customers = DataLayer.ListCustomers();

//        public static void SendEmail(string recipients, string from, string subject, string body)
//        {
//            foreach (Customer customer in customers)
//            {
//                var mailMessage = new MailMessage();

//                mailMessage.To.Add(string.Join(",", recipients));
//                mailMessage.From = new MailAddress(from);
//                mailMessage.Subject = subject;
//                mailMessage.Body = body;
//            }
//        }

//        public static bool SendWelcomeMail()
//        {
//            string welcomeSubject = "Welcome as a new customer Company!";
//            string ourEmailAddress = "info@company.com";
//            string bodyTemplate =
//                "Hi {0}<br>We would like to welcome you as customer on our site!<br><br>Best Regards<br>Company Team";

//            try
//            {
//                foreach (var customer in DataLayer..ListCustomers())
//                {
//                    if(customer.Created)
//                }
//            }
//        }
    }
}
