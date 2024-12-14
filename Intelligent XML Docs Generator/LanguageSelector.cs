using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Intelligent_XML_Docs_Generator
{
    public partial class LanguageSelector : Form
    {
        public LanguageSelector()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            LanguageSettings.CurrentLanguage = LanguageComboBox.Text;
            this.Close();
        }

        private void LanguageSelector_Load(object sender, EventArgs e)
        {
            var languages = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Select(c => c.DisplayName)
                .OrderBy(c => c)
                .ToArray();

            LanguageComboBox.Items.AddRange(languages);
            LanguageComboBox.SelectedItem = languages.FirstOrDefault(l => l.Contains("English"));
        }
    }
}
