using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class TermFormBase : Form
    {
        protected TermDatabase termDatabase;
        protected TextBox nameTextBox;
        protected TextBox definitionTextBox;
        protected CheckedListBox referencesCheckedListBox;
        protected TextBox tagsTextBox;
        protected Button saveButton;
        protected Font defaultFont;

        public TermFormBase(TermDatabase db)
        {
            termDatabase = db;

            defaultFont = new Font("Arial", 12);
            nameTextBox = new TextBox { Left = 10, Top = 10, Width = 200, Font = defaultFont };
            definitionTextBox = new TextBox { Left = 10, Top = 50, Width = 200, Height = 100, Multiline = true, Font = defaultFont };
            referencesCheckedListBox = new CheckedListBox { Left = 10, Top = 160, Width = 200, Height = 100, Font = defaultFont };
            tagsTextBox = new TextBox { Left = 10, Top = 270, Width = 200, Font = defaultFont };
            saveButton = new Button { Text = "Зберегти", Left = 10, Top = 310, Width = 100, Height = 40, Font = defaultFont };

            saveButton.Click += SaveButton_Click;

            Controls.Add(new Label { Text = "Назва терміну", Left = 220, Top = 10, Font = defaultFont });
            Controls.Add(nameTextBox);
            Controls.Add(new Label { Text = "Визначення", Left = 220, Top = 50, Font = defaultFont });
            Controls.Add(definitionTextBox);
            Controls.Add(new Label { Text = "Посилання", Left = 220, Top = 160, Font = defaultFont });
            Controls.Add(referencesCheckedListBox);
            Controls.Add(new Label { Text = "Теги", Left = 220, Top = 270, Font = defaultFont });
            Controls.Add(tagsTextBox);
            Controls.Add(saveButton);

            this.MinimumSize = new Size(400, 400);

            LoadExistingTerms();
        }

        public static void MissingFields(string name, string definition, string tags)
        {
            bool nameMissing = string.IsNullOrWhiteSpace(name);
            bool definitionMissing = string.IsNullOrWhiteSpace(definition);
            bool tagsMissing =  string.IsNullOrWhiteSpace(tags);

            if (nameMissing || definitionMissing || tagsMissing)
            {
                string errorMessage = "Відсутні наступні поля:\n";

                if (nameMissing)
                    errorMessage += "- Назва\n";
                if (definitionMissing)
                    errorMessage += "- Визначення\n";
                if (tagsMissing)
                    errorMessage += "- Теги (потрібен хоча б один тег)\n";

                MessageBox.Show(errorMessage, "Відсутні поля", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected virtual void SaveButton_Click(object sender, EventArgs e)
        {
        }

        private void LoadExistingTerms()
        {
            foreach (var term in termDatabase.GetAllTerms())
            {
                referencesCheckedListBox.Items.Add(term.Name);
            }
        }
    }
}
