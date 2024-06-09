using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Bogus;

namespace Tubes3_ImHim
{
    public class Database
    {
        // static void Main()
        // {
        //     string folderPath = "path_to_your_folder";
        //     string connectionString = GetConnectionStringFromJson("connectionSettings.json");
        //     Seeding(folderPath, connectionString);
        // }

        public static string GetConnectionStringFromJson(string jsonFilePath)
        {
            // Read JSON content from file
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Deserialize JSON into Dictionary<string, string>
            Dictionary<string, string> settings = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            // Build connection string
            string connectionString = $"Server={settings["Server"]};User={settings["User"]};Password={settings["Password"]};Database={settings["Database"]}";

            return connectionString;
        }

        public static Biodata GetBiodataByName(string nama, string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM biodata WHERE nama = @nama";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nama", nama);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Biodata
                    {
                        NIK = reader["NIK"].ToString(),
                        Nama = reader["nama"].ToString(),
                        TempatLahir = reader["tempat_lahir"].ToString(),
                        TanggalLahir = DateTime.Parse(reader["tanggal_lahir"].ToString()),
                        JenisKelamin = reader["jenis_kelamin"].ToString(),
                        GolonganDarah = reader["golongan_darah"].ToString(),
                        Alamat = reader["alamat"].ToString(),
                        Agama = reader["agama"].ToString(),
                        StatusPerkawinan = reader["status_perkawinan"].ToString(),
                        Pekerjaan = reader["pekerjaan"].ToString(),
                        Kewarganegaraan = reader["kewarganegaraan"].ToString()
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public static string GetColumnValueById(string connectionString, string columnName, int idValue)
        {
            string value = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT {columnName} FROM sidik_jari WHERE ID = @idValue;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idValue", idValue);
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            value = result.ToString();
                        }
                    }

                    if (value != null)
                    {
                        Console.WriteLine($"Retrieved value '{value}' from column '{columnName}' for primary key '{idValue}'.");
                    }
                    else
                    {
                        Console.WriteLine($"No value found for primary key '{idValue}'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return value;
        }

        public static List<string> GetAllColumnValues(string connectionString, string tableName, string columnName)
        {
            List<string> values = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT {columnName} FROM {tableName};";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string value = reader.GetString(columnName);
                                values.Add(value);
                            }
                        }
                    }

                    Console.WriteLine($"Retrieved {values.Count} values from column '{columnName}' in table '{tableName}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return values;
        }

        public static List<int> GetAllIds(string connectionString)
        {
            List<int> ids = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT ID FROM sidik_jari;";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ids.Add(reader.GetInt32("ID"));
                            }
                        }
                    }

                    Console.WriteLine($"Retrieved {ids.Count} ids from table sidik_jari.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return ids;
        }

        public static void Seeding(string folderPath, string connectionString)
        {
            System.Diagnostics.Debug.WriteLine("Seeding Started");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected to MariaDB database.");

                    if (!Directory.Exists(folderPath))
                    {
                       Console.WriteLine("The specified folder does not exist.");
                       return;
                    }

                    RemoveAllData(connectionString);
                    AddFirstColumn(connectionString, "sidik_jari", "ID");
                    AddColumn(connectionString, "sidik_jari", "ascii", "text");

                    IEnumerable<string> bmFiles = Directory.EnumerateFiles(folderPath, "*.BMP");

                    // Set Seed supaya ga random terus tiap generate ulang
                    Randomizer.Seed = new Random(7777);

                    var faker = new Faker();
                    foreach (string bmFile in bmFiles)
                    {
                        using (MySqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {


                                string ascii = ImageProcesser.BitmapToAscii(bmFile); // Ensure this method exists
                                string path = bmFile;
                                string name = GenerateRandomName(faker);
                                string alayName = GenerateAlayName(name);
                                string placeOfBirth = GeneratePlaceOfBirth(faker);
                                string dateOfBirthString = GenerateDateOfBirth(faker);
                                string gender = GenerateGender(faker);
                                string bloodType = GenerateBloodType(faker);
                                string address = GenerateAddress(faker);
                                string religion = GenerateReligion(faker);
                                string status = GenerateStatus(faker);
                                string job = GenerateJob(faker);
                                string citizenship = GenerateCitizenship(faker);

                                string nik = GenerateRandomNIK(dateOfBirthString, faker);
                                bool isnikUnique = false;

                                while (!isnikUnique)
                                {
                                    string query = "SELECT COUNT(*) FROM biodata WHERE NIK = @nik;";
                                    using (MySqlCommand command = new MySqlCommand(query, connection))
                                    {
                                        command.Parameters.AddWithValue("@nik", nik);
                                        int count = Convert.ToInt32(command.ExecuteScalar());
                                        if (count == 0)
                                        {
                                            isnikUnique = true;
                                        }
                                        else
                                        {
                                            nik = GenerateRandomNIK(dateOfBirthString, faker);
                                        }
                                    }
                                }

                                // Insert into biodata table
                                string biodataQuery = @"
                                        INSERT INTO biodata 
                                        (NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan)
                                        VALUES 
                                        (@NIK, @nama, @tempat_lahir, @tanggal_lahir, @jenis_kelamin, @golongan_darah, @alamat, @agama, @status_perkawinan, @pekerjaan, @kewarganegaraan)";

                                using (MySqlCommand cmd = new MySqlCommand(biodataQuery, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@NIK", nik);
                                    cmd.Parameters.AddWithValue("@nama", alayName);
                                    cmd.Parameters.AddWithValue("@tempat_lahir", placeOfBirth);
                                    cmd.Parameters.AddWithValue("@tanggal_lahir", dateOfBirthString);
                                    cmd.Parameters.AddWithValue("@jenis_kelamin", gender);
                                    cmd.Parameters.AddWithValue("@golongan_darah", bloodType);
                                    cmd.Parameters.AddWithValue("@alamat", address);
                                    cmd.Parameters.AddWithValue("@agama", religion);
                                    cmd.Parameters.AddWithValue("@status_perkawinan", status);
                                    cmd.Parameters.AddWithValue("@pekerjaan", job);
                                    cmd.Parameters.AddWithValue("@kewarganegaraan", citizenship);

                                    cmd.ExecuteNonQuery();
                                }

                                // Insert into sidik_jari table
                                string sidikJariQuery = "INSERT INTO sidik_jari (berkas_citra, nama, ascii) VALUES (@berkas_citra, @nama, @ascii)";

                                using (MySqlCommand cmd = new MySqlCommand(sidikJariQuery, connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@berkas_citra", path);
                                    cmd.Parameters.AddWithValue("@nama", name);
                                    cmd.Parameters.AddWithValue("@ascii", ascii);

                                    cmd.ExecuteNonQuery();
                                }

                                // Commit transaction
                                transaction.Commit();
                                Console.WriteLine("All records were written to the database.");
                            }
                            catch (Exception ex)
                            {
                                // Rollback transaction if any error occurs
                                transaction.Rollback();
                                Console.WriteLine("Error: " + ex.Message);
                            }
                        }
                    }
                    
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        public static void RemoveAllData(string connectionString)
        {
            List<string> tableNames = new List<string>();
            string getTablesQuery = "SHOW TABLES;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (MySqlCommand getTablesCommand = new MySqlCommand(getTablesQuery, connection))
                    using (MySqlDataReader reader = getTablesCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }

                    foreach (string tableName in tableNames)
                    {
                        string truncateQuery = $"TRUNCATE TABLE {tableName};";
                        using (MySqlCommand truncateCommand = new MySqlCommand(truncateQuery, connection))
                        {
                            truncateCommand.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("All data removed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public static void AddFirstColumn(string connectionString, string tableName, string newPrimaryKeyColumn)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the column exists
                    string checkColumnQuery = $@"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                    AND TABLE_NAME = '{tableName}'
                    AND COLUMN_NAME = '{newPrimaryKeyColumn}';";

                    bool columnExists;
                    using (MySqlCommand checkColumnCommand = new MySqlCommand(checkColumnQuery, connection))
                    {
                        columnExists = Convert.ToInt32(checkColumnCommand.ExecuteScalar()) > 0;
                    }

                    if (!columnExists)
                    {
                        // Add the new column as the first column
                        string addColumnQuery = $@"
                        ALTER TABLE {tableName}
                        ADD COLUMN {newPrimaryKeyColumn} INT NOT NULL AUTO_INCREMENT PRIMARY KEY FIRST;";
                        using (MySqlCommand addColumnCommand = new MySqlCommand(addColumnQuery, connection))
                        {
                            addColumnCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Column '{newPrimaryKeyColumn}' added as the first column and set as primary key successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Column '{newPrimaryKeyColumn}' already exists.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public static void AddColumn(string connectionString, string tableName, string columnName, string columnType)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the column exists
                    string checkColumnQuery = $@"
                    SELECT COUNT(*)
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                    AND TABLE_NAME = '{tableName}'
                    AND COLUMN_NAME = '{columnName}';";

                    bool columnExists;
                    using (MySqlCommand checkColumnCommand = new MySqlCommand(checkColumnQuery, connection))
                    {
                        columnExists = Convert.ToInt32(checkColumnCommand.ExecuteScalar()) > 0;
                    }

                    if (!columnExists)
                    {
                        // Add the new column
                        string addColumnQuery = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType};";
                        using (MySqlCommand addColumnCommand = new MySqlCommand(addColumnQuery, connection))
                        {
                            addColumnCommand.ExecuteNonQuery();
                        }

                        Console.WriteLine($"Column '{columnName}' added successfully to table '{tableName}'.");
                    }
                    else
                    {
                        Console.WriteLine($"Column '{columnName}' already exists in table '{tableName}'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        static string GenerateRandomName(Faker faker)
        {
            return faker.Name.FullName();
        }

        static string GenerateAlayName(string name)
        {
            string resultstring = name;

            Random random = new Random(); 

            // for each character in result random remove vowel
            for (int i = 0; i < resultstring.Length; i++)
            {
                int randomNumber = random.Next(0, 2);
                if (randomNumber == 1 && "aeiouAEIOU".Contains(resultstring[i]))
                {
                    resultstring = resultstring.Remove(i, 1);
                }
            }

            char[] result = resultstring.ToLower().ToCharArray();

            // for each character in result random uppercase
            for (int i = 0; i < result.Length; i++)
            {
                int randomNumber = random.Next(0, 2);
                if (result[i] != ' ')
                {
                    if (randomNumber == 1)
                    {
                        result[i] = char.ToUpper(result[i]);
                    }
                }
            }

            // for each character in result random change to alay
            for (int i = 0; i < result.Length; i++)
            {
                int randomNumber = random.Next(0, 2);
                if (result[i] != ' ' && randomNumber == 1)
                {
                    switch (result[i])
                    {
                        case 'a':
                        case 'A':
                            result[i] = '4';
                            break;
                        case 'b':
                        case 'B':
                            result[i] = '8';
                            break;
                        case 'e':
                        case 'E':
                            result[i] = '3';
                            break;
                        case 'g':
                        case 'G':
                            result[i] = '6';
                            break;
                        case 'i':
                        case 'I':
                            result[i] = '1';
                            break;
                        case 'o':
                        case 'O':
                            result[i] = '0';
                            break;
                        case 's':
                        case 'S':
                            result[i] = '5';
                            break;
                        case 't':
                        case 'T':
                            result[i] = '7';
                            break;
                        case 'z':
                        case 'Z':
                            result[i] = '2';
                            break;
                    }
                }
            }

            return new string(result);
        }

        static string GeneratePlaceOfBirth(Faker faker)
        {
            return faker.Address.City();
        }

        static string GenerateDateOfBirth(Faker faker)
        { 
            return faker.Date.Past(30,DateTime.Now.AddYears(-10)).ToString("yyyy-MM-dd");
        }

        static string GenerateRandomNIK(string dateOfBirth, Faker faker)
        {
            string randomNumber1 = faker.Random.Number(1, 9).ToString() + faker.Random.Number(10000, 99999).ToString();

            string formattedDateOfBirth = dateOfBirth.Substring(8, 2) + dateOfBirth.Substring(5, 2) + dateOfBirth.Substring(2, 2);

            string randomNumber2 = faker.Random.Number(0000, 9999).ToString();

            string randomNIK = randomNumber1 + formattedDateOfBirth + randomNumber2;

            return randomNIK;
        }

        static string GenerateGender(Faker faker)
        {
            return faker.Random.Bool() ? "Laki-Laki" : "Perempuan";
        }

        static string GenerateBloodType(Faker faker)
        {
            string[] bloodTypes = { "A", "B", "AB", "O" };
            return bloodTypes[faker.Random.Number(0, 3)];
        }

        static string GenerateAddress(Faker faker)
        {
            return faker.Address.FullAddress();
        }

        static string GenerateReligion(Faker faker)
        {
            string[] religions = { "Islam", "Kristen", "Katolik", "Hindu", "Buddha", "Konghucu" };
            return religions[faker.Random.Number(0, 5)];
        }

        static string GenerateStatus(Faker faker)
        {
            string[] Status = { "Belum Menikah", "Menikah", "Cerai" };
            return Status[faker.Random.Number(0, 2)];
        }

        static string GenerateJob(Faker faker)
        {
            return faker.Name.JobTitle();
        }

        static string GenerateCitizenship(Faker faker)
        {
            return faker.Address.Country();
        }
    }
}