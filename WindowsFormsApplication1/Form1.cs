using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Management;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using Printer;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static string MACIP;
        public string checkstr;
        Printer p = new Printer();
        //public int PrintCnt = 0;
        #region 控件缩放
        double formWidth;//窗体原始宽度
        double formHeight;//窗体原始高度
        double scaleX;//水平缩放比例
        double scaleY;//垂直缩放比例
        Dictionary<string, string> controlInfo = new Dictionary<string, string>();
        //控件中心Left,Top,控件Width,控件Height,控件字体Size
        /// <summary>
        /// 获取所有原始数据
        /// </summary>
        protected void GetAllInitInfo(Control CrlContainer)
        {
            if (CrlContainer.Parent == this)
            {
                formWidth = Convert.ToDouble(CrlContainer.Width);
                formHeight = Convert.ToDouble(CrlContainer.Height);
            }
            foreach (Control item in CrlContainer.Controls)
            {
                if (item.Name.Trim() != "")
                    controlInfo.Add(item.Name, (item.Left + item.Width / 2) + "," + (item.Top + item.Height / 2) + "," + item.Width + "," + item.Height + "," + item.Font.Size);
                if ((item as UserControl) == null && item.Controls.Count > 0)
                    GetAllInitInfo(item);
            }
        }
        private void ControlsChangeInit(Control CrlContainer)
        {
            scaleX = (Convert.ToDouble(CrlContainer.Width) / formWidth);
            scaleY = (Convert.ToDouble(CrlContainer.Height) / formHeight);
        }
        private void ControlsChange(Control CrlContainer)
        {
            double[] pos = new double[5];//pos数组保存当前控件中心Left,Top,控件Width,控件Height,控件字体Size
            foreach (Control item in CrlContainer.Controls)
            {
                if (item.Name.Trim() != "")
                {
                    if ((item as UserControl) == null && item.Controls.Count > 0)
                        ControlsChange(item);
                    string[] strs = controlInfo[item.Name].Split(',');
                    for (int j = 0; j < 5; j++)
                    {
                        pos[j] = Convert.ToDouble(strs[j]);
                    }
                    double itemWidth = pos[2] * scaleX;
                    double itemHeight = pos[3] * scaleY;
                    item.Left = Convert.ToInt32(pos[0] * scaleX - itemWidth / 2);
                    item.Top = Convert.ToInt32(pos[1] * scaleY - itemHeight / 2);
                    item.Width = Convert.ToInt32(itemWidth);
                    item.Height = Convert.ToInt32(itemHeight);
                    try
                    {
                        item.Font = new Font(item.Font.Name, float.Parse((pos[4] * Math.Min(scaleX, scaleY)).ToString()));
                    }
                    catch
                    {
                    }
                }
            }
        }




        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (controlInfo.Count > 0)
            {
                ControlsChangeInit(this.Controls[0]);
                ControlsChange(this.Controls[0]);
            }
        }
        #endregion
        #region 获取MAC地址        
        //public static string GetNetworkAdpaterID()

        //{

        //    //try

        //    //{

        //    string mac = "";

        //    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");

        //    ManagementObjectCollection moc = mc.GetInstances();

        //    foreach (ManagementObject mo in moc)

        //        if ((bool)mo["IPEnabled"] == true)

        //        {

        //            mac += mo["MacAddress"].ToString() + " ";

        //            break;

        //        }

        //    moc = null;

        //    mc = null;
        //    MACIP = mac.Trim();

        //    return MACIP;

        //    //}

        //    //catch (Exception e)

        //    //{

        //    //    return "uMnIk";

        //    //}

        //}
        #endregion
        public Form1()
        {
            InitializeComponent();
            
            WriteFile((System.Environment.CurrentDirectory + "\\para.txt"));
            Read_para(System.Environment.CurrentDirectory + "\\para.txt");
            this.textBox1.Focus();
            Control.CheckForIllegalCrossThreadCalls = false;
            GetAllInitInfo(this.Controls[0]);
            //GetNetworkAdpaterID();//获取MAC
            //Read(System.Environment.CurrentDirectory + "\\sys.bat");//读取txt

            //if (checkstr == "")
            //{
            //    WriteMAC((System.Environment.CurrentDirectory + "\\sys.bat"), MACIP);
            //}
            //else if (checkstr != MACIP)
            //{
            //    MessageBox.Show("非法使用该软件，请联系作者！", "警告");

            //    this.Close();
            //}
            textBox1.Focus();
            p.Printer_Print += FrmMain_Printer_Print;
            Write((System.Environment.CurrentDirectory + "\\N309history.txt"), "---------------------Start------------------------------");
        }       
        private void button2_Click(object sender, EventArgs e)
        {
            var barCode = new BarCode();
            if(checkBox1.Checked)
            {
                barCode.ValueFont = new Font(textBox4.Text,Convert.ToSingle(textBox5.Text),FontStyle.Bold);
            }
            else
            {
                barCode.ValueFont = new Font(textBox4.Text, Convert.ToSingle(textBox5.Text));
            }
            
            pictureBox1.Image = barCode.GetCodeImage(textBox1.Text.ToUpper(), BarCode.Encode.Code128A);
            pictureBox2.Image = QRCode.GetQRCode(textBox1.Text);
            Write((System.Environment.CurrentDirectory + "\\N309history.txt"), DateTime.Now.ToString()+" : "+textBox1.Text);
            
            int Len = textBox1.Text.Length;
            if(Len!=17)
            {
                MessageBox.Show( "SN码错误，请重新扫码！","错误");
            }
            else
            {
                p.Print("", 500, 120, 300);
            }
            textBox1.Focus();
            textBox1.SelectAll();
        }
        void FrmMain_Printer_Print(Graphics g)
        {
            g.DrawImage(this.pictureBox2.Image, new Rectangle(20+(Convert.ToInt32(textBox2.Text)), 7+ (Convert.ToInt32(textBox3.Text)), 100, 100));
            g.DrawImage(this.pictureBox1.Image, new Rectangle(140 + (Convert.ToInt32(textBox2.Text)), 17+ (Convert.ToInt32(textBox3.Text)), 250, 80));
            //g.DrawImage(pictureBox1.Image, new Point(120, 5));//起始位置参数
            //g.DrawImage(pictureBox2.Image, new Point(5, 5));//起始位置参数
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = System.Environment.CurrentDirectory + "\\N309history.txt";
            if (filePath.Equals(""))
            {
                MessageBox.Show("路径不能为空！", "操作提示");
                return;
            }
            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("指定文件不存在！", "操作提示");
                return;
            }
            System.Diagnostics.Process.Start("explorer.exe", filePath);
        }
        #region TXT
        public void Write(string path, string TxtData)
        {
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.WriteLine(TxtData);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
        #endregion
        public void WriteMAC(string path, string TxtData)
        {
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(TxtData);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
        public void Read(string path)
        {
            //FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(path, Encoding.Default);
            checkstr = File.ReadAllText(path);
            sr.Close();
        }

        public void Read_para(string path)
        {
            string Check;
            
            StreamReader sr = new StreamReader(path, Encoding.Default);
            textBox4.Text = sr.ReadLine();
            textBox5.Text = sr.ReadLine();
            textBox3.Text = sr.ReadLine();
            textBox2.Text = sr.ReadLine();
            Check = sr.ReadLine();
            if (Check == "1")
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
            sr.Close();
            
        }
        public void Write_para(string path)
        {
            string Check;
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            //开始写入
            sw.WriteLine(textBox4.Text);
            sw.WriteLine(textBox5.Text);
            sw.WriteLine(textBox3.Text);
            sw.WriteLine(textBox2.Text);
            if (checkBox1.Checked)
                Check = Convert.ToString(1);
            else
                Check = Convert.ToString(0);
            sw.WriteLine(Check);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
        public static void WriteFile(string Path)
        {
            if (!System.IO.File.Exists(Path))
            {
                System.IO.FileStream f = System.IO.File.Create(Path);
                f.Close();
                f.Dispose();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);

            f2.Close();
            f2.Dispose();

        }
        private void button4_Click(object sender, EventArgs e)
        {
            Write_para(System.Environment.CurrentDirectory + "\\para.txt");
            MessageBox.Show("写入成功", "提示");
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {

            if/*(false)// */(tabControl1.SelectedTab.Name == "调试信息") //参数设置
            {
                Form2 Form = new Form2();
                DialogResult result;
                result = Form.ShowDialog();
                Form.Left = this.Left - Form.Width;
                Form.Top = this.Top;
                Form.Close();
                if (result == DialogResult.OK)
                {

                    MessageBox.Show("登陆成功");
                    tabControl1.SelectedIndex = 1;

                }
                else if (result == DialogResult.Cancel)
                {
                    MessageBox.Show("您是来逗我的吗？");
                    tabControl1.SelectedIndex = 0;
                }
                else
                {
                    tabControl1.SelectedIndex = 0;
                }
            }
        }
            
    }    
}
