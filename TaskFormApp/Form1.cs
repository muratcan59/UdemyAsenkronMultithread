using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskFormApp
{
    public partial class Form1 : Form
    {
        public int counter { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private async void BtnReadFile_Click(object sender, EventArgs e)
        {
            string data = String.Empty;

            Task<String> okuma = ReadFileAsync2();

            richTextBox2.Text = await new HttpClient().GetStringAsync("https://www.google.com");

            data = await okuma;

            richTextBox1.Text = data;
        }

        private void BtnCounter_Click(object sender, EventArgs e)
        {
            textBoxCounter.Text = counter++.ToString();
        }

        private string ReadFile()
        {
            string data = string.Empty;
            using (StreamReader s = new StreamReader("dosya.txt"))
            {
                Thread.Sleep(5000);
                data = s.ReadToEnd();
            }

            return data;
        }

        private async Task<string> ReadFileAsync()
        {
            string data = string.Empty;
            using (StreamReader s = new StreamReader("dosya.txt"))
            {
                //Datayı okur(Okuma esnasında başka işlemler yapılabilir)
                Task<string> myTask = s.ReadToEndAsync();

                await Task.Delay(5000);
                //Okunan veri belirtilen alanda çağırılır.
                data = await myTask;
            }

            return data;
        }

        private Task<string> ReadFileAsync2()
        {
            StreamReader s = new StreamReader("dosya.txt");

            return s.ReadToEndAsync();
        }
    }
}