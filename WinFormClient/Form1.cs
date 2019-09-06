using Reportman;
using Reportman.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormClient
{
    public partial class Form1 : Form
    {
        Logic logic;
        
        public Form1()
        {
            InitializeComponent();
            Log.LogInfo += PrintLog;
        }
        
        public void PrintLog(object sender,string log)
        {
            string l = log;
            BeginInvoke(new Action(() =>
            {
                if (textBox1.Lines.Count() >= 500)
                    textBox1.Text = string.Empty;
                textBox1.AppendText(l + Environment.NewLine);
                textBox1.ScrollToCaret();
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            if (logic != null)
                logic = new Logic();            
            logic.Start();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //还原窗体显示    
                WindowState = FormWindowState.Normal;
                //激活窗体并给予它焦点
                this.Activate();
                //任务栏区显示图标
                this.ShowInTaskbar = true;
                //托盘区图标隐藏
                notifyIcon1.Visible = false;
                textBox1.ScrollToCaret();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                //隐藏任务栏区图标
                this.ShowInTaskbar = false;
                //图标显示在托盘区
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            logic.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否确认退出程序？", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                // 关闭所有的线程
                this.Dispose();
                this.Close();
            }
        }
    }
}
