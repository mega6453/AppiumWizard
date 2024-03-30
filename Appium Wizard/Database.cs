using System.Data.SQLite;
using System.Net;

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
                            string port1Value = reader["PortNumber1"].ToString();
                            string port2Value = reader["PortNumber2"].ToString();
                            string port3Value = reader["PortNumber3"].ToString();
                            string port4Value = reader["PortNumber4"].ToString();
                            string port5Value = reader["PortNumber5"].ToString();

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

    }
}
