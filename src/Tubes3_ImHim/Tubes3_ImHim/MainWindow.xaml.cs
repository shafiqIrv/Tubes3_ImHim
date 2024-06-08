using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace Tubes3_ImHim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string mode;

        public string src_path;
        public string src_ascii;
        public string target_path;
        public string target_ascii;


        public MainWindow()
        {
            InitializeComponent();

            // Switch Button Listener
            switch_mode.Checked += switchChecked;
            switch_mode.Unchecked += switchUnchecked;

            // Initialize default mode as KMP
            mode = "bm";
            bm.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00E0FF"));
            bm.FontWeight = FontWeights.Bold;
            search_time.Text = mode;
        }

        private void chooseFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Bitmap Files|*.bmp";  // Menambahkan filter hanya untuk file .bmp

            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {

                // Menampilkan gambar bmp yang dipilih pada Image control 'finger_src'
                src_path = fileDialog.FileName;
                finger_src.Source = new BitmapImage(new Uri(src_path));
            }
        }


        private void search(object sender, RoutedEventArgs e)
        {
            // Check dulu si image nya empty ga
            if (finger_src.Source == null)
            {
                information.Text = "Please select the fingerprint image you want to search for";
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB2B2"));
                return;
            }

            information.Text = "";
            information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));


            target_path = "C:\\Users\\Shafi\\Documents\\Works\\Informatika\\Semester 4\\Stima\\Tubes 3\\With Repo\\Tubes3_ImHim\\src\\Tubes3_ImHim\\Tubes3_ImHim\\resources\\Finger\\1__M_Left_thumb_finger.BMP";

            string src = (ImageProcesser.BitmapToAscii(src_path));
            string target = (ImageProcesser.BitmapToAscii(target_path));
            finger_target.Source = new BitmapImage(new Uri(target_path));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            float result = LCS.Calculate(src, src);
            sw.Stop();

            information.Text = "SRC :" + src.Length + " TARGET: " + target.Length;

            search_time.Text = sw.ElapsedMilliseconds.ToString();
            similarity_persentage.Text = result.ToString();



        }

        private void switchChecked(object sender, RoutedEventArgs e)
        {
            // Change color BM
            bm.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            bm.FontWeight = FontWeights.Light;

            // Change color KMP
            kmp.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00E0FF"));
            kmp.FontWeight = FontWeights.Bold;

        }

        private void switchUnchecked(object sender, RoutedEventArgs e)
        {
            // Change color KMP
            kmp.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
            kmp.FontWeight = FontWeights.Light;
            
            // Change color BM
            bm.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00E0FF"));
            bm.FontWeight = FontWeights.Bold;

            Console.WriteLine(mode);
            
        }
    }
}