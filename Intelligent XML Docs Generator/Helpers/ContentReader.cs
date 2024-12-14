using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Intelligent_XML_Docs_Generator.Helpers
{
    public static class ContentReader
    {
        private static readonly string SystemInstructionUrl = @"https://raw.githubusercontent.com/phanxuanquang/Intelligent-XML-Docs-Generator/refs/heads/master/Intelligent%20XML%20Docs%20Generator/SystemInstruction.md";
        private static string _systemInstructionContent = string.Empty;

        public static async Task<string> GetPromptContentAsync()
        {
            if (!string.IsNullOrWhiteSpace(_systemInstructionContent))
            {
                return _systemInstructionContent;
            }

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(SystemInstructionUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        _systemInstructionContent = await response.Content.ReadAsStringAsync();
                        return _systemInstructionContent;
                    }
                    else
                    {
                        throw new ArgumentNullException($"Cannot get the system instruction content from {SystemInstructionUrl}");
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentNullException($"Cannot get the system instruction content from {SystemInstructionUrl}", ex);
                }
            }
        }
    }
}
