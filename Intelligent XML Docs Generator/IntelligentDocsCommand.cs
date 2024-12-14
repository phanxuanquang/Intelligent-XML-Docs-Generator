using EnvDTE;
using Intelligent_Generator;
using Intelligent_XML_Docs_Generator.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace Intelligent_XML_Docs_Generator
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class IntelligentDocsCommand
    {
        public const int GenerateXmlDocsCommandId = 0x0100;
        public const int ChooseLanguageCommandId = 0x0200;

        public static readonly Guid CommandSet = new Guid("0934180d-f5cc-4410-99bd-d87b357c5253");

        private readonly AsyncPackage package;
        private IntelligentDocsCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, GenerateXmlDocsCommandId);
            var menuItem = new MenuCommand(this.GenerateXmlDocs, menuCommandID);
            commandService.AddCommand(menuItem);

            var menuCommandIDChooseLanguage = new CommandID(CommandSet, ChooseLanguageCommandId);
            var menuItemChooseLanguage = new MenuCommand(ChooseLanguage, menuCommandIDChooseLanguage);
            commandService.AddCommand(menuItemChooseLanguage);
        }
        public static IntelligentDocsCommand Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new IntelligentDocsCommand(package, commandService);
        }


        private async void GenerateXmlDocs(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte?.ActiveDocument != null)
            {
                string language = dte.ActiveDocument.Language;

                if (language.Equals("CSharp", StringComparison.OrdinalIgnoreCase))
                {
                    var selectedCode = await CodeSelector.GetSelectedTextAsync();
                    if (string.IsNullOrWhiteSpace(selectedCode))
                    {
                        VsShellUtilities.ShowMessageBox(
                            package,
                            "No code selected. Please select some code and try again.",
                            "Intelligent XML Docs Generator",
                            OLEMSGICON.OLEMSGICON_WARNING,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                        return;
                    }

                    var instruction = await ContentReader.GetPromptContentAsync();
                    try
                    {
                        var xmlDocs = await Generator.GenerateContent("", instruction, $"## C# Code Snipet\n\n```code\n{selectedCode}\n```\n\n## Language for the XML documentation:\n\n- {LanguageSettings.CurrentLanguage}", 20);

                        if (!string.IsNullOrWhiteSpace(xmlDocs))
                        {
                            if (xmlDocs.StartsWith("```"))
                            {
                                xmlDocs = xmlDocs.Substring(6, xmlDocs.Length - 10).Trim();
                            }

                            await CodeInsertor.InsertTextAboveSelectionAsync(xmlDocs);
                        }
                    }
                    catch (Exception ex)
                    {
                        VsShellUtilities.ShowMessageBox(
                            package,
                            $"An error occurred while generating XML documentation: {ex.Message}",
                            "Intelligent XML Docs Generator",
                            OLEMSGICON.OLEMSGICON_CRITICAL,
                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                    }
                }
                else
                {
                    VsShellUtilities.ShowMessageBox(
                        package,
                        "This feature is only available for C# files.",
                        "Invalid File Type",
                        OLEMSGICON.OLEMSGICON_WARNING,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }

        private void ChooseLanguage(object sender, EventArgs e)
        {
            using (var languageSelector = new LanguageSelector())
            {
                languageSelector.ShowDialog();
            }
        }
    }
}
