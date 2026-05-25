using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ContactsDataAccessLayer;

namespace ContactsBuisinessLayer
{

    public class clsContact
    {
        // this will help us to determine which mode we are in when saving the contact
        enum enMode { AddNew, Update };
        enMode Mode = enMode.AddNew;

        // to prevent changing the ContactID from Presentation Layer
        public int ContactID { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int CountryID { get; set; }
        public string ImagePath { get; set; }

        // Constructor for add new contact mode when we want to create a new contact
        // and save it to the database
        public clsContact()
        {
            ContactID = -1;
            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            Address = "";
            DateOfBirth = DateTime.Now;
            CountryID = -1;

            Mode = enMode.AddNew;
        }

        // Constructor for update and read(find) mode when Contact Data is read from the database
        // and we want to create an instance of the contact with that data
        private clsContact(int contactID, string firstName, string lastName, string email, string phone, string address, DateTime dateOfBirth, int countryID, string imagePath)
        {
            ContactID = contactID;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Address = address;
            DateOfBirth = dateOfBirth;
            CountryID = countryID;
            ImagePath = imagePath;

            Mode = enMode.Update;
        }
        static public clsContact Find(int ContactID)
        {
            string FirstName = "", LastName = "", Email = "", Phone = "", Address = "", ImagePath = "";
            int CountryID = -1;
            DateTime DateOfBirth = DateTime.Now;

            bool isFound = clsContactDataAccess.GetContactByID(ContactID, ref FirstName, ref LastName, ref Email, ref Phone, ref Address, ref DateOfBirth, ref ImagePath, ref CountryID);

            if (isFound)
            {
                return new clsContact(ContactID, FirstName, LastName, Email, Phone, Address, DateOfBirth, CountryID, ImagePath);
            }
            else
            {
                return null;
            }

        }

        // this will not be static because it will be called on an instance of the contact that we want to save
        // like this: contact.Save();


        private bool _AddNewContact()
        {
            this.ContactID = clsContactDataAccess.AddNewContact(FirstName, LastName, Email, Phone, Address, DateOfBirth, CountryID, ImagePath);

            return this.ContactID != -1;
        }

        private bool _UpdateContact()
        {
            return clsContactDataAccess.UpdateContact(ContactID, FirstName, LastName, Email, Phone, Address, DateOfBirth, CountryID, ImagePath);
        }

        static public bool Delete(int ContactID)
        {
            return clsContactDataAccess.DeleteContact(ContactID);
        }

        static public DataTable GetAllContacts()
        {
            return clsContactDataAccess.GetAllContacts();
        }

        static public bool IsContactExist(int ID)
        {
            return clsContactDataAccess.IsContactExist(ID);
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewContact())
                    {
                        // change the mode to update 
                        // because otherwise if we call save again, it will try to add a new contact
                        // to the database instead of updating the existing one
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateContact();

                default:
                    return false;
            }


        }

        static public DataTable SearchContacts(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            if (int.TryParse(text, out int ID))
            {
                return clsContactDataAccess.SearchContactsByID(ID);
            }
            else
            {
                return clsContactDataAccess.SearchContactsByName(text);
            }
        }
    }


}
