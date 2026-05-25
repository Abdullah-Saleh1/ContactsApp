using ContactsBuisinessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;


namespace ContactApp_PresentationLayer
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DataView dataView;
        private DispatcherTimer _searchTimer;

        private void _LoadAllContacts()
        {
            dataView = clsContact.GetAllContacts()?.DefaultView; 
            if ( dataView != null ) ContactsGrid.ItemsSource = dataView;

            txt_Search.Clear(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataView = new DataView();
            _LoadAllContacts();

            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(300);
            _searchTimer.Tick += SearchTimer_Tick;
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            ApplySearch();
        }


        // Search Functionality
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button?.TemplatedParent is TextBox textBox)
            {
                textBox.Clear();
            }

            _LoadAllContacts(); 
        }

        private void ApplySearch()
        {
            string txt = txt_Search.Text.Trim();

            if (string.IsNullOrEmpty(txt))
            {
                _LoadAllContacts(); 
                return;
            }

            //dataView = clsContact.SearchContacts(txt)?.DefaultView;
            // 
            dataView.RowFilter =
                $"FirstName + ' ' + LastName LIKE '%{txt}%' OR Convert(ContactID, 'System.String') LIKE '%{txt}%'";


            if (dataView != null) ContactsGrid.ItemsSource = dataView;
            

        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            ApplySearch(); 
        }

        private void txt_Search_TextChanged(object sender, TextChangedEventArgs e)
        {

            _searchTimer.Stop();   // reset timer
            _searchTimer.Start();  // restart on every keystroke
        }


        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            AddEditContact AddContact = new AddEditContact(-1);
            AddContact.ShowDialog();

            _LoadAllContacts();
        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsGrid.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please select a contact to edit.");
                return;
            }

            int ContactID = Convert.ToInt32((ContactsGrid.SelectedItem as DataRowView)["ContactID"]);
            AddEditContact EditContact = new AddEditContact(ContactID); 
            EditContact.ShowDialog();

            _LoadAllContacts();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {

            if (ContactsGrid.SelectedItems.Count < 1)
            {
                MessageBox.Show("Please select a contact to delete.");
                return;
            }

            int ContactID = Convert.ToInt32((ContactsGrid.SelectedItem as DataRowView)["ContactID"]);

            if (MessageBox.Show("Are you sure you want to delete this contact?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (clsContact.Delete(ContactID))
                {
                    MessageBox.Show("Contact deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    _LoadAllContacts();
                } else
                {
                    MessageBox.Show("Failed to delete contact.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
