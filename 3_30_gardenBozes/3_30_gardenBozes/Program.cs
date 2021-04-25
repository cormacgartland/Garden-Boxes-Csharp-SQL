using System;
using System.Data.SqlClient;

namespace _3_30_gardenBozes
{
    class Program
    {
        static void Main(string[] args)
        {
            string dB = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\corma\acad_pgh\wk_6_git\garden-boxes\3_30_gardenBozes\3_30_gardenBozes\Database1.mdf;Integrated Security=True";

            // Opening Message
            Console.WriteLine("Welcome. Please type 'Menu' to view the menu.");
            string nav = Console.ReadLine().ToLower();

            // Program Loop
            bool menuLoop = true;
            while (menuLoop)
            {
                
                if (nav == "menu")
                {
                    Console.Clear();
                    Console.WriteLine("Please Enter A Number To Select:");
                    Console.WriteLine("1) Garden Box Calculator");
                    Console.WriteLine("2) Add Crop To Selection");
                    Console.WriteLine("3) Exit");

                    nav = Console.ReadLine();

                    if (nav == "1")
                    {
                        // Starts garden box calculations
                        Console.Clear();
                        int area = gardenBoxCalc();

                        // Crash check. Make sure values are correct data types
                        bool calcLoop = true;
                        while (calcLoop)
                        {
                            // Converts user input into int 
                            string answer = plantSelect(dB);
                            int id;
                            bool idIsInt = int.TryParse(answer, out id);


                            if (idIsInt == true)
                            {
                                plantCalc(dB, id, area);
                                calcLoop = false;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please try again.");
                                Console.WriteLine();
                            }
                            
                        }
                        Console.WriteLine("Enter 'Menu' to go back");
                        nav = Console.ReadLine();

                    }
                    else if (nav == "2")
                    {
                        // Add a crop to the database
                        Console.Clear();
                        addCrop(dB);
                        Console.WriteLine("Enter 'Menu' to go back");
                        nav = Console.ReadLine();
                    }
                    else if (nav == "3")
                    {
                        // Exit the program
                        menuLoop = false;
                        Console.Clear();
                        Console.WriteLine("Goodbye!");
                    }
                    else
                    {  
                        // Accepts only one of the choices provided
                        nav = "menu";                 
                    }
                }
                else
                {
                    // The program really really wants you to type menu
                    Console.Clear();
                    Console.WriteLine("Please type 'Menu' to view the menu.");
                    nav = Console.ReadLine().ToLower();
                }

            }

        }

        static int gardenBoxCalc()
        {
            // Calculate dimmensions of the garden box
            Console.WriteLine("What is the length of your garden bed in feet:");            
            int length = inputProtect();            
            Console.WriteLine();

            Console.WriteLine("What is the width of your garden bed in feet:");           
            int width = inputProtect();
            Console.WriteLine();

            int area = length * width;

            return area;
        }

        static string plantSelect(string dB)
        {
            // Select a crop
            Console.Clear();
            Console.WriteLine("Please choose a number from the list:");
            SqlConnection conn = new SqlConnection(dB);
            conn.Open();
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Plants", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            // Prints all options from database
            while (reader.Read())
            {
                Console.WriteLine($"{reader["id"]}) {reader["Type"]}");               
            }
            Console.WriteLine();
            reader.Close();
            conn.Close();

            // Takes user input as a string (later converted to an int)
            string userAnswer = Console.ReadLine();

            return userAnswer;
        }

        static void plantCalc(string dB, int userAnswer, int area)
        {
            SqlConnection conn = new SqlConnection(dB);
            conn.Open();
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Plants", conn);
            SqlDataReader reader = cmd.ExecuteReader();

            // Just some light plant math. nbd.
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["id"]);

                if (userAnswer == id)
                {
                    double amtPerSqFt = Convert.ToDouble(reader["amt_per_sqft"]);

                    double plantMath = area * amtPerSqFt / 16;
                    Console.WriteLine($"Your garden is {area} sqft.\nYou can plant {Math.Floor(plantMath)} {reader["Type"]} crop(s)!");
                }
                
            }
            reader.Close();
            Console.WriteLine();
            conn.Close();             
        }

        static void addCrop(string dB)
        {

            Console.WriteLine("What type of crop would you like to plant?");
            string plantType= Console.ReadLine();
            Console.WriteLine("How many could you fit in a 4 x 4 plot?");
            string plantAmt = Console.ReadLine();
            int plantAmtInt;
            bool plantAmtIsInt = int.TryParse(plantAmt, out plantAmtInt); // as long as it's an int the program will run

            if (plantAmtIsInt == true) // Since the value isn't an int, this never runs so the db is unchanged
            {

                SqlConnection conn = new SqlConnection(dB);
                conn.Open();
                SqlCommand cmd = new SqlCommand($"INSERT INTO Plants (Type, amt_per_sqft) VALUES ('{plantType}', '{plantAmtInt}')", conn);
                cmd.ExecuteNonQuery();

                conn.Close();
                Console.WriteLine("Crop entered!");
            }
            else
            {
                Console.WriteLine("Invalid Entry. Please try again.");
            }
                     
        }

        static int inputProtect()
        {
            // the job of this function is to return an int NO MATTER WHAT
            string input = Console.ReadLine();
            int conversion = 0;

            bool inputLoop = true;
            while (inputLoop)
            {
                
                bool parsedStr = int.TryParse(input, out conversion);

                if (parsedStr != true) // if the conversion worked, make 
                {
                    Console.WriteLine("Invalid input");
                    input = Console.ReadLine();
                }
                else
                {
                    return conversion;
                    inputLoop = false;
                }
            }
            return conversion;
        }
    }
}
