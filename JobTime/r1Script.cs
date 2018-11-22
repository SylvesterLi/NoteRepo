using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int httpsss = 1;

        private void button1_Click(object sender, EventArgs e)
        {
            //for (int i = 1; i < 104; i++)
            //{
            //    httpsss = "http://www.xfccpx.cn/cc66/r1b.asp?page=" + i;
            //}

            webBrowser1.Navigate(new Uri("http://www.xfccpx.cn/cc66/r1b.asp?page=" + httpsss));
            button1.Text = "当前为第" + httpsss + "页";
            httpsss++;
            if (httpsss == 105)
            {
                MessageBox.Show("！！！！");
                return;
            }

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            string ph_ans = "";
            //HtmlElement ElementCollection = webBrowser1.Document.GetElementById("alist");
            HtmlElementCollection htmlElementCollection = webBrowser1.Document.GetElementsByTagName("div");
            HtmlElementCollection roof = htmlElementCollection[0].GetElementsByTagName("h1");
            HtmlElementCollection ans = htmlElementCollection[0].GetElementsByTagName("p");
            if (ans.Count == 60)
            {
                for (int i = 0; i < 60; i += 6)
                {
                    ph_ans += roof[i / 6].InnerText + "\r\n";
                    ph_ans += ans[i + 0].InnerText + "\r\n";
                    ph_ans += ans[i + 1].InnerText + "\r\n";
                    ph_ans += ans[i + 2].InnerText + "\r\n";
                    ph_ans += ans[i + 3].InnerText + "\r\n";
                    ph_ans += ans[i + 4].InnerText + "\r\n";
                    ph_ans += ans[i + 5].InnerText + "\r\n";
                }


                textBox1.AppendText(ph_ans);
            }
            else
            {
                MessageBox.Show("Not 60! Continue?");
            }


        }
    }
}
