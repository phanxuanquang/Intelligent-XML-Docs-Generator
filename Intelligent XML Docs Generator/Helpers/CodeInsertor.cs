using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;

namespace Intelligent_XML_Docs_Generator.Helpers
{
    public static class CodeInsertor
    {
        public static async Task InsertTextAboveSelectionAsync(string textToInsert)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            var activeDocument = dte?.ActiveDocument;

            if (activeDocument?.Selection is TextSelection textSelection)
            {
                textSelection.LineUp();
                textSelection.Insert("\r\n");
                textSelection.Insert(textToInsert);
                dte.ExecuteCommand("Edit.FormatDocument");
            }
        }
    }
}
