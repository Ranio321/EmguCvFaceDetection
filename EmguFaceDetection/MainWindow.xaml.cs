using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using Emgu;
using Pen = System.Drawing.Pen;
using Color = System.Drawing.Color;
using Emgu.CV;
using Emgu.CV.Structure;
using Rectangle = System.Drawing.Rectangle;
using System.Threading;

namespace EmguFaceDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnInitialize;
            Closing += CloseEvent;
        }

        FilterInfoCollection filter;
        VideoCaptureDevice device;


        private void OnInitialize(object sendet, RoutedEventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
            {
                Camera.Items.Add(device.Name);

            }
            Camera.SelectedIndex = 0;
            device = new VideoCaptureDevice();
            
        }

        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            device = new VideoCaptureDevice(filter[Camera.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
            
        }


        //static CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");
        //static CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        static CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt.xml");
        static CascadeClassifier EyeDetection = new CascadeClassifier("haarcascade_eye.xml");
        //static CascadeClassifier EyeDetection = new CascadeClassifier("haarcascade_lefteye_2splits.xml");



        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
                         
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Gray, byte> grayImage = new Image<Gray, byte>(bitmap);
           
            var TempImage = new Image<Bgr, byte>(bitmap);
           // var TestImage = TempImage.InRange(new Bgr(200, 200, 200),
               // new Bgr(250, 250, 250));
            Rectangle[] Faces = cascadeClassifier.DetectMultiScale(grayImage, 1.1, 4);
            int i = 1;
            foreach(var s in Faces)
            {

                TempImage.Draw(s, new Bgr(0, 0, 255), 2);
                
                grayImage.ROI = new Rectangle(s.X,s.Y,s.Width,s.Height/2);
              //  TestImage.ROI=new Rectangle(s.X, s.Y, s.Width, s.Height/ 2);
                //CircleF[] circles = TestImage.HoughCircles(new Gray(4), new Gray(200),2,TestImage.Height,10,400)[0];
                Rectangle[] Eyes = EyeDetection.DetectMultiScale(grayImage, 1.1, 4);

                //foreach(var c in circles)
                //{
                //    TempImage.Draw(c, new Bgr(0, 255, 0), 2);
                  
                //}


                foreach (var k in Eyes)
                {
                    var d = k;
                    
                    d.Y += s.Y;
                    d.X += s.X;

                    TempImage.Draw(d, new Bgr(255, 0, 0), 2);
                }
               // grayImage.ROI = Rectangle.Empty;
            }
            DispatchIfNecessary(() =>
            {
                 MyImage.Source = TempImage.Bitmap.BitmapToImageSource();
                //MyImage.Source = bitmap.BitmapToImageSource();

            }
            );
            }














        //private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        //{
        //    Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

        //    // Bitmap bitmap = new Bitmap("C:/Users/grzeg/Downloads/Michu.jpg");
        //    Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
        //    System.Drawing.Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 2, 1);
        //    foreach (var r in rectangles)
        //    {
        //        using (Graphics graphics = Graphics.FromImage(bitmap))
        //        {
        //            using (Pen pen = new Pen(Color.Red, 1))
        //            {
        //                graphics.DrawRectangle(pen, r);
        //            }
        //        }

        //    }

        //        DispatchIfNecessary(() =>
        //        {
        //            MyImage.Source = bitmap.BitmapToImageSource();
        //        }
        //        );



        //}

        private void CloseEvent(object sender, CancelEventArgs e)
        {
            Thread.Sleep(10);
            
            if (device.IsRunning)
            {
                device.NewFrame -= Device_NewFrame;
                device.SignalToStop();
             
               
                device = null;
              
                
            }
            System.Windows.Application.Current.Shutdown();
        }

        public void DispatchIfNecessary(Action action)
        {
            
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(action);
            else
                action.Invoke();
        }

    }

}
