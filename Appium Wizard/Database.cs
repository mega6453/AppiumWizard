using System;
using System.Data.SQLite;

namespace Appium_Wizard
{
    public class Database
    {
        static string databaseFilePath = FilesPath.databaseFilePath;
        static string connectionString = $"Data Source=\"{databaseFilePath}\";Version=3;";


        public static void InsertDataIntoDevicesTable(string Name, string OS, string Version, string Status, string UDID, int Width, int Height, string ConnectionType, string IPAddress)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT INTO Devices (Name, OS, Version, Status, UDID, Width, Height, Connection, IPAddress) VALUES ('" + Name + "', '" + OS + "', '" + Version + "', '" + Status + "','" + UDID + "', '" + Width + "','" + Height + "','" + ConnectionType + "','" + IPAddress + "')";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void UpdateDataInDevicesTable(string Name, string OS, string Version, string Status, string UDID, int Width, int Height, string ConnectionType, string IPAddress)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Devices SET OS = '" + OS + "', Version = '" + Version + "', Status = '" + Status + "', Width = '" + Width + "', Height = '" + Height + "', Connection = '" + ConnectionType + "', IPAddress = '" + IPAddress + "' WHERE UDID = '" + UDID + "'";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void UpdateDataInDevicesTable(string UDID, string key, string value)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Devices SET " + key + " = '" + value + "' WHERE UDID = '" + UDID + "'";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void UpdateDataIntoProfileCounterTable(int count)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE ProfileCounter SET count = @count";
                    command.Parameters.AddWithValue("@count", count);
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }

        public static void UpdateDataIntoPortNumberTable(string serverNumber, int portNumber)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE PortNumber SET ('" + serverNumber + "') = ('" + portNumber + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }


        public static void DeleteDataFromDevicesTable(string UDID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "DELETE FROM Devices WHERE UDID = @UDID";
                    command.Parameters.AddWithValue("@UDID", UDID);
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }
        public static List<Dictionary<string, string>> QueryDataFromDevicesTable()
        {
            List<Dictionary<string, string>> listOfDeviceDetails = new List<Dictionary<string, string>>();
            Dictionary<string, string> keyValuePairs;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Devices", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string os = reader.GetString(1);
                            string version = reader.GetString(2);
                            string status = reader.GetString(3);
                            string UDID = reader.GetString(4);
                            string Width = reader.GetInt32(5).ToString();
                            string Height = reader.GetInt32(6).ToString();
                            string Connection = reader.GetString(7);
                            string IPAddress = reader.GetString(8);
                            keyValuePairs = new Dictionary<string, string>();
                            keyValuePairs.Add("Name", name.Replace("''", "'"));
                            keyValuePairs.Add("OS", os);
                            keyValuePairs.Add("Version", version);
                            keyValuePairs.Add("Status", status);
                            keyValuePairs.Add("UDID", UDID);
                            keyValuePairs.Add("Width", Width);
                            keyValuePairs.Add("Height", Height);
                            keyValuePairs.Add("Connection", Connection);
                            keyValuePairs.Add("IPAddress", IPAddress);
                            listOfDeviceDetails.Add(keyValuePairs);
                        }
                    }
                }
                connection.Close();
                return listOfDeviceDetails;
            }
        }

        public static int QueryDataFromProfileCounterTable()
        {
            int count = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM ProfileCounter", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            count = reader.GetInt32(0);
                        }
                    }
                }
                connection.Close();
                return count;
            }
        }

        public static Dictionary<string, string> QueryDataFromPortNumberTable()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM PortNumber", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string port1Value = string.IsNullOrEmpty(reader["PortNumber1"]?.ToString()) ? "0" : reader["PortNumber1"].ToString();
                            string port2Value = string.IsNullOrEmpty(reader["PortNumber2"]?.ToString()) ? "0" : reader["PortNumber2"].ToString();
                            string port3Value = string.IsNullOrEmpty(reader["PortNumber3"]?.ToString()) ? "0" : reader["PortNumber3"].ToString();
                            string port4Value = string.IsNullOrEmpty(reader["PortNumber4"]?.ToString()) ? "0" : reader["PortNumber4"].ToString();
                            string port5Value = string.IsNullOrEmpty(reader["PortNumber5"]?.ToString()) ? "0" : reader["PortNumber5"].ToString();

                            result["PortNumber1"] = port1Value;
                            result["PortNumber2"] = port2Value;
                            result["PortNumber3"] = port3Value;
                            result["PortNumber4"] = port4Value;
                            result["PortNumber5"] = port5Value;
                        }
                    }
                }
                connection.Close();
                return result;
            }
        }



        public static string QueryDataFromFirstTimeRunTable()
        {
            string output = "test";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM FirstTimeRun", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            output = reader.GetString(0);
                        }
                    }
                }
                connection.Close();
                return output;
            }
        }

        public static void UpdateDataIntoFirstTimeRunTable(string YesNo)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE FirstTimeRun SET ('isFirstTimeRun') = ('" + YesNo + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }

        public static string QueryDataFromGUIDTable()
        {
            string output = string.Empty;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM GUID", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            output = reader.GetString(0);
                        }
                    }
                }
                connection.Close();
                return output;
            }
        }

        public static void UpdateDataIntoGUIDTable(string GUID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE GUID SET ('clientID') = ('" + GUID + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }


        public static Dictionary<string, string> QueryDataFromServerFinalCommandTable()
        {
            string defaultAppiumCOmmand = "appium --allow-cors";
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM ServerFinalCommand", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Server1 = reader.IsDBNull(0) ? defaultAppiumCOmmand : reader.GetString(0);
                            var Server2 = reader.IsDBNull(1) ? defaultAppiumCOmmand : reader.GetString(1);
                            var Server3 = reader.IsDBNull(2) ? defaultAppiumCOmmand : reader.GetString(2);
                            var Server4 = reader.IsDBNull(3) ? defaultAppiumCOmmand : reader.GetString(3);
                            var Server5 = reader.IsDBNull(4) ? defaultAppiumCOmmand : reader.GetString(4);

                            result["Server1"] = Server1;
                            result["Server2"] = Server2;
                            result["Server3"] = Server3;
                            result["Server4"] = Server4;
                            result["Server5"] = Server5;
                        }
                    }
                }
                connection.Close();
                return result;
            }
        }

        public static void UpdateDataIntoServerFinalCommandTable(string serverNumber, string serverCommand)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE ServerFinalCommand SET ('" + serverNumber + "') = ('" + serverCommand + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }


        public static Dictionary<string, string> QueryDataFromServerArgsTable()
        {
            string defaultAppiumCOmmand = "appium --allow-cors";
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM ServerArgsCommand", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Server1 = reader.IsDBNull(0) ? defaultAppiumCOmmand : reader.GetString(0);
                            var Server2 = reader.IsDBNull(1) ? defaultAppiumCOmmand : reader.GetString(1);
                            var Server3 = reader.IsDBNull(2) ? defaultAppiumCOmmand : reader.GetString(2);
                            var Server4 = reader.IsDBNull(3) ? defaultAppiumCOmmand : reader.GetString(3);
                            var Server5 = reader.IsDBNull(4) ? defaultAppiumCOmmand : reader.GetString(4);

                            result["Server1"] = Server1;
                            result["Server2"] = Server2;
                            result["Server3"] = Server3;
                            result["Server4"] = Server4;
                            result["Server5"] = Server5;
                        }
                    }
                }
                connection.Close();
                return result;
            }
        }

        public static void UpdateDataIntoServerArgsTable(string serverNumber, string serverCommand)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE ServerArgsCommand SET ('" + serverNumber + "') = ('" + serverCommand + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }


        public static Dictionary<string, string> QueryDataFromServerCapsTable()
        {
            string defaultAppiumCOmmand = "appium --allow-cors";
            Dictionary<string, string> result = new Dictionary<string, string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM ServerCapsCommand", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var Server1 = reader.IsDBNull(0) ? defaultAppiumCOmmand : reader.GetString(0);
                            var Server2 = reader.IsDBNull(1) ? defaultAppiumCOmmand : reader.GetString(1);
                            var Server3 = reader.IsDBNull(2) ? defaultAppiumCOmmand : reader.GetString(2);
                            var Server4 = reader.IsDBNull(3) ? defaultAppiumCOmmand : reader.GetString(3);
                            var Server5 = reader.IsDBNull(4) ? defaultAppiumCOmmand : reader.GetString(4);

                            result["Server1"] = Server1;
                            result["Server2"] = Server2;
                            result["Server3"] = Server3;
                            result["Server4"] = Server4;
                            result["Server5"] = Server5;
                        }
                    }
                }
                connection.Close();
                return result;
            }
        }

        public static void UpdateDataIntoServerCapsTable(string serverNumber, string serverCommand)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE ServerCapsCommand SET ('" + serverNumber + "') = ('" + serverCommand + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }


        public static string QueryDataFromiOSExecutorTable()
        {
            string output = string.Empty;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM iOSExecutor", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            output = reader.GetString(0);
                        }
                    }
                }
                connection.Close();
                return output;
            }
        }

        public static void UpdateDataIntoiOSExecutorTable(string executionMethod)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE iOSExecutor SET ('Executor') = ('" + executionMethod + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }


        public static void UpdateDataIntoNotificationsTable(string DeviceConnected, string DeviceDisconnected, string Screenshot, string ScreenRecording)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE Notifications SET ('DeviceConnected') = ('" + DeviceConnected + "'), ('DeviceDisconnected') = ('" + DeviceDisconnected + "'),('Screenshot') = ('" + Screenshot + "'),('ScreenRecording') = ('" + ScreenRecording + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }

        public static Dictionary<string, string> QueryDataFromNotificationsTable()
        {
            var output = new Dictionary<string, string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM Notifications", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // Assuming you want to read only the first row
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                string? columnValue = reader.IsDBNull(i) ? null : reader.GetString(i);
                                output[columnName] = columnValue;
                            }
                        }
                    }
                }
                connection.Close();
            }
            return output;
        }


        public static void UpdateDataIntoAlwaysOnTopTable(string YesOrNo)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE AlwaysOnTop SET ('SetTop') = ('" + YesOrNo + "')";
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                }
                connection.Close();
            }
        }

        public static string QueryDataFromAlwaysOnTopTable()
        {
            string output = string.Empty;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM AlwaysOnTop", connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            output = reader.GetString(0);
                        }
                    }
                }
                connection.Close();
                return output;
            }
        }
    }
}
