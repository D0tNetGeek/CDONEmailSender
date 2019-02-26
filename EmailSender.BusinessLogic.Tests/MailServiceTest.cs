using System;
using System.Collections.Generic;
using EmailSender.BusinessLogic.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EmailSender.BusinessLogic.Tests
{
    [TestClass]
    public class MailServiceTest
    {
        MailService mailService;
        Mock<IMailSender> senderMock;
        List<string> errors;

        [TestInitialise]
        public void Setup()
        {
            _mailService = new MailService();
            senderMock = new Mock<IMailSender>();
            errors = new List<string>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Send_Invalid_Mail_Type()
        {
            mailService.Send((MailType)(-1));
        }

        [TestMethod]
        public void SendWelcomeMails_No_Customer_Then_No_Call_To_MailSender()
        {
            mailService.Customers = Enumerable.Empty<Customer>();
            mailService.Sender = senderMock.Object;

            mailService.Send(MailType.Welcome);

            senderMock.VerifyNoOtherCalls();

            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        public void SendWelcomeMail_No_New_Customers_Then_No_Call_To_MailSender()
        {
            mailService.Customers = new List<Customer>
            {
                new Customer{CreatedDatetime = DateTime.Now.AddDays(-2)}
            };

            mailService.Sender(MailType.Welcome);
            senderMock.VerifyNoOtherCalls();

            Assert.AreEqual(0, errors.Count);
        }
    }
}
