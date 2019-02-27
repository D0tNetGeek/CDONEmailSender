using System.Collections.Generic;
namespace EmailSender.BusinessLogic.Interfaces
{
    /// <summary>
    /// Mail sender interface.
    /// </summary>
    public interface IMailService
    {
        void Send(IList<string> errors, string from, string subject, string to, string body);
    }
}
