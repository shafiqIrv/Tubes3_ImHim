using Microsoft.Win32;
using System.Windows;
using System.Windows.Media.Imaging;


namespace Tubes3_ImHim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void chooseFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Bitmap Files|*.bmp";  // Menambahkan filter hanya untuk file .bmp

            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                // Menampilkan gambar bmp yang dipilih pada Image control 'finger_src'
                //finger_src.Source = new BitmapImage(new Uri(fileDialog.FileName));
                //string test = ImageProcesser.BitmapToAscii(fileDialog.FileName);
                //nama.Text = (BM.Search(ImageProcesser.get30MiddleCharacter(test),test)).ToString();
            }
        }


        private void search(object sender, RoutedEventArgs e)
        {
               
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}