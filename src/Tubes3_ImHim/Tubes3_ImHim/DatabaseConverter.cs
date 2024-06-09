using System;
using System.IO;
using System.Reflection.Metadata;
using MySql.Data.MySqlClient;

namespace Tubes3_ImHim
{
    public class DatabaseConverter
    {
        public static void ConvertData(string connectionString)
        {
            string parentDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"../../../../../../"));
            string imagefolder = parentDirectory + "test\\";

            Database.AddFirstColumn(connectionString, "sidik_jari", "ID");
            Database.AddColumn(connectionString, "sidik_jari", "ascii", "text");
            UpdatePathColumn(connectionString, "sidik_jari", "berkas_citra", imagefolder);
            UpdateColumnBasedOnAnother(connectionString, "sidik_jari", "ascii", "ID");
        }

        public static string GetFileNameFromPath(string path)
        {
            string[] parts = path.Split('/');
            string filename = parts[parts.Length - 1];
            return filename;
        }

        public static void UpdatePathColumn(string connectionString, string tableName, string columnToUpdate, string pathfolder)
        {
            List<string> oldValues = new List<string>();

            // Use a temporary connection to fetch data
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = $"SELECT {columnToUpdate} FROM {tableName};";

                using (MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection))
                using (MySqlDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        oldValues.Add(reader.GetString(0));
                    }
                }
            }

            // Use another connection for updates
            using (MySqlConnection updateConnection = new MySqlConnection(connectionString))
            {
                updateConnection.Open();
                MySqlTransaction transaction = updateConnection.BeginTransaction();

                try
                {
                    foreach (string oldValue in oldValues)
                    {
                        string newValue = pathfolder + Path.GetFileName(oldValue);

                        string updateQuery = $"UPDATE {tableName} SET {columnToUpdate} = @newValue WHERE {columnToUpdate} = @oldValue;";
                        using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, updateConnection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@newValue", newValue);
                            updateCommand.Parameters.AddWithValue("@oldValue", oldValue);
                            updateCommand.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    Console.WriteLine($"Values updated successfully for column '{columnToUpdate}' in table '{tableName}'.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }

        public static void UpdateColumnBasedOnAnother(string connectionString, string tableName, string updateColumn, string referenceColumn)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Begin transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string selectQuery = $"SELECT {referenceColumn}, {updateColumn} FROM {tableName};";
                        List<(int referenceValue, string newValue)> updates = new List<(int referenceValue, string newValue)>();

                        // Collect data for updates
                        using (var selectCommand = new MySqlCommand(selectQuery, connection, transaction))
                        using (var reader = selectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int referenceValue = reader.GetInt32(0);
                                string newValue = ImageProcesser.BitmapToAscii(Database.GetColumnValueById(Database.GetConnectionStringFromJson("connectionSettings.json"), "berkas_citra", referenceValue));

                                updates.Add((referenceValue, newValue));
                            }
                        }

                        // Execute updates
                        foreach (var (referenceValue, newValue) in updates)
                        {
                            string updateColumnQuery = $"UPDATE {tableName} SET {updateColumn} = @newValue WHERE {referenceColumn} = @referenceValue;";
                            using (var updateCommand = new MySqlCommand(updateColumnQuery, connection, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@newValue", newValue);
                                updateCommand.Parameters.AddWithValue("@referenceValue", referenceValue);
                                updateCommand.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        Console.WriteLine($"Values updated successfully for column '{updateColumn}' in table '{tableName}'.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
            }
        }

    }
}