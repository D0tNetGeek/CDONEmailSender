using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using EmailSender.BusinessLogic.Interfaces;

namespace EmailSender.BusinessLogic
{
    /// <summary>
    /// Class implements IMailSender interface to send the mails.
    /// </summary>
    sealed class SmtpMailSender : IMailService
    {
        private const int NumberOfRetriesOnError = 2;
        private const int DelayOnError = 10;

        public void Send(IList<string> errors, string from, string subject, string to, string body)
        {
            //Try re-sending mails for number of retries times, in case of error.
            for (int i = 0; i <= NumberOfRetriesOnError; ++i)
            {
                try
                {
                    //Create a SmtpClient to our smtphost: yoursmtphost
                    using (var smtp = new SmtpClient("yoursmtphost"))
                    {
                        //Create a new MailMessage
                        MailMessage mail = new MailMessage
                        {
                            //Send mail from "info@cdon.com
                            From = new MailAddress(@from)
                        };

                        //Add customer to receiver list
                        mail.To.Add(to);

                        //Add body to mail
                        mail.Body = body;

                        //Add subject
                        mail.Subject = subject;

#if DEBUG
                        //Don't send mails in debug mode, just write the emails in console.
                        Console.WriteLine("Send mail to :" + to);
#else
                        //Send mail
                        smtp.Send(mail);
#endif
                    }
                }
                catch (SmtpException ex)
                {
                    //Something went wrong.
                    if (i < NumberOfRetriesOnError)
                    {
                        Thread.Sleep((i + 1) * DelayOnError);
                    }
                    else
                    {
                        errors.Add($"{to}: {ex.Message}");
                    }
                }
            }
        }
    }
}
