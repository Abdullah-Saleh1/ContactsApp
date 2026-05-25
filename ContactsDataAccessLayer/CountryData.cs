using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;

namespace ContactsDataAccessLayer
{
    public class clsCountryDataAccess
    {
        static public bool GetCountryByID(int CountryID, ref string CountryName, ref string Code, ref string PhoneCode)
        {
            string query = "select CountryName, Code, PhoneCode from Countries where CountryID = @CountryID";
            bool IsFound = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryID", CountryID);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    if (reader.HasRows)
                    {
                        IsFound = true; 
                        if (reader.Read())
                        {
                            CountryName = reader["CountryName"].ToString(); // no validtion required because it's not null field
                            Code = reader["Code"] as string ?? ""; // allow null
                            PhoneCode = reader["PhoneCode"] as string ?? ""; // allow null
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsFound = false; 
            }

            return IsFound;
        }

        static public bool GetCountryByName(string CountryName, ref int CountryID, ref string Code, ref string PhoneCode)
        {
            string query = "select CountryID, Code, PhoneCode from Countries where CountryName = @CountryName";
            bool IsFound = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryName", CountryName);
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            CountryID = Convert.ToInt32(reader["CountryID"]);
                            Code = reader["Code"] as string ?? "";
                            PhoneCode = reader["PhoneCode"] as string ?? "";
                            IsFound = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsFound = false;
            }

            return IsFound;
        }

        static public bool IsCountryExist(int CountryID)
        {
            string query = "select Found=1 from Countries where CountryID = @CountryID";
            bool isFound = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryID", CountryID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        isFound = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            return isFound;
        }

        static public bool IsCountryExist(string CountryName)
        {
            string query = "select Found=1 from Countries where CountryName = @CountryName";
            bool isFound = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryName", CountryName);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        isFound = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            return isFound;
        }

        static public DataTable GetAllCountries()
        {
            DataTable dt = new DataTable();
            string query = "select * from Countries";
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
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
                // Log the error late
                return null;
            }

            return dt;
        }

        static public int AddNewCountry(string CountryName, string Code, string PhoneCode)
        {
            string query = "insert into Countries (CountryName, Code, PhoneCode) values (@CountryName, @Code, @PhoneCode); SELECT SCOPE_IDENTITY();";
            int newCountryID = -1;

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryName", CountryName);
                    cmd.Parameters.AddWithValue("@Code", Code);
                    cmd.Parameters.AddWithValue("@PhoneCode", PhoneCode);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        newCountryID = Convert.ToInt32(result);
                    }

                }
            }
            catch (Exception ex)
            {
                newCountryID = -1;

            }

            return newCountryID;
        }

        static public bool UpdateCountry(int CountryID, string CountryName, string Code, string PhoneCode)
        {
            string query = "update Countries set CountryName = @CountryName, Code = @Code, PhoneCode = @PhoneCode where CountryID = @CountryID";
            int rowsAffected = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryID", CountryID);
                    cmd.Parameters.AddWithValue("@CountryName", CountryName);
                    cmd.Parameters.AddWithValue("@Code", Code);
                    cmd.Parameters.AddWithValue("@PhoneCode", PhoneCode);
                    conn.Open();

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                rowsAffected = 0;
            }

            return rowsAffected > 0;
        }

        static public bool DeleteCountry(int CountryID)
        {

            string query = "delete from Countries where CountryID = @CountryID";
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CountryID", CountryID);
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                rowsAffected = 0;
            }
            return rowsAffected > 0;
        }
    }
}
