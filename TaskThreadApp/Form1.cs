using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskThreadApp
{
    public partial class Form1 : Form
    {
        public static int counter { get; set; } = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            var atask = Go(progressBar1);
            var btask = Go(progressBar2);

            await Task.WhenAll(atask, btask);
        }

        public async Task Go(ProgressBar pb)
        {
            //Run: Çağrılan taskları kuyruğa ekleyip eşzamanlı olarak çalışmasını sağlar.
            await Task.Run(() =>
            {
                Enumerable.Range(1, 100).ToList().ForEach(x =>
                {
                    Thread.Sleep(100);
                    pb.Invoke((MethodInvoker)delegate { pb.Value = x; });
                });
            });
        }

        private void btnCounter_Click(object sender, EventArgs e)
        {
            btnCounter.Text = counter++.ToString();
        }
    }
}
