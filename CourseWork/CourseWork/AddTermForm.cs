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
    public partial class AddTermForm : TermFormBase
    {
        public string newName;
        private bool checkedName = false;
        public AddTermForm(TermDatabase db) : base(db)
        {
        }

        protected override void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (var termCheck in termDatabase.GetAllTerms())
            {
                if (termCheck.Name == nameTextBox.Text)
                {
                    checkedName = true;
                }
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text) || string.IsNullOrWhiteSpace(definitionTextBox.Text) || string.IsNullOrWhiteSpace(tagsTextBox.Text))
            {
                MissingFields(nameTextBox.Text, definitionTextBox.Text, tagsTextBox.Text);
            }

            else if (checkedName == true)
            {
                MessageBox.Show("Термін з такою назвою вже існує!", "Дублювання назв", MessageBoxButtons.OK, MessageBoxIcon.Error);
                checkedName = false;
            }

            else
            {
                var name = nameTextBox.Text;
                var definition = definitionTextBox.Text;
                var references = referencesCheckedListBox.CheckedItems.Cast<string>().ToList();
                var tags = tagsTextBox.Text.Split(',').Select(t => t.Trim()).ToList();

                var term = new Term(name, definition, references, tags);
                termDatabase.AddTerm(term);
                newName = name;
                this.Close();
            }
        }
    }
}
