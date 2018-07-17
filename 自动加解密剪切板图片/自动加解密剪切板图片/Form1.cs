using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 自动加解密剪切板图片
{
    public partial class Form1 : Form
    {
        KeyboardHook k_hook;
        static int key = 114514;
        
        public Form1()
        {
            InitializeComponent();
            try
            {
                k_hook = new KeyboardHook();
                k_hook.KeyDownEvent += new System.Windows.Forms.KeyEventHandler(hook_KeyDown);//钩住键按下 
                k_hook.KeyPressEvent += K_hook_KeyPressEvent;
                k_hook.Start();//安装键盘钩子
            }
            catch (Exception)
            {

                MessageBox.Show("键盘钩子加载失败，请在安全软件里同意一下((\n");
            }
           
        }
        private void K_hook_KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            //tb1.Text += e.KeyChar;
            int i = (int)e.KeyChar;
            //  System.Windows.Forms.MessageBox.Show(i.ToString());
        }
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)ModifierKeys == (int)Keys.Alt && (e.KeyValue == 49 || e.KeyValue == 50))
            {
                try
                {
                    int.Parse(T1.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("随机数种子必须为Int32数值");
                    return;
                }
                try
                {
                    if (Clipboard.ContainsImage())//判断剪切板有没有图片
                    {
                        key = int.Parse(T1.Text);
                        string[] files = new string[Clipboard.GetFileDropList().Count];
                        Clipboard.GetFileDropList().CopyTo(files, 0);
                        foreach (string file in files)
                        { }
                        // Image img = Bitmap.FromFile(files[files.Length-1]);
                        Bitmap BB = new Bitmap(files[files.Length - 1]);
                        if (BB != null)
                        {
                            int Width = BB.Width;
                            int Height = BB.Height;

                            if (e.KeyValue == 49)
                            {

                                Stopwatch watch = new Stopwatch();
                                watch.Start();
                                int[] list = GetRandomIntValues(Width * Height);
                                watch.Stop();
                                Console.WriteLine(watch.Elapsed.TotalSeconds);

                                Bitmap newb = new Bitmap(Width, Height);
                                for (int i = 0; i < list.Length; i++)
                                {
                                    newb.SetPixel((i % Width), (i / Width), BB.GetPixel((list[i] % Width), (list[i] / Width)));

                                }
                                Image img = newb;
                                Clipboard.SetImage(img);
                                // MessageBox.Show("49");
                            }
                            else
                            {
                                int[] list = GetRandomIntValues(Width * Height);
                                Bitmap newb = new Bitmap(Width, Height);
                                for (int i = 0; i < list.Length; i++)
                                {
                                    newb.SetPixel((list[i] % Width), (list[i] / Width), BB.GetPixel((i % Width), (i / Width)));

                                }
                                Image img = newb;
                                Clipboard.SetImage(img);

                                //    MessageBox.Show("50");
                            }
                            Random A = new Random();
                            // Clipboard.SetImage(img);
                        }
                        else
                        {
                            MessageBox.Show("null");
                        }
                    }
                    else
                    {
                        MessageBox.Show("剪切板里没有图片");
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("emmmm出现了未知的错误，联系一下我(作者)我来继续魔改魔改((");
                }
               
            }
        }

        private static string GetBase64(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }


        private static string BitmapGetBase64(string file)
        {
            System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(file);
            using (MemoryStream ms1 = new MemoryStream())
            {
                bmp1.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr1 = new byte[ms1.Length];
                ms1.Position = 0;
                ms1.Read(arr1, 0, (int)ms1.Length);
                ms1.Close();
                return Convert.ToBase64String(arr1);
            }
        }

        private static Bitmap usBase64(string file)
        {
            byte[] arr = Convert.FromBase64String(file);
            using (MemoryStream ms = new MemoryStream(arr))
            {
                Bitmap bmp = new Bitmap(ms);
                return bmp;
            }
        }
        #region DES 加密解密

        /// <summary>
        ///  DES 加密
        /// </summary>
        /// <param name="source">原文</param>
        /// <param name="keyVal">秘钥</param>
        /// <param name="ivVal">向量</param>
        /// <returns></returns>
        public static string Des(string source, string keyVal, string ivVal)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(source);
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
            catch { return "转换出错！"; }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="source">原文</param>
        /// <param name="keyVal">秘钥</param>
        /// <param name="ivVal">向量</param>
        public static string UnDes(string source, string keyVal, string ivVal)
        {
            try
            {
                string[] sInput = source.Split("-".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                var des = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(keyVal.Length > 8 ? keyVal.Substring(0, 8) : keyVal), IV = Encoding.ASCII.GetBytes(ivVal.Length > 8 ? ivVal.Substring(0, 8) : ivVal) };
                var desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
            catch { return "解密出错！"; }
        }

        #endregion

        private int[] GetRandomIntValues(int expectedCouont)
        {
            Dictionary<int, int> container = new Dictionary<int, int>(expectedCouont);
            Random r = new Random(key);
            while (container.Count < expectedCouont)
            {
                int value = r.Next(0, expectedCouont);
                if (!container.ContainsKey(value))
                {
                    container.Add(value, value);
                }
            }
            int[] result = new int[expectedCouont];
            container.Values.CopyTo(result, 0);
            return result;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// 执行JS
        /// </summary>
        /// <param name="sCode">代码的字符串</param>
        /// <param name="method">方法名</param>
        /// <param name="sExpression">参数体</param>
        /// <param name="language">脚本语言（JScript/VBScript）</param>
        /// <returns>返回值</returns>
        public static string ExecuteScript(string sCode, string method, string sExpression, string language)
        {
            MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = false;
            scriptControl.Language = language;
            scriptControl.AllowUI = true;
            scriptControl.AddCode(sCode);

            object[] param = sExpression.Split(',');

            object[] param1 = new object[param.Length];
            for (int i = 0; i < param.Length; i++)
            {
                param1[i] = param[i];
            }
            try
            {
                string str1 = scriptControl.Run(method, param1).ToString();
                return str1;
            }
            catch (COMException comex)
            {
                string str = comex.Message;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }
        Form frmOpener;
        private void Form1_Load(object sender, EventArgs e)
        {

            this.Hide(); //这里只是隐藏主窗口，后台在默默加载数据。
          
        }
        public void ShowLogin(Form frmMain)
        {

            frmOpener = frmMain;
            this.Show();
        }

        private void T1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
