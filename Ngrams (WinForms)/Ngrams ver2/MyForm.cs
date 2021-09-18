using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Ngrams_ver2
{
    class MyForm : Form
    {
        private string currText;
        private Dictionary<string, string> dictFrequency;
        public MyForm()
        {
            this.Text = "N-граммная модель продолжения текста"; 
            var boxLabel = new Label
            {
                Text = "Введите одно или несколько слов: ",
                //Dock = DockStyle.Fill
            };
            var box = new TextBox
            {

            };
            var buttonReadTxt = new Button
            {
                Text = "Открыть",
            };
            var buttonGenerate = new Button
            {
                Text = "Продолжить",
            };
            var outputLabel = new Label
            {
                Text = "Здесь будет выведено продолжение фразы"
            };
            var openedFileLabel = new Label
            {
                Text = "Не открыто ни одного файла"
            };
            var lengthLabel = new Label
            {
                Text = "Введите максимальное количество слов в продолжении фразы"
            };
            var lengthTextBox = new TextBox
            {

            };

            Controls.Add(boxLabel);
            Controls.Add(box);
            Controls.Add(buttonGenerate);
            Controls.Add(outputLabel);
            Controls.Add(buttonReadTxt);
            Controls.Add(openedFileLabel);
            Controls.Add(lengthLabel);
            Controls.Add(lengthTextBox);

            SizeChanged += (sender, args) =>
            {
                var height = 30;

                boxLabel.Location = new Point(0, 0);
                boxLabel.Size = new Size(ClientSize.Width, height);
                box.Location = new Point(0, boxLabel.Bottom);
                box.Size = boxLabel.Size;
                lengthLabel.Location = new Point(0, box.Bottom);
                lengthLabel.Size = new Size(ClientSize.Width / 2, height);
                lengthTextBox.Location = new Point(lengthLabel.Size.Width, box.Bottom);
                lengthTextBox.Size = lengthLabel.Size;
                buttonGenerate.Location = new Point(0, lengthTextBox.Bottom);
                buttonGenerate.Size = new Size(ClientSize.Width, height * 3);
                outputLabel.Location = new Point(0, buttonGenerate.Bottom);
                outputLabel.Size = buttonGenerate.Size;
                buttonReadTxt.Location = new Point(0, ClientSize.Height - height * 4);
                buttonReadTxt.Size = buttonGenerate.Size;
                openedFileLabel.Location = new Point(0, ClientSize.Height - height);
                openedFileLabel.Size = boxLabel.Size;
            };

            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);


            buttonReadTxt.Click += (sender, args) => 
            {
                var tuple = Open();
                currText = tuple.Item1;
                var sentences = Parser.ParseSentences(currText);
                dictFrequency = Analyzer.GetMostFrequentNextWords(sentences);
                openedFileLabel.Text = tuple.Item2;
            };

            buttonGenerate.Click += (sender, args) =>
            {
                var beginning = box.Text;
                string phrase = "";
                if (!string.IsNullOrEmpty(beginning) && currText != null)
                {
                    if (int.TryParse(lengthTextBox.Text, out _))
                        phrase = Generator.ContinuePhrase(dictFrequency, beginning.ToLower(),
                            int.Parse(lengthTextBox.Text));
                    else phrase = Generator.ContinuePhrase(dictFrequency, beginning.ToLower(), 10);
                }
                outputLabel.Text = phrase;
            };
        }

        private (string, string) Open()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                var fileStream = openFileDialog.OpenFile();
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    fileContent = reader.ReadToEnd();
                }
            }
            return (fileContent, filePath);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "MyForm";
            this.Load += new System.EventHandler(this.MyForm_Load);
            this.ResumeLayout(false);

        }

        private void MyForm_Load(object sender, EventArgs e)
        {

        }
    }
}
