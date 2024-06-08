using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data.SQLite;


namespace Tubes3_ImHim
{
    public class Biodata
    {
        public string NIK { get; set; }
        public string Nama { get; set; }
        public string TempatLahir { get; set; }
        public DateTime? TanggalLahir { get; set; }
        public string JenisKelamin { get; set; }
        public string GolonganDarah { get; set; }
        public string Alamat { get; set; }
        public string Agama { get; set; }
        public string StatusPerkawinan { get; set; }
        public string Pekerjaan { get; set; }
        public string Kewarganegaraan { get; set; }

        public static Biodata GetAttributesByName(string name)
        {
            string dbPath = "bio.db"; // Adjust the path to your SQLite database file
            Biodata biodata = null;

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                // SQL query to select all attributes for a specific name
                string query = "SELECT * FROM biodata WHERE nama = @name";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            biodata = new Biodata
                            {
                                NIK = reader["NIK"].ToString(),
                                Nama = reader["nama"].ToString(),
                                TempatLahir = reader["tempat_lahir"].ToString(),
                                TanggalLahir = reader.IsDBNull(reader.GetOrdinal("tanggal_lahir")) ? (DateTime?)null : DateTime.Parse(reader["tanggal_lahir"].ToString()),
                                JenisKelamin = reader["jenis_kelamin"].ToString(),
                                GolonganDarah = reader["golongan_darah"].ToString(),
                                Alamat = reader["alamat"].ToString(),
                                Agama = reader["agama"].ToString(),
                                StatusPerkawinan = reader["status_perkawinan"].ToString(),
                                Pekerjaan = reader["pekerjaan"].ToString(),
                                Kewarganegaraan = reader["kewarganegaraan"].ToString(),
                            };
                        }
                    }
                }

                conn.Close();
            }

            return biodata;
        }

        public static List<string> GetAllNames()
        {
            string dbPath = "Data Source=Path_to_Your_Database.db"; // Adjust the path to your SQLite database file
            List<string> names = new List<string>();

            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();

                // SQL query to select all names from the biodata table
                string query = "SELECT nama FROM biodata";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["nama"].ToString();
                            names.Add(name);
                        }
                    }
                }

                conn.Close();
            }

            return names;
        }

    }


}
