namespace EmailSender.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface for template rendering.
    /// </summary>
    public interface IMailTemplateRenderer
    {
        string Render(string template, object input);
    }
}