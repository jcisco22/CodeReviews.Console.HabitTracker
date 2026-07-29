
using Microsoft.Data.Sqlite;
using System.Globalization;
namespace habit_tracker
{

    class Programm
    {
        static string connectionstring = @"Data Source=habit-Tracker.db";

        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection(connectionstring))

            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                 Id INTEGER PRIMARY KEY AUTOINCREMENT,
                 Date TEXT,
                 Quantity INTEGER
                 )";

                tableCmd.ExecuteNonQuery();
                connection.Close();


            }
            GetUserInput();
        }
        static void GetUserInput()
        {
            bool end = false;

            while (end == false)
            {
                Console.WriteLine("Main Menu\n\n");
                Console.WriteLine("What would you like to do?");
                Console.WriteLine("Enter 0 to close");
                Console.WriteLine("Enter 1 to view record");
                Console.WriteLine("Enter 2 to insert record");
                Console.WriteLine("Enter 3 to delete record");
                Console.WriteLine("Enter 4 to update record");

                string command = Console.ReadLine();

                switch (command)
                {
                    case "0":
                        Console.WriteLine("Press any key to exit.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;

                    case "1":
                        GetAllRecords();
                        break;

                    case "2":
                        Insert();
                        break;

                    case "3":
                        Delete();
                        break;

                    case "4":
                        Update();
                        break;

                    default:
                        Console.WriteLine("Invalid command. Please type a number from 0 to 4.");
                        break;
                }
            }
        }


        private static void GetAllRecords()
        {
            Console.Clear();
            using (var connection = new SqliteConnection(connectionstring))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                $" SELECT * FROM drinking_water ";

                List<Drinkingwater> tableData = new();

                SqliteDataReader sqliteDataReader = tableCmd.ExecuteReader();
                if (sqliteDataReader.HasRows)
                {
                    while (sqliteDataReader.Read())
                    {
                        tableData.Add(
                          new Drinkingwater
                          {
                              Id = sqliteDataReader.GetInt32(0),
                              date = DateTime.ParseExact(
                              sqliteDataReader.GetString(1), new[] { "dd-MM-yy", "dd-MM-yyyy" },
                            new CultureInfo("en-us"), DateTimeStyles.None),
                              Quantity = sqliteDataReader.GetInt32(2)
                              // each added value needs to be converted into a type the databse can read
                              //id and quantity are integers, dattime is date
                          });
                    }

                }
                else
                {
                    Console.WriteLine("No rows found");
                }
                connection.Close();

                Console.WriteLine();

                foreach (var item in tableData)
                {
                    Console.WriteLine($"{item.Id} -- Quantity: {item.Quantity} -- DATE:{item.date.ToString("dd-MM-yyyy")}");
                }

            }

        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if (numberInput == "0") GetUserInput();

            int.TryParse(numberInput, out int finalInput);

            while (!Int32.TryParse(numberInput, out _) || finalInput < 0)
            {
                Console.WriteLine("\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main menu or try again:\n\n");
                numberInput = Console.ReadLine();
            }

            return finalInput;

        }

        internal static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to main menu\n");
            string DateInput = Console.ReadLine();


            if (DateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(DateInput, new[] { "dd-MM-yy", "dd-MM-yyyy" }, new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Console.WriteLine("Not all paths return a value\n\n");
                DateInput = Console.ReadLine();
            }

            return DateInput;
        }


        private static void Insert()
        {
            Console.Clear();
            string date = GetDateInput();
            int quantity = GetNumberInput("Please enter the number of glasses drank.");

            using (var connection = new SqliteConnection(connectionstring))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                $"INSERT INTO drinking_water(date, quantity) VALUES(@date, @quantity)";
                tableCmd.Parameters.AddWithValue("@date", date);
                tableCmd.Parameters.AddWithValue("@quantity", quantity);

                tableCmd.ExecuteNonQuery();
                connection.Close();

            }

        }

        private static void Delete()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("What Record would you like to delete?");
            using (var connection = new SqliteConnection(connectionstring))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                tableCmd.CommandText = "DELETE FROM drinking_water WHERE Id = @id";
                tableCmd.Parameters.AddWithValue("@id", recordId);

                int rowcount = tableCmd.ExecuteNonQuery();

                if (rowcount == 0)
                {
                    Console.WriteLine($"Record with {recordId} does not exist");
                    Delete();
                }
                else
                    Console.WriteLine($"Record {recordId} deleted.");
            }

        }

        private static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("Please type Id of the record you want to update. Type 0 to return to main menu.\n\n");

            using (var connection = new SqliteConnection(connectionstring))
            {
                connection.Open();

                var checkcmd = connection.CreateCommand();
                checkcmd.CommandText = "SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = @id)";
                checkcmd.Parameters.AddWithValue("@id", recordId);

                int checkquery = Convert.ToInt32(checkcmd.ExecuteScalar());

                if (checkquery == 0)
                {
                    Console.WriteLine($"Record with Id {recordId} dosen't exist.\n\n");
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();

                int Quantity = GetNumberInput("Please insert number of glasses, no decimals allowed.\n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water set date = '{date}', quantity = {Quantity} WHERE Id = {recordId} ";

                tableCmd.ExecuteNonQuery();
                connection.Close();



            }
        }

        public class Drinkingwater
        {
            public int Id { get; set; }

            public DateTime date { get; set; }

            public int Quantity { get; set; }



        }




    }

}


