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
                        Bitmap BB=new Bitmap(1,1);
                        // Image img = Bitmap.FromFile(files[files.Length-1]);
                        try
                        {
                            BB = new Bitmap(Clipboard.GetImage());
                        }
                        catch (Exception)
                        {
                            try
                            {
                                BB = new Bitmap(files[files.Length - 1]);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("好像碰到了啥暂不支持的方式或者格式..");

                            }
                           
                          
                        }
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

                              //  img.Save("./s.Gif", System.Drawing.Imaging.ImageFormat.Gif);
                                //Clipboard.SetDataObject(img);
                                //DataFormats.Format format = DataFormats.GetFormat("GIF");
                                //Clipboard.SetData(format.Name,img);

                                 Clipboard.SetImage(img);
                                //Clipboard.SetImage(new Bitmap("./s.gif"));
                                img.Dispose();
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
                                //img.Save("./s.Gif", System.Drawing.Imaging.ImageFormat.Gif);
                                //Bitmap newc = new Bitmap("./s.Gif");
                                Clipboard.SetImage(img);
                                // Clipboard.SetImage(new Bitmap("./s.gif"));
                                img.Dispose();
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
       
        private void Form1_Load(object sender, EventArgs e)
        {

          
          
        }
      

        private void T1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
