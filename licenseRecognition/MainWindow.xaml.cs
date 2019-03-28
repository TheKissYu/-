using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Microsoft.Win32;
using System.Drawing; //需要在解决方案的引用中添加“System.Drawing”
using System.Drawing.Imaging;
using System.IO;

namespace licenseRecognition
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap m_Bitmap;//存放最先打开的图片 // 需要在解决方案的引用中添加“System.Drawing”
        private Bitmap always_Bitmap;//永远是彩色图片，用以备用
        private Bitmap other_c_Bitmap;
        private Bitmap extract_Bitmap_one;
        private Bitmap extract_Bitmap_two;
        private Bitmap z_Bitmap0;
        private Bitmap z_Bitmap1;
        private Bitmap z_Bitmap2;
        private Bitmap z_Bitmap3;
        private Bitmap z_Bitmap4;
        private Bitmap z_Bitmap5;
        private Bitmap z_Bitmap6;
        //private Bitmap z_Bitmap7;
        private Bitmap objNewPic;
        private Bitmap c_Bitmap; //车牌图像

        private Bitmap[] z_Bitmaptwo = new Bitmap[7];//用于储存最终的黑白字体

        private Bitmap[] charFont;
        private Bitmap[] provinceFont;
        string[] charString;//存储的路径
        string[] provinceString;//省份字体
        string[] charDigitalString;
        string[] provinceDigitalString;
        System.Drawing.Pen pen1 = new System.Drawing.Pen(System.Drawing.Color.Black);
        private String name;  // pictureName;
        private float count;
        private float[] gl = new float[256];
        int[] gray = new int[256]; //灰度化
        int[] rr = new int[256];
        int[] gg = new int[256];
        int[] bb = new int[256];
        float[,] m = new float[5000, 5000];
        int flag = 0, flag1 = 0;
        int xx = -1;
        //private bool aline = false;
        public static string SourceBathOne = "G:\\licensePlate\\";//备用
        public static string charSourceBath = "MYsource\\char\\";
        public static string provinceSourceBath = "MYsource\\font\\";

        public MainWindow()
        {
            InitializeComponent();
        }



        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {   //打开图片

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jpeg文件(*.jpg)|*.jpg|Bitmap文件(*.bmp)|*.bmp| 所有合适文件(*.bmp/*.jpg)|*.bmp/*.jpg";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;//该值指示对话框在关闭前是否还原当前目录
            if (openFileDialog.ShowDialog() == true)
            {
                name = openFileDialog.FileName;
                //MessageBox.Show(name);
                m_Bitmap = (Bitmap)Bitmap.FromFile(name, false);//使用该文件中的嵌入颜色管理信息，从指定的文件创建m_Bitmap
                this.always_Bitmap = m_Bitmap.Clone(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), PixelFormat.DontCare);

                IntPtr ip = m_Bitmap.GetHbitmap(); //将Bitmap转换为BitmapSource
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                imgLoad.Source = bitmapSource;
            }


        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap文件(*.bmp)|*.bmp| Jpeg文件(*.jpg)|*.jpg| 所有合适文件(*.bmp/*.jpg)|*.bmp/*.jpg";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                m_Bitmap.Save(saveFileDialog.FileName);
            }

        }


        private void btnPictureGray_Click(object sender, RoutedEventArgs e)
        { //灰度化
            if (m_Bitmap != null)
            {
                int tt = 0;
                for (int i = 0; i < 256; i++)//清掉数组gray里的数据
                {
                    gray[i] = 0;
                }
                for (int i = 0; i < 256; i++)//清掉数组rr里的数据
                {
                    rr[i] = 0;
                }
                for (int i = 0; i < 256; i++)//清掉数组gg里的数据
                {
                    gg[i] = 0;
                }
                for (int i = 0; i < 256; i++)//清掉数组bb里的数据
                {
                    bb[i] = 0;
                }
                BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int stride = bmData.Stride;//获取或设置 Bitmap 对象的跨距宽度（也称为扫描宽度）。 
                System.IntPtr Scan0 = bmData.Scan0;//获取或设置位图中第一个像素数据的地址。 它也可以看成是位图中的第一个扫描行
                unsafe  //"生成"选择"允许不安全代码" 
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - m_Bitmap.Width * 3;
                    byte red, green, blue;
                    int nWidth = m_Bitmap.Width;
                    int nHeight = m_Bitmap.Height;
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            blue = p[0];
                            green = p[1];
                            red = p[2];
                            tt = p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);
                            rr[red]++;
                            gg[green]++;
                            bb[blue]++;
                            gray[tt]++;   //统计灰度值为tt的象素点数目  
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                m_Bitmap.UnlockBits(bmData);
                count = m_Bitmap.Width * m_Bitmap.Height;
                IntPtr ip = m_Bitmap.GetHbitmap();//将Bitmap转换为BitmapSource
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                imgLoad.Source = bitmapSource;
            }

        }




        private void btnGrayScales_Click(object sender, RoutedEventArgs e)
        { //传统直方图均衡化 -- 灰度均衡化
            if (m_Bitmap != null)
            {
                BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                //加入内存进行处理
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;//扫描的第一行
                int tt = 0;
                int[] SumGray = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    SumGray[i] = 0;
                }
                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;
                    int nOffset = stride - m_Bitmap.Width * 3;
                    int nHeight = m_Bitmap.Height;
                    int nWidth = m_Bitmap.Width;
                    SumGray[0] = gray[0];//灰度均衡化 
                    for (int i = 1; i < 256; ++i)//灰度级频度数累加                         
                        SumGray[i] = SumGray[i - 1] + gray[i];
                    for (int i = 0; i < 256; ++i) //计算调整灰度值     频率乘以灰度总级数得出该灰度变换后的灰度级                   
                        SumGray[i] = (int)(SumGray[i] * 255 / count);
                    for (int i = 0; i < 256; i++)
                    {
                        gray[i] = 0;
                    }
                    for (int y = 0; y < nHeight; ++y)
                    {
                        for (int x = 0; x < nWidth; ++x)
                        {
                            tt = p[0] = p[1] = p[2] = (byte)(SumGray[p[0]]);
                            gray[tt]++;
                            p += 3;
                        }
                        p += nOffset;
                    }
                }
                m_Bitmap.UnlockBits(bmData);

                IntPtr ip = m_Bitmap.GetHbitmap();//将Bitmap转换为BitmapSource
                BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                imgLoad.Source = bitmapSource;
            }

        }


        private void btnMedianFilter_Click(object sender, RoutedEventArgs e)
        {// 高斯平滑滤波滤波去噪
            BitmapData bmData = m_Bitmap.LockBits(new Rectangle(0, 0, m_Bitmap.Width, m_Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            for (int i = 0; i < 256; i++)
            {
                gray[i] = 0;
            }
            unsafe
            {
                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;
                byte* p = (byte*)(void*)Scan0;
                byte* pp;
                int tt;
                int nOffset = stride - m_Bitmap.Width * 3;
                int nWidth = m_Bitmap.Width;
                int nHeight = m_Bitmap.Height;
                long sum = 0;
                int[,] gaussianMatrix = { { 1, 2, 3, 2, 1 }, { 2, 4, 6, 4, 2 }, { 3, 6, 7, 6, 3 }, { 2, 4, 6, 4, 2 }, { 1, 2, 3, 2, 1 } };//高斯滤波器所选的n=5模板
                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        if (!(x <= 1 || x >= nWidth - 2 || y <= 1 || y >= nHeight - 2))
                        {
                            pp = p;
                            sum = 0;
                            int dividend = 79;
                            for (int i = -2; i <= 2; i++)
                                for (int j = -2; j <= 2; j++)
                                {
                                    pp += (j * 3 + stride * i);
                                    sum += pp[0] * gaussianMatrix[i + 2, j + 2];
                                    if (i == 0 && j == 0)
                                    {
                                        if (pp[0] > 240)//如果模板中心的灰度大于240
                                        {
                                            sum += p[0] * 30;
                                            dividend += 30;
                                        }
                                        else if (pp[0] > 230)
                                        {
                                            sum += pp[0] * 20;
                                            dividend += 20;
                                        }
                                        else if (pp[0] > 220)
                                        {
                                            sum += p[0] * 15;
                                            dividend += 15;
                                        }
                                        else if (pp[0] > 210)
                                        {
                                            sum += pp[0] * 10;
                                            dividend += 10;
                                        }
                                        else if (p[0] > 200)
                                        {
                                            sum += pp[0] * 5;
                                            dividend += 5;
                                        }
                                    }
                                    pp = p;
                                }
                            sum = sum / dividend;
                            if (sum > 255)
                            {
                                sum = 255;
                            }
                            p[0] = p[1] = p[2] = (byte)(sum);
                        }
                        tt = p[0];
                        gray[tt]++;
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            m_Bitmap.UnlockBits(bmData);
            IntPtr ip = m_Bitmap.GetHbitmap();//将Bitmap转换为BitmapSource
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            imgLoad.Source = bitmapSource;


        }


        private void btnEdgeDetection_Click(object sender, RoutedEventArgs e)
        { // sobel边缘检测

            
        }



        private void btnLocation_Click(object sender, RoutedEventArgs e)
        { //车牌定位
            

        }



        private void btnLicenceGray_Click(object sender, RoutedEventArgs e)
        {//车牌灰度化
            
        }



        private void btnLicenceBinary_Click(object sender, RoutedEventArgs e)
        { //车牌二值化
            
        }



        private void btnCharSplit_Click(object sender, RoutedEventArgs e)
        {  // 字符切割
            
        }
        private void btnCharIdentify_Click(object sender, RoutedEventArgs e)
        {   //字符识别
            
        }



    }
}
