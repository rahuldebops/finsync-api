namespace finsyncapi.BAL.IServices
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string templateName,Dictionary<string, string> values);
    }
}