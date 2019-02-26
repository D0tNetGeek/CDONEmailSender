using System.Collections.Generic;
namespace EmailSender.BusinessLogic.Interfaces
{
    /// <summary>
    /// Mail sender interface.
    /// </summary>
    public interface IMailSender
    {
        void Send(IList<string> errors, string from, string subject, string to, string body);
    }

    sealed class NullMailSender : IMailSender
    {
        public void Send(IList<string> errors,  string from, string subject, string to, string body)
        {
            
        }
    }
}
