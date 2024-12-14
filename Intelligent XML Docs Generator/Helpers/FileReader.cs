using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Intelligent_XML_Docs_Generator.Helpers
{
    public static class FileReader
    {
        private static readonly string SystemInstructionUrl = "SystemInstruction.txt";

        public static async Task<string> GetPromptContentAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(SystemInstructionUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
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
