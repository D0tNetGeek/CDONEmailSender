using System.Linq;
using EmailSender.BusinessLogic;

namespace EmailSender.Console
{
    //This application runs everyday.
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

            System.Console.WriteLine("Send Welcomemail\n");
            sender.SendWelcomeEmails();

            System.Console.WriteLine("\nSend Comebackmail\n\n");
            sender.SendComeBackEmail("CDONComebackToUs");

            if (sender.Errors.Any())
            {
                System.Console.WriteLine("Oops, something went wrong when sending mail (I think...)\n\n");
                System.Console.WriteLine(string.Join("\n", sender.Errors));
            }
            else
                System.Console.WriteLine("\nAll mails are sent, I hope...");

            System.Console.ReadKey();
        }
    }
}
