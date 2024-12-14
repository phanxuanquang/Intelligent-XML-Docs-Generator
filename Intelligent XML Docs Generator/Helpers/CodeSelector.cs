using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;

namespace Intelligent_XML_Docs_Generator.Helpers
{
    public static class CodeSelector
    {
        public static async Task<string> GetSelectedTextAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(); // Đảm bảo chạy trên UI thread

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte?.ActiveDocument?.Selection is TextSelection textSelection)
            {
                return textSelection.Text;
            }

            return string.Empty; // Trả về chuỗi rỗng nếu không có đoạn code nào được chọn
        }
    }
}
