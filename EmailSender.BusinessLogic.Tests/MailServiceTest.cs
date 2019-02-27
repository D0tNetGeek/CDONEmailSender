using System;
using System.Collections.Generic;
using System.Linq;
using EmailSender.BusinessLogic.Enums;
using EmailSender.BusinessLogic.Interfaces;
using EmailSender.BusinessLogic.Resources;
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
        Mock<IMailTemplateRenderer> mailTemplateRenderer;

        [TestInitialize]
        public void Setup()
        {
            mailService = new MailService();
            senderMock = new Mock<IMailService>();
            errors = new List<string>();
            mailTemplateRenderer = new Mock<IMailTemplateRenderer>();
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
        public void SendWelcomeMail_No_New_Customers_Then_No_Call_To_MailService()
        {
            mailService.Customers = new List<Customer>
            {
                new Customer {CreatedDateTime = DateTime.Now.AddDays(-2)}
            };

            mailService.Send(MailType.Welcome);
            senderMock.VerifyNoOtherCalls();

            Assert.AreEqual(0, errors.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendWelcomeMail_Throws_InvalidOperationException_If_Customers_Not_Specified()
        {
            mailService.Customers = null;
            mailService.Sender = senderMock.Object;

            mailService.Send(MailType.Welcome);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SendWelcomeMail_Throws_InvalidOperationExeption_If_Sender_Not_Specified()
        {
            mailService.Sender = null;
            mailService.Customers = Enumerable.Empty<Customer>();

            mailService.Send(MailType.Welcome);
        }

        [TestMethod]
        public void SendWelcomeEmails_As_Many_Calls_To_MailService_As_New_Customers()
        {
            var renderedTemplate = "foo";

            mailTemplateRenderer.Setup(x => x.Render(EmailTemplates.WelcomeEmail, It.IsAny<object>())).Returns(renderedTemplate);

            mailService.Customers = new List<Customer>{
                new Customer { CreatedDateTime=DateTime.Now, Email="foo@abc.com"},
                new Customer {CreatedDateTime=DateTime.Now, Email = "bar@abc.com"}
            };

            mailService.Sender = senderMock.Object;
            mailService.MailTemplateRenderer = mailTemplateRenderer.Object;

            senderMock.Verify(x => x.Send(It.IsAny<IList<string>>(),
                                       EmailTemplates.OurEmailAddress,
                                       EmailTemplates.WelcomeEMail_Subject,
                                       mailService.Customers.ElementAt(0).Email,
                                       renderedTemplate), Times.Once());

            senderMock.Verify(x => x.Send(It.IsAny<IList<string>>(),
                                       EmailTemplates.OurEmailAddress,
                                       EmailTemplates.WelcomeEMail_Subject,
                                       mailService.Customers.ElementAt(1).Email,
                                       renderedTemplate), Times.Once());
            senderMock.VerifyNoOtherCalls();

            Assert.AreEqual(0, errors.Count);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void SendComeBackEmail_Throws_InvalidOperationException_If_Customers_Not_Specified()
        {
            mailService.Customers = null;
            mailService.Orders = Enumerable.Empty<Order>();
            mailService.Sender = senderMock.Object;

            mailService.Send(MailType.ComeBack);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void SendComeBackEmail_Throws_InvalidOperationException_If_Sender_Not_Specified()
        {
            mailService.Sender = null;

            mailService.Orders = Enumerable.Empty<Order>();
            mailService.Customers = Enumerable.Empty<Customer>();

            mailService.Send(MailType.ComeBack);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void SendComeBackEmail_Throws_InvalidOperationException_If_Orders_Not_Specified()
        {
            mailService.Orders = null;
            mailService.Customers = Enumerable.Empty<Customer>();
            mailService.Sender = senderMock.Object;

            mailService.Send(MailType.ComeBack);
        }

        [TestMethod]
        public void SendWelcomeEmails_As_Many_Calls_To_MailSender_As_Customers_Without_Recent_Orders()
        {
            var renderedTemplate = "foo";

            mailTemplateRenderer.Setup(x => x.Render(EmailTemplates.ComeBackEmail, It.IsAny<object>()))
                                .Returns(renderedTemplate);

            mailService.Customers = new List<Customer>
            {
                new Customer { CreatedDateTime = DateTime.Now, Email = "a@b.com" },
                new Customer { CreatedDateTime = DateTime.Now, Email = "b@b.com" },
                new Customer { CreatedDateTime = DateTime.Now, Email = "c@b.com" }
            };

            mailService.Orders = new List<Order>
            {
                new Order { CustomerEmail = "a@b.com", OrderDatetime = DateTime.Now.AddMonths(-6) },
                new Order { CustomerEmail = "a@b.com", OrderDatetime = DateTime.Now.AddDays(-3) },
                new Order { CustomerEmail = "b@b.com", OrderDatetime = DateTime.Now.AddMonths(-2) },
            };

            mailService.Sender = senderMock.Object;
            mailService.MailTemplateRenderer = mailTemplateRenderer.Object;

            mailService.Send(MailType.ComeBack);

            senderMock.Verify(x => x.Send(It.IsAny<IList<string>>(),
                                       EmailTemplates.OurEmailAddress,
                                       EmailTemplates.ComeBackEMail_Subject,
                                       mailService.Customers.ElementAt(1).Email,
                                       renderedTemplate), Times.Once());
            senderMock.VerifyNoOtherCalls();

            Assert.AreEqual(0, errors.Count);
        }
    }
}
