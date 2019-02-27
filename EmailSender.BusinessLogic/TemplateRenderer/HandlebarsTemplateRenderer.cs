using EmailSender.BusinessLogic.Interfaces;

namespace EmailSender.BusinessLogic.TemplateRenderer
{
    /// <summary>
    /// Handlebars Template Renderer Helper class to render the templates.
    /// </summary>
    public class HandlebarsTemplateRenderer : IMailTemplateRenderer
    {
        public string Render(string template, object input)
        {
            var compiledTemplate = HandlebarsDotNet.Handlebars.Compile(template);

            return compiledTemplate(input);
        }
    }
}