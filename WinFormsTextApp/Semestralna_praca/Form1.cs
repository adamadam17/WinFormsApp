using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Semestralna_praca
{
    public partial class Form1 : Form
    {
        private string filepath;

        public Form1()
        {
            InitializeComponent();
            filepath = "";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK )
            {
                filepath = openFileDialog1.FileName;
            }
            else
            {
                filepath = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            richTextBox1.Clear();

            if (filepath == "")
            {
                MessageBox.Show("Žiadny súbor nebol vybraný");
                return;
            }
            try
            {

                string text = File.ReadAllText(filepath);
                string originalText = text;
                int pocet_zmien = 0;

                if (checkedListBox1.CheckedItems.Contains("zmena veľkých písmen na malé") &&
                     checkedListBox1.CheckedItems.Contains("zmena malých písmen na veľké"))
                {
                    MessageBox.Show("Nemôžete navzájom aplikovať 'Zmena veľkých písmen na malé' a 'Zmena malých písmen na veľké'.", "Konflikt transformácie", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                    if (checkedListBox1.CheckedItems.Contains("zmena veľkých písmen na malé"))
                    {
                        text = text.ToLower();
                        pocet_zmien += CountChanges(originalText, text);
                    }

                    if (checkedListBox1.CheckedItems.Contains("zmena malých písmen na veľké"))
                    {
                        text = text.ToUpper();
                        pocet_zmien += CountChanges(originalText, text);
                    }
                }

                if (checkedListBox1.CheckedItems.Contains("zmeniť prvé písmeno vety na veľké"))
                {
                    string changedText = Regex.Replace(text, @"(?:[.!?]\s*|^)(\w)", m => m.Value.ToUpper());
                    pocet_zmien += CountChanges(originalText, changedText);
                    text = changedText;
                }

                if (checkedListBox1.CheckedItems.Contains("zmeniť prvé písmeno slova na veľké"))
                {
                    string changedText = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
                    pocet_zmien += CountChanges(originalText, changedText);
                    text = changedText;
                }

                if (checkedListBox1.CheckedItems.Contains("odstránenie diakritiky"))
                {
                    string changedText = RemoveDiacritics(text);
                    pocet_zmien += CountChanges(originalText, changedText);
                    text = changedText;
                }

                SaveModifiedText(text, pocet_zmien);

                richTextBox1.AppendText("Pôvodný text: \n" + originalText + "\n");
                richTextBox1.AppendText("Upravený text: \n" + text + "\n");
                richTextBox1.AppendText("Počet zmien: " + pocet_zmien + "\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Chyba pri spracovaní súboru: " + ex.Message);
            }
        }

        private int CountChanges(string originalText, string modifiedText)
        {
            int changes = 0;
            for (int i = 0; i < originalText.Length; i++)
            {
                if (originalText[i] != modifiedText[i])
                    changes++;
            }
            return changes;
        }
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char znak in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(znak) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(znak);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private void SaveModifiedText(string text, int počet_zmien)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                {
                    sw.Write(text);
                    sw.WriteLine("\nTotal changes: " + počet_zmien);
                }
                MessageBox.Show("File saved successfully at " + saveFileDialog.FileName);
            }
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
