using System;
using System.Data;
using System.Data.SqlClient;


namespace ContactsDataAccessLayer
{
    public class clsContactDataAccess
    {
        public static bool GetContactByID(int ContactID, ref string FirstName, ref string LastName, ref string Email, ref string Phone, ref string Address, ref DateTime DateOfBirth, ref string ImagePath, ref int CountryID)
        {
            string query = "select * from Contacts where ContactID = @ContactID";
            bool isFound = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ContactID", ContactID); // Parameterized Query to prevent SQL Injection
                    conn.Open();

                    using (SqlDataReader Reader = command.ExecuteReader())
                    {
                        if (Reader.Read())
                        {
                            isFound = true;

                            FirstName = (string)Reader["FirstName"];
                            LastName = (string)Reader["LastName"];
                            Email = (string)Reader["Email"];
                            Phone = (string)Reader["Phone"];
                            Address = (string)Reader["Address"];
                            DateOfBirth = (DateTime)Reader["DateOfBirth"];
                            CountryID = (int)Reader["CountryID"];


                            ImagePath = Reader["ImagePath"] == DBNull.Value ? "" : (string)Reader["ImagePath"];

                        }
                        else
                        {
                            isFound = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                isFound = false;
            }
            return isFound;
        }

        public static int AddNewContact(string FirstName, string LastName, string Email, string Phone, string Address, DateTime DateOfBirth, int CountryID, string ImagePath)
        {
            string query = "insert into Contacts (FirstName, LastName, Email, Phone, Address, DateOfBirth, CountryID, ImagePath) values (@FirstName, @LastName, @Email, @Phone, @Address, @DateOfBirth, @CountryID, @ImagePath); SELECT SCOPE_IDENTITY();";
            int newContactID = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@CountryID", CountryID);

                    // If ImagePath is empty, we should insert NULL instead of an empty string to the database
                    if (ImagePath == "" || ImagePath == null) 
                    {
                        command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ImagePath", ImagePath);
                    }

                    conn.Open();
                    object InsertedID = command.ExecuteScalar();
                    if (InsertedID != null)
                    {
                        newContactID = Convert.ToInt32(InsertedID);
                        return newContactID;
                    }
                    return newContactID;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                return newContactID;
            }
        }

        public static bool UpdateContact(int ContactID, string FirstName, string LastName, string Email, string Phone, string Address, DateTime DateOfBirth, int CountryID, string ImagePath)
        {
            string query = "update Contacts set FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, Address = @Address, DateOfBirth = @DateOfBirth, CountryID = @CountryID, ImagePath = @ImagePath where ContactID = @ContactID";
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {


                    conn.Open();
                    command.Parameters.AddWithValue("@ContactID", ContactID);
                    command.Parameters.AddWithValue("@FirstName", FirstName);
                    command.Parameters.AddWithValue("@LastName", LastName);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Address", Address);
                    command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                    command.Parameters.AddWithValue("@CountryID", CountryID);
                    // If ImagePath is empty, we should update it to NULL instead of an empty string in the database
                    if (ImagePath == "")
                    {
                        command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@ImagePath", ImagePath);
                    }


                    int affectedRows = Convert.ToInt32(command.ExecuteNonQuery());
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                return false;
            }

        }

        public static bool DeleteContact(int ContactID)
        {
            string query = "delete from Contacts where ContactID = @ContactID";
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ContactID", ContactID);
                    conn.Open();
                    int affectedRows = Convert.ToInt32(command.ExecuteNonQuery());
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                return false;
            }
        }

        public static DataTable GetAllContacts()
        {
            string query = "select * from Contacts";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                return null;
            }

            return dt;
        }

        public static bool IsContactExist(int ContactID)
        {
            string query = "select Found=1 from Contacts where ContactID = @ContactID";

            bool IsFound = false; 
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ContactID", ContactID);
                    conn.Open();


                    IsFound = Convert.ToInt32(command.ExecuteScalar()) > 0;                    
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                IsFound = false; 
            }

            return IsFound; 
        }

        public static DataTable SearchContactsByName(string text)
        {
            string query = @"SELECT * FROM Contacts 
                 WHERE FirstName LIKE @Text 
                 OR LastName LIKE @Text
                 OR Email LIKE @Text";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Text", $"%{text}%");
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                return null;
            }

            return dt;
        }

        public static DataTable SearchContactsByID(int ID)
        {
            string query = @"SELECT * FROM Contacts 
                 WHERE ContactID LIKE @ID";
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ID", $"%{ID}%");
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dt.Load(reader);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error: {ex.Message}"); Logging the error later
                return null;
            }

            return dt;
        }
    }
}
