using Bogus.DataSets;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Org.BouncyCastle.Crypto;
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
        public int match_id;
        public float total_time;
        public float match_similarity = 0;
        public string connection_string = Database.GetConnectionStringFromJson("connectionSettings.json");


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
            // Clear data di label informasi kalo ada
            clearInformation();

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Bitmap Files|*.bmp";  // Menambahkan filter hanya untuk file .bmp

            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {

                // Menampilkan gambar bmp yang dipilih pada Image control 'finger_src'
                src_path = fileDialog.FileName;
                finger_src.Source = new BitmapImage(new Uri(src_path));
                src_ascii = ImageProcesser.BitmapToAscii(src_path);
            }
        }

        private void chooseDirectory(object sender, RoutedEventArgs e)
        {
            // Clear data di label informasi 
            clearInformation();

            OpenFolderDialog folderDialog = new OpenFolderDialog();

            bool? success = folderDialog.ShowDialog();
            if (success == true)
            {

                dataset_path = folderDialog.FolderName;

                try
                {
                    // Implement Load databasenya disini sama pengecekan kosong apa ga nya
                    connection_string = Database.GetConnectionStringFromJson("connectionSettings.json");  // JANLUP GANTI JADI DI FILE EXTERNAL
                    Database.Seeding(dataset_path, connection_string);
                    information.Text = "Successfully loaded dataset from " + dataset_path;
                    information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GREEN));
                }
                catch (Exception ex)
                {
                    information.Text = "Failed to load " + dataset_path;
                    information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
                }

                // Data nya kosong
                if ((Database.GetAllIds(connection_string)).Count == 0)
                {
                    information.Text = "The dataset given is empty";
                    information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
                    return;
                }
                else
                {
                    search_btn.IsEnabled = true;
                    choose_file_btn.IsEnabled = true;
                }

 

            }
        }

        private void loadFromDB(object sender, RoutedEventArgs e)
        {
            // Clear data di label informasi 
            clearInformation();

            try
            {
                // Implement Load databasenya disini sama pengecekan kosong apa ga nya
                connection_string = Database.GetConnectionStringFromJson("connectionSettings.json"); 
                //DatabaseConverter.ConvertData(connection_string);
                information.Text = "Successfully loaded dataset";
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GREEN));
            }
            catch (Exception ex)
            {
                information.Text = "Failed to load " + dataset_path;
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
            }

            // Data nya kosong
            if ((Database.GetAllIds(connection_string)).Count == 0)
            {
                information.Text = "The dataset given is empty";
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
                return;
            }
            else
            {
                search_btn.IsEnabled = true;
                choose_file_btn.IsEnabled = true;
            }



        }


        private void search(object sender, RoutedEventArgs e)
        {
            match_similarity = 0;
            // Clear data di label informasi 
            clearInformation();

            // Cek finger print yang diinput ada ga
            if (finger_src.Source == null)
            {
                setInformation("Please select the fingerprint image you want to search for", RED);
                return;
            }

            // Searching
            string directoryPath = dataset_path;
            try
            {
                List<int> ids = Database.GetAllIds(connection_string);

                Stopwatch sw = new Stopwatch();
                sw.Start();

                // Cari dulu di awal dengan algoritma KMP/BM
                setInformation("Searching using " + mode + " algorithm", WHITE);
                setInformation((ids.Count).ToString(),   WHITE);
                foreach (int id in ids)
                {
                    string ascii = Database.GetColumnValueById(connection_string, "ascii" ,id);

                    bool found;

                    if (mode == "kmp")
                    {
                        found = KMP.Search(src_ascii, ascii);
                    }
                    else
                    {
                        found = BM.Search(src_ascii, ascii);
                    }

                    if (found)
                    {
                        sw.Stop();
                        total_time = sw.ElapsedMilliseconds;
                        match_id = id;
                        match_similarity = 100;

                        showResult();
                        return;
                    }
                }

                // Gaketemu, lanjut iterasi ulang pake Levenshtein
                int id_tercocok = 0;
                float persentase_id_tercocok = 0;
                foreach (int id in ids)
                {
                    string ascii = Database.GetColumnValueById(connection_string, "ascii", id);
                    float persentase = LevenshteinDistance.Calculate(src_ascii, ascii);

                    if (persentase > persentase_id_tercocok || persentase > 55)
                    {
                        persentase_id_tercocok = persentase;
                        id_tercocok = id;
                    }

                }

                sw.Stop();
                total_time = sw.ElapsedMilliseconds;
                match_id = id_tercocok;
                match_similarity = persentase_id_tercocok;
                showResult();

            }
            catch (Exception ex)
            {
                information.Text = ex.Message;
                information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(RED));
            }
        }


        private void showResult()
        {
            // Dah ketemu, nyari pake regex
            List<string> names = Database.GetAllColumnValues(connection_string, "biodata", "nama");
            setInformation("SHOW RES", GREEN);
            foreach (string name in names)
            {
                //setInformation(name, GREEN);
                if (RegexClass.IsMatch(Database.GetColumnValueById(connection_string, "nama", match_id), name))
                {
                    setInformation("MASUK REGEX", GREEN);
                    Biodata bio_match = Database.GetBiodataByName( name, connection_string);

                    // Show hasil match nya
                    string match_path = Database.GetColumnValueById(connection_string, "berkas_citra", match_id);
                    finger_target.Source = new BitmapImage(new Uri(match_path));
                    similarity_persentage.Text = match_similarity.ToString() + " %";
                    search_time.Text = total_time.ToString() + " ms";
                    setInformation("Found match at " + Path.GetFileName(match_path), GREEN);

                    //    //Set Biodata
                    db_nik.Text = bio_match.NIK;
                    db_nama.Text = Database.GetColumnValueById(connection_string, "nama", match_id);
                    db_birthplace.Text = bio_match.TempatLahir;
                    db_birthdate.Text = (bio_match.TanggalLahir).ToString("dd-MM-yyyy");
                    db_gender.Text = bio_match.JenisKelamin;
                    db_bloodtype.Text = bio_match.GolonganDarah;
                    db_address.Text = bio_match.Alamat;
                    db_religion.Text = bio_match.Agama;
                    db_marriage.Text = bio_match.StatusPerkawinan;
                    db_job.Text = bio_match.Pekerjaan;
                    db_nationallity.Text = bio_match.Kewarganegaraan;
                }
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

        private void clearInformation()
        {
            // Clear data sebelumnya kalo ada
            search_time.Text = "";
            similarity_persentage.Text = "";
            finger_target.Source = null;
            information.Text = "";
            information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(WHITE));
        }
        
        private void setInformation(string msg, string color)
        {
            information.Text = msg;
            information.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
    
    }
}