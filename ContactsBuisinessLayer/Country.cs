using System;
using System.Data;
using ContactsDataAccessLayer; 

namespace ContactsBuisinessLayer
{
    public class clsCountry
    {
        enum enMode { AddNew, Update };
        enMode Mode = enMode.AddNew;

        public int CountryID { get; private set; }

        public string Code { get; set; }
        public string PhoneCode { get; set; }
        public string CountryName { get; set; }

        public clsCountry()
        {
            CountryID = -1;
            CountryName = "";
            Code = "";
            PhoneCode = ""; 

            Mode = enMode.AddNew;
        }

        private clsCountry(int CountryID, string CountryName, string Code, string PhoneCode)
        {
            this.CountryID = CountryID;
            this.CountryName = CountryName;
            this.Code = Code;
            this.PhoneCode = PhoneCode; 

            Mode = enMode.Update;
        }

        static public clsCountry Find(int CountryID)
        {
            string CountryName = "", Code = "", PhoneCode = "";
            if (clsCountryDataAccess.GetCountryByID(CountryID, ref CountryName, ref Code, ref PhoneCode))
            {
                return new clsCountry(CountryID, CountryName, Code, PhoneCode);
            }
            return null;
        }

        static public clsCountry Find(string CountryName)
        {
            int CountryID = -1;
            string Code = "", PhoneCode = "";
            if (clsCountryDataAccess.GetCountryByName(CountryName, ref CountryID, ref Code, ref PhoneCode))
            {
                return new clsCountry(CountryID, CountryName, Code, PhoneCode);
            }
            return null;
        }

        static public bool IsCountryExist(int CountryID)
        {
            return clsCountryDataAccess.IsCountryExist(CountryID);
        }

        static public bool IsCountryExist(string CountryName)
        {
            return clsCountryDataAccess.IsCountryExist(CountryName);
        }

        static public DataTable GetAllCountries()
        {
            return clsCountryDataAccess.GetAllCountries();
        }

        private bool _AddNewCountry()
        {
            this.CountryID = clsCountryDataAccess.AddNewCountry(CountryName, Code, PhoneCode);
            return this.CountryID != -1;
        }
        private bool _UpdateCountry()
        {
            return clsCountryDataAccess.UpdateCountry(CountryID, CountryName, Code, PhoneCode);
        }

        static public bool Delete(int CountryID)
        {
            return clsCountryDataAccess.DeleteCountry(CountryID); 
        }

        public bool Save()
        {
            switch(Mode)
            {
                case enMode.AddNew:

                    if (_AddNewCountry())
                    {
                        this.Mode = enMode.Update; 
                        return true; 
                    }
                    return false; 

                case enMode.Update:
                    return _UpdateCountry();  ;
                default:
                    return false;
            }
        }
    }
}   
