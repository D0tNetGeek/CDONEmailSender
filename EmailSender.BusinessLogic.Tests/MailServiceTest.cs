using System;
using System.Collections.Generic;
using System.Linq;
using EmailSender.BusinessLogic.Enums;
using EmailSender.BusinessLogic.Interfaces;
using EmailSender.DataLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EmailSender.BusinessLogic.Tests
{
    [TestClass]
    public class MailServiceTest
    {
        MailService mailService;
        Mock<IMailService> senderMock;
        List<string> errors;

        [TestInitialize]
        public void Setup()
        {
            mailService = new MailService();
            senderMock = new Mock<IMailService>();
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
                new Customer {CreatedDateTime = DateTime.Now.AddDays(-2)}
            };

            mailService.Send(MailType.Welcome);
            senderMock.VerifyNoOtherCalls();

            Assert.AreEqual(0, errors.Count);
        }
    }
}
