using Newtonsoft.Json;

namespace CourseWork
{
    public partial class MainForm : Form
    {
        private TermDatabase termDatabase;
        private ListBox termListBox;
        private TextBox searchTextBox;
        private ComboBox tagSearchComboBox;
        private CheckedListBox tagCheckedListBox;
        private Button addTermButton;
        private Button deleteTermButton;
        private Button editTermButton;
        private Panel termDetailsPanel;
        private Font defaultFont;
        private Button saveFilteredButton;
        private Button loadFilteredButton;
        private const string DataFilePath = "terms.json";

        public MainForm(TermDatabase db)
        {
            termDatabase = db;

            defaultFont = new Font("Arial", 12);

            termListBox = new ListBox { Left = 10, Top = 10, Width = 300, Height = 400, Font = defaultFont };
            termListBox.SelectedIndexChanged += TermListBox_SelectedIndexChanged;

            searchTextBox = new TextBox { Left = 320, Top = 10, Width = 200, Font = defaultFont };
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            tagSearchComboBox = new ComboBox { Left = 530, Top = 10, Width = 90, DropDownStyle = ComboBoxStyle.Simple, Height = 26, Font = defaultFont };
            tagSearchComboBox.Click += TagSearchComboBox_Click;
            tagSearchComboBox.KeyPress += TagSearchComboBox_KeyPress;

            tagCheckedListBox = new CheckedListBox { Left = 530, Top = 40, Width = 120, Height = 130, Visible = false, HorizontalScrollbar = true, Font = defaultFont };
            tagCheckedListBox.ItemCheck += TagCheckedListBox_ItemCheck;

            addTermButton = new Button { Text = "Додати", Left = 10, Top = 420, Width = 130, Height = 40, Font = defaultFont };
            addTermButton.Click += AddTermButton_Click;

            deleteTermButton = new Button { Text = "Видалити", Left = 180, Top = 420, Width = 130, Height = 40, Font = defaultFont };
            deleteTermButton.Click += DeleteTermButton_Click;

            editTermButton = new Button { Text = "Редагувати", Left = 95, Top = 475, Width = 130, Height = 40, Font = defaultFont };
            editTermButton.Click += EditTermButton_Click;

            termDetailsPanel = new Panel { Left = 320, Top = 40, Width = 300, Height = 370, AutoScroll = true, Font = defaultFont };

            saveFilteredButton = new Button { Text = "Зберегти список", Left = 320, Top = 480, Width = 150, Height = 40, Font = defaultFont };
            saveFilteredButton.Click += SaveFilteredButton_Click;

            loadFilteredButton = new Button { Text = "Відкрити список", Left = 480, Top = 480, Width = 150, Height = 40, Font = defaultFont };
            loadFilteredButton.Click += LoadFilteredButton_Click;

            Controls.Add(termListBox);
            Controls.Add(searchTextBox);
            Controls.Add(tagSearchComboBox);
            Controls.Add(tagCheckedListBox);
            Controls.Add(addTermButton);
            Controls.Add(deleteTermButton);
            Controls.Add(editTermButton);
            Controls.Add(termDetailsPanel);
            Controls.Add(saveFilteredButton);
            Controls.Add(loadFilteredButton);

            this.MinimumSize = new Size(670, 580);

            LoadDatabase();
            LoadTerms();
            LoadTags();

            this.FormClosing += MainForm_FormClosing;
        }

        private void LoadTerms()
        {
            termListBox.Items.Clear();
            foreach (var term in termDatabase.GetAllTerms())
            {
                termListBox.Items.Add(term.Name);
            }
        }

        private void LoadTags()
        {
            tagCheckedListBox.Items.Clear();
            var tags = termDatabase.GetAllTerms().SelectMany(t => t.Tags).Distinct().ToList();
            foreach (var tag in tags)
            {
                tagCheckedListBox.Items.Add(tag);
            }
        }

        private void SelectTerm(string termName)
        {
            termListBox.SelectedItem = termName;
        }

        private void TermListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            termDetailsPanel.Controls.Clear();
            if (termListBox.SelectedItem != null)
            {
                var selectedTerm = termDatabase.GetAllTerms().FirstOrDefault(t => t.Name == termListBox.SelectedItem.ToString());
                if (selectedTerm != null)
                {
                    int top = 0;
                    termDetailsPanel.Controls.Add(new Label { Text = $"Назва терміну: {selectedTerm.Name}", Top = top, Left = 0, Width = termDetailsPanel.Width });
                    top += 25;

                    var definitionTextBox = new TextBox { Text = selectedTerm.Definition, Top = top, Left = 0, Width = termDetailsPanel.Width, Height = 100, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };
                    definitionTextBox.Enter += (s, e) => { definitionTextBox.Parent.Focus(); };
                    termDetailsPanel.Controls.Add(definitionTextBox);
                    top += 120;

                    var referencesLabel = new Label { Text = "Посилання:", Top = top, Left = 0, Width = termDetailsPanel.Width };
                    termDetailsPanel.Controls.Add(referencesLabel);
                    top += 20;

                    foreach (var reference in selectedTerm.References)
                    {
                        var linkLabel = new LinkLabel { Text = reference, Top = top, Left = 20, Width = termDetailsPanel.Width, AutoSize = true };
                        linkLabel.Click += (s, ev) => ReferenceLinkClicked(reference);
                        termDetailsPanel.Controls.Add(linkLabel);
                        top += 20;
                    }

                    var tagsLabel = new Label { Text = $"Теги: {string.Join(", ", selectedTerm.Tags)}", Top = top + 10, Left = 0, Width = termDetailsPanel.Width, Height = 60 };
                    termDetailsPanel.Controls.Add(tagsLabel);
                }
            }
        }

        private void ReferenceLinkClicked(string reference)
        {
            for (int i = 0; i < tagCheckedListBox.Items.Count; i++)
            {
                tagCheckedListBox.SetItemChecked(i, false);
            }
            searchTextBox.Text = string.Empty;
            LoadTerms();
            termListBox.SelectedItem = reference;
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterTerms();
        }

        private void TagSearchComboBox_Click(object sender, EventArgs e)
        {
            tagCheckedListBox.Visible = !tagCheckedListBox.Visible;
        }

        private void TagSearchComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void TagCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate {
                UpdateTagSearchComboBox();
                FilterTerms();
            });
        }

        private void UpdateTagSearchComboBox()
        {
            var selectedTags = tagCheckedListBox.CheckedItems.Cast<string>().ToList();
            tagSearchComboBox.Items.Clear();
            if (selectedTags.Any())
            {
                tagSearchComboBox.Items.Add(string.Join(", ", selectedTags));
                tagSearchComboBox.SelectedIndex = 0;
            }
            else
            {
                tagSearchComboBox.Text = string.Empty;
            }
        }

        private void FilterTerms()
        {
            var searchText = searchTextBox.Text.ToLower();
            var selectedTags = tagCheckedListBox.CheckedItems.Cast<string>().ToList();

            termListBox.Items.Clear();
            foreach (var term in termDatabase.SearchTerms(searchText))
            {
                if (!selectedTags.Any() || selectedTags.All(t => term.Tags.Contains(t)))
                {
                    termListBox.Items.Add(term.Name);
                }
            }
        }

        private void AddTermButton_Click(object sender, EventArgs e)
        {
            var addTermForm = new AddTermForm(termDatabase);
            addTermForm.ShowDialog();
            LoadTerms();
            LoadTags();
            SelectTerm(addTermForm.newName);
        }

        private void DeleteTermButton_Click(object sender, EventArgs e)
        {
            if (termListBox.SelectedItem != null)
            {
                termDatabase.DeleteTerm(termListBox.SelectedItem.ToString());
                termListBox.ClearSelected();
                LoadTerms();
                LoadTags();
            }
        }

        private void EditTermButton_Click(object sender, EventArgs e)
        {
            if (termListBox.SelectedItem != null)
            {
                var selectedTerm = termDatabase.GetAllTerms().FirstOrDefault(t => t.Name == termListBox.SelectedItem.ToString());
                if (selectedTerm != null)
                {
                    var editTermForm = new EditTermForm(termDatabase, selectedTerm);
                    editTermForm.ShowDialog();
                    LoadTerms();
                    LoadTags();
                    SelectTerm(editTermForm.existingTerm.Name);
                }
            }
        }

        private void SaveFilteredTerms()
        {
            var filteredTerms = new List<Term>();
            foreach (var item in termListBox.Items)
            {
                var term = termDatabase.GetAllTerms().FirstOrDefault(t => t.Name == item.ToString());
                if (term != null)
                {
                    filteredTerms.Add(term);
                }
            }

            using (var saveFileDialog = new SaveFileDialog { Filter = "JSON files (*.json)|*.json" })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = saveFileDialog.FileName;
                    var json = JsonConvert.SerializeObject(filteredTerms, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
            }
        }

        private void LoadFilteredTerms()
        {
            using (var openFileDialog = new OpenFileDialog { Filter = "JSON files (*.json)|*.json" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    var json = File.ReadAllText(filePath);
                    var filteredTerms = JsonConvert.DeserializeObject<List<Term>>(json);

                    termListBox.Items.Clear();
                    foreach (var term in filteredTerms)
                    {
                        termListBox.Items.Add(term.Name);
                    }
                }
            }
        }

        private void LoadDatabase()
        {
            termDatabase.LoadFromFile(DataFilePath);
        }

        private void SaveDatabase()
        {
            termDatabase.SaveToFile(DataFilePath);
        }

        private void SaveFilteredButton_Click(object sender, EventArgs e)
        {
            SaveFilteredTerms();
        }

        private void LoadFilteredButton_Click(object sender, EventArgs e)
        {
            LoadFilteredTerms();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDatabase();
        }
    }
}