using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskCancellationTokenApp
{
    public partial class Form1 : Form
    {
        //CancellationToken: Bir asenkron işlemi uzun süre cevap verilmediği zaman durdurabilmesini sağlar.
        //Task başlatıldığında tutulan Token üzerinden durdurulmasını sağlar.
        CancellationTokenSource ct = new CancellationTokenSource();
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            Task<HttpResponseMessage> myTask;

            try
            {
                myTask = new HttpClient().GetAsync("https://localhost:44385/api/home", ct.Token);

                await myTask;

                var content = await myTask.Result.Content.ReadAsStringAsync();

                richTextBox1.Text = content;
            }
            catch (TaskCanceledException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ct.Cancel();
        }
    }
}
