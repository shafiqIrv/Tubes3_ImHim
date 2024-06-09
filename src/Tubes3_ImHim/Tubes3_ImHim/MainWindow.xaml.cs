using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
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
        private const string RED = "#FFB2B2";
        private const string GREEN = "#B2FFC7";
        private const string WHITE = "#FFFFFF";
        private const string CYAN = "#00E0FF";

        public string mode;
        public string dataset_path;
        public string src_path;
        public string src_ascii;
        public string target_path;
        public string target_ascii;
        public string matchable_path;
        public string matchable_filename;
        public float matchable_similarity = 0;


        public MainWindow()
        {
            InitializeComponent();

            // Switch Button Listener
            switch_mode.Checked += switchChecked;
            switch_mode.Unchecked += switchUnchecked;

            // Initialize button to be disabled before entering dataset

            // Initialize default mode as KMP
            mode = "bm";
            bm.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(CYAN));
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

        private void chooseDirectory(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog();

            bool? success = folderDialog.ShowDialog();
            if (success == true)
            {

                dataset_path = folderDialog.FolderName;


                // Implement Load databasenya disini sama pengecekan kosong apa ga nya

                // Enable button lainnya
                information.Text = "Successfully loaded dataset from " + dataset_path;
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GREEN  ));

                search_btn.IsEnabled = true;
                choose_file_btn.IsEnabled = true;
            }
        }

        private void search(object sender, RoutedEventArgs e)
        {
            // Clear data sebelumnya kalo ada
            search_time.Text = "";
            similarity_persentage.Text = "";
            finger_target.Source = null;
            information.Text = "";
            information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(WHITE));

            // Check dulu si image nya empty ga
            if (finger_src.Source == null)
            {
                information.Text = "Please select the fingerprint image you want to search for";
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
                return;
            }

            // Searching
            string directoryPath = dataset_path;
            try
            {


                IEnumerable<string> files = Directory.EnumerateFiles(directoryPath);

                Stopwatch sw = new Stopwatch();
                sw.Start();
                // Cari dulu di awal dengan algoritma KMP/BM


                // Iterasi melalui semua file dengan Levenshtein distance
                foreach (string file in files)
                {
                    target_path = file;
                    string target_ascii = ImageProcesser.BitmapToAscii(target_path);
                    string src_ascii = ImageProcesser.BitmapToAscii(src_path);

                    float persentage = LevenshteinDistance.Calculate(target_ascii, src_ascii);

                    if (persentage > matchable_similarity)
                    {
                        matchable_similarity = persentage;
                        matchable_path = target_path;
                        //matchable_filename = Path.GetFileName(src_path);
                    }

                }

                sw.Stop();

                information.Text = matchable_path;
                search_time.Text = sw.ElapsedMilliseconds + " ms";

                // Output
                finger_target.Source = new BitmapImage(new Uri(matchable_path));
                information.Text = "Found most similar at " + Path.GetFileName(matchable_path);
                search_time.Text = sw.ElapsedMilliseconds + " ms";
                similarity_persentage.Text = matchable_similarity.ToString() + " %";

            }
            catch (Exception ex)
            {
                information.Text = "Invalid Dataset Directory";
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
            }
        }

        private void switchChecked(object sender, RoutedEventArgs e)
        {
            // Change color BM
            bm.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(WHITE));
            bm.FontWeight = FontWeights.Light;

            // Change color KMP
            kmp.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(CYAN));
            kmp.FontWeight = FontWeights.Bold;

        }

        private void switchUnchecked(object sender, RoutedEventArgs e)
        {
            // Change color KMP
            kmp.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(WHITE));
            kmp.FontWeight = FontWeights.Light;
            
            // Change color BM
            bm.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(CYAN));
            bm.FontWeight = FontWeights.Bold;

            Console.WriteLine(mode);
            
        }
    }
}