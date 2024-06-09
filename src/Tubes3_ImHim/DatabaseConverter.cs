using System;
using Mysql.Data.MySqlClient;

namespace Tubes3_ImHim
{
    public class DatabaseConverter
    {
        public static void Convert(string connectionString)
        {
            string parentDirectory = Path.GetFullPath(Path.Combine(currentDirectory, @"../../../../../"));
            string imagefolder = parentDirectory + "test\";

            Database.AddFirstColumn(connectionString, "sidik_jari", "ID");
            Database.AddColumn(connectionString, "sidik_jari", "ascii", "text");
            UpdatePathColumn(connectionString, "sidik_jari", "berkas_citra", imagefolder);
            UpdateColumnBasedOnAnother(connectionString, "sidik_jari", "ascii", "berkas_citra");
        }

        public static string GetFileNameFromPath(string path)
        {
            string[] parts = path.Split('/');
            string filename = parts[parts.Length - 1];
            return filename;
        }

        public static void UpdatePathColumn(string connectionString, string tableName, string columnToUpdate, string pathfolder)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Retrieve all rows from the table
                    string selectQuery = $"SELECT * FROM {tableName};";
                    using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                    using (MySqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Get the old value of the column
                            string oldValue = reader.GetString(reader.GetOrdinal(columnToUpdate));

                            // Apply transformation to get the new value
                            string newValue = pathfolder + GetFileNameFromPath(oldValue);

                            // Update the value of the column for this row
                            string updateQuery = $"UPDATE {tableName} SET {columnToUpdate} = @newValue WHERE {columnToUpdate} = @oldValue;";
                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@newValue", newValue);
                                updateCommand.Parameters.AddWithValue("@oldValue", oldValue);
                                updateCommand.ExecuteNonQuery();
                            }
                        }
                    }

                    Console.WriteLine($"Values updated successfully for column '{columnToUpdate}' in table '{tableName}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public static void UpdateColumnBasedOnAnother(string connectionString, string tableName, string updateColumn, string referenceColumn)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Check if the columns exist
                    string checkColumnQuery = $@"
                        SELECT COUNT(*)
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_SCHEMA = DATABASE()
                        AND TABLE_NAME = '{tableName}'
                        AND COLUMN_NAME IN ('{updateColumn}', '{referenceColumn}');";

                    bool columnsExist;
                    using (MySqlCommand checkColumnCommand = new MySqlCommand(checkColumnQuery, connection))
                    {
                        columnsExist = Convert.ToInt32(checkColumnCommand.ExecuteScalar()) == 2;
                    }

                    if (columnsExist)
                    {
                        string selectQuery = $"SELECT {referenceColumn}, {updateColumn} FROM {tableName};";
                        using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                        using (MySqlDataReader reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string referenceValue = reader.GetString(0);
                                string currentValue = reader.GetString(1);

                                string newValue = ImageProcesser.BitmapToAscii(referenceValue);

                                // Update the value of the update column
                                string updateColumnQuery = $"UPDATE {tableName} SET {updateColumn} = @newValue WHERE {referenceColumn} = @referenceValue;";
                                using (MySqlCommand updateCommand = new MySqlCommand(updateColumnQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@newValue", newValue);
                                    updateCommand.Parameters.AddWithValue("@referenceValue", referenceValue);
                                    updateCommand.ExecuteNonQuery();
                                }

                                Console.WriteLine($"Value updated successfully for column '{updateColumn}' in table '{tableName}'.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"One or both specified columns do not exist in table '{tableName}'.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }
    }
}