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
    public partial class EditTermForm : TermFormBase
    {
        public Term existingTerm;
        private bool checkedName = false;

        public EditTermForm(TermDatabase db, Term term) : base(db)
        {
            existingTerm = term;

            nameTextBox.Text = existingTerm.Name;
            definitionTextBox.Text = existingTerm.Definition;
            tagsTextBox.Text = string.Join(", ", existingTerm.Tags);
            LoadReferences();

            for (int i = 0; i < referencesCheckedListBox.Items.Count; i++)
            {
                if (existingTerm.References.Contains(referencesCheckedListBox.Items[i].ToString()))
                {
                    referencesCheckedListBox.SetItemChecked(i, true);
                }
            }
        }

        private void LoadReferences()
        {
            referencesCheckedListBox.Items.Clear();
            foreach (var term in termDatabase.GetAllTerms())
            {
                if (term.Name != existingTerm.Name)
                {
                    referencesCheckedListBox.Items.Add(term.Name);
                }
            }
        }

        protected override void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (var termCheck in termDatabase.GetAllTerms())
            {
                if (termCheck.Name == nameTextBox.Text && termCheck.Name != existingTerm.Name)
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
                var oldName = existingTerm.Name;
                var newName = nameTextBox.Text;

                existingTerm.Name = newName;
                existingTerm.Definition = definitionTextBox.Text;
                existingTerm.References = referencesCheckedListBox.CheckedItems.Cast<string>().ToList();
                existingTerm.Tags = tagsTextBox.Text.Split(',').Select(t => t.Trim()).ToList();
                termDatabase.UpdateReferences(oldName, newName);
                this.Close();
            }
        }
    }
}
