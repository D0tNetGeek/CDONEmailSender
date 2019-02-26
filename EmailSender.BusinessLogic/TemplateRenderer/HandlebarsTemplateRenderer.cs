using EmailSender.BusinessLogic.Interfaces;

namespace EmailSender.BusinessLogic.TemplateRenderer
{
    public class HandlebarsTemplateRenderer : IMailTemplateRenderer
    {
        public string Render(string template, object input)
        {
            var compiledTemplate = HandlebarsDotNet.Handlebars.Compile(template);

            return compiledTemplate(input);
        }
    }
}