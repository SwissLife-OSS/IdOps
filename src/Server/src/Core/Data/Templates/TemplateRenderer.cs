using HandlebarsDotNet;

namespace IdOps.Templates
{
    public class TemplateRenderer : ITemplateRenderer
    {
        static TemplateRenderer()
        {
            Handlebars.RegisterHelper("toLower", (writer, context, parameters) =>
            {
                writer.WriteSafeString(parameters[0]?.ToString()?.ToLower());
            });
            Handlebars.RegisterHelper("toUpper", (writer, context, parameters) =>
            {
                writer.WriteSafeString(parameters[0]?.ToString()?.ToUpper());
            });
            Handlebars.RegisterHelper("ets", (writer, context, parameters) =>
            {
                var paramValue = parameters[0]?.ToString();

                if (!string.IsNullOrWhiteSpace(paramValue))
                {
                    writer.WriteSafeString(paramValue.Trim('/') + "/");
                }
            });
        }

        public string Render(string template, object data)
        {
            return Handlebars.Compile(template)(data);
        }
    }
}
