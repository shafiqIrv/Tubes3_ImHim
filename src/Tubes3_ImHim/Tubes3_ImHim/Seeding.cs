using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes3_ImHim
{
    using System;
    using System.Data.SQLite;
    using Bogus;

    public void SeedDatabaseWithFaker()
    {
        string dbPath = "Data Source=Path_to_Your_Database.db";
        using (var conn = new SQLiteConnection(dbPath))
        {
            conn.Open();

            // Menggunakan Faker untuk menghasilkan data
            var faker = new Faker("id"); // Lokalisasi Indonesia

            // Seeding untuk tabel biodata
            for (int i = 0; i < 600; i++)
            {
                var nik = faker.Random.Number(10000000, 99999999).ToString("D8");
                var nama = faker.Name.FullName();
                var tempatLahir = faker.Address.City();
                var tanggalLahir = faker.Date.Past(30, DateTime.Today.AddYears(-18)).ToString("yyyy-MM-dd");
                var jenisKelamin = faker.PickRandom(new string[] { "Laki-Laki", "Perempuan" });
                var golonganDarah = faker.PickRandom(new string[] { "A", "B", "AB", "O" });
                var alamat = faker.Address.StreetAddress();
                var agama = faker.PickRandom(new string[] { "Islam", "Kristen", "Katolik", "Hindu", "Buddha" });
                var statusPerkawinan = faker.PickRandom(new string[] { "Belum Menikah", "Menikah", "Cerai" });
                var pekerjaan = faker.Name.JobTitle();
                var kewarganegaraan = "Indonesia";

                var insertBiodata = $@"
            INSERT INTO biodata (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
            VALUES ('{nik}', '{nama}', '{tempatLahir}', '{tanggalLahir}', '{jenisKelamin}', '{golonganDarah}', '{alamat}', '{agama}', '{statusPerkawinan}', '{pekerjaan}', '{kewarganegaraan}');
            ";
                using (var cmd = new SQLiteCommand(insertBiodata, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Seeding untuk tabel sidik_jari (sederhana)
            for (int i = 0; i < 600; i++)
            {
                var nama = faker.Name.FullName();

                var insertSidikJari = $@"
            INSERT INTO sidik_jari (nama) VALUES ('{nama}');
            ";
                using (var cmd = new SQLiteCommand(insertSidikJari, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            conn.Close();
        }
    }

}
