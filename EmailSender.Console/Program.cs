using System.Linq;
using EmailSender.BusinessLogic;
using EmailSender.BusinessLogic.Enums;

namespace EmailSender.Console
{
    //This application sends emails to customers.
    internal class Program
    {
        /// <summary>
        /// This console app sends email to customers
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            MailService sender = new MailService();

            sender.Errors.Clear();

            System.Console.WriteLine("Send Welcomemail\n");
            sender.Send(MailType.Welcome);

            System.Console.WriteLine("\nSend Comebackmail\n\n");
            sender.Send(MailType.ComeBack);

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
