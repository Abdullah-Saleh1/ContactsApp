using ContactsBuisinessLayer;
using Microsoft.Win32;
using System;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ContactApp_PresentationLayer
{
    /// <summary>
    /// Interaction logic for AddEditContact.xaml
    /// </summary>
    public partial class AddEditContact : Window
    {

        DataView CountriesDataView = new DataView(); 
        enum enMode { Add, Edit }
        private enMode _mode;
        private int _ContactID = -1;
        private clsContact _Contact = new clsContact();

        public AddEditContact(int ContactID)
        {
            InitializeComponent();

            _LoadCountries();
            CountriesComboBox.SelectedValue = 1; 
            
            this._ContactID = ContactID;

            _mode = ContactID == -1 ? enMode.Add : enMode.Edit; 
        }

        public void _LoadImage(string path, System.Windows.Controls.Image control)
        {
            if (System.IO.File.Exists(path))
                control.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            btnRemoveImage.Visibility = Visibility.Visible;
        }

        private void DisplayContactData()
        {
            _Contact = clsContact.Find(_ContactID);


            if (_Contact != null)
            {
                txtContactID.Text = _ContactID.ToString(); 
                txtFirstName.Text = _Contact.FirstName;
                txtLastName.Text = _Contact.LastName;
                txtEmail.Text = _Contact.Email;
                txtPhone.Text = _Contact.Phone;
                txtAddress.Text = _Contact.Address;
                dtpDateOfBirth.Value = _Contact.DateOfBirth;
                CountriesComboBox.SelectedValue = _Contact.CountryID; 
                CountriesComboBox.SelectedValue = _Contact.CountryID;

                if (!string.IsNullOrEmpty(_Contact.ImagePath))
                {
                    _LoadImage(_Contact.ImagePath, imgContact);
                }

            } else
            {
                MessageBox.Show("Contact not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        } 

        private void _LoadCountries()
        {
            CountriesDataView = clsCountry.GetAllCountries().DefaultView;
            CountriesComboBox.ItemsSource = CountriesDataView;
            dtpDateOfBirth.Value = DateTime.Now; 
        }

        private void btnSetImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _Contact.ImagePath = openFileDialog.FileName;
                _LoadImage(_Contact.ImagePath, imgContact); 
                //if (System.IO.File.Exists(_Contact.ImagePath))
                //{
                //    imgContact.Source = new BitmapImage(new Uri(_Contact.ImagePath, UriKind.Absolute));
                //}
            }
        }

        private bool _CheckFields()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Please enter First Name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFirstName.Focus(); // ينقل المؤشر على الفيلد الفاضي علطول
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter Last Name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter Email Address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please enter Phone Number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Please enter Address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtAddress.Focus();
                return false;
            }

            if (dtpDateOfBirth.Value == null)
            {
                MessageBox.Show("Please select a Date of Birth.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dtpDateOfBirth.Focus();
                return false;
            }

            if (CountriesComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a Country.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                CountriesComboBox.Focus();
                return false;
            }

            return true; 
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (!_CheckFields()) return; 

            _Contact.FirstName = txtFirstName.Text.Trim();
            _Contact.LastName = txtLastName.Text.Trim();
            _Contact.Email = txtEmail.Text.Trim();
            _Contact.Phone = txtPhone.Text.Trim();
            _Contact.Address = txtAddress.Text.Trim();
            _Contact.DateOfBirth = dtpDateOfBirth.Value ?? DateTime.Now;
            _Contact.CountryID = (int)CountriesComboBox.SelectedValue; // Selected Value returns an object, so we need to convert it to int
            // we make the selected value is CountryID and the DisplayMember is CountryName in xaml file

            string imagePath = "";
            if (imgContact.Source is BitmapImage bitmap && bitmap.UriSource != null)
            {
                imagePath = bitmap.UriSource.LocalPath;
            }

            _Contact.ImagePath = imagePath;

            if (_Contact.Save())
            {
                MessageBox.Show("Contact saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            } else
            {
                MessageBox.Show("Failed to save contact.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _mode = enMode.Edit;
            txtContactDetails.Text = "Edit Contact";
            txtContactID.Text = _Contact.ContactID.ToString(); 
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _LoadCountries(); 

            if (_mode == enMode.Edit)
            {
                txtContactDetails.Text = "Edit Contact";
                DisplayContactData();
            }
            else
            {
                txtContactDetails.Text = "Add New Contact";
            } 
            
        }

        private void btnRemoveImage_Click(object sender, RoutedEventArgs e)
        {

            imgContact.Source = null; 
            _Contact.ImagePath = ""; 

            btnRemoveImage.Visibility = Visibility.Hidden; 
        }
    }
}
