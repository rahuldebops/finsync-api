using finsyncapi.BAL.IServices;

namespace finsyncapi.BAL.Services
{
    public class TemplateRenderer : ITemplateRenderer
    {
        public async Task<string> RenderAsync(string templateName,Dictionary<string, string> values)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Helpers","Template","Email",$"{templateName}.html");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Template not found: {templateName}");
            }

            var html = await File.ReadAllTextAsync(filePath);

            foreach (var item in values)
            {
                html = html.Replace($"{{{{{item.Key}}}}}",item.Value);
            }

            return html;
        }
    }
}