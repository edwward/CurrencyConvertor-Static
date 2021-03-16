using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrencyConvertor_Static
{
    
    public partial class MainWindow : Window
    {
        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            lblCurrency.Content = "Hello Dear User";
            BindCurrency();
            
        }
       

        private void BindCurrency()         //fill combobox "from"
        {
            DataTable dataTableCurrency = new DataTable();
            dataTableCurrency.Columns.Add("Text");
            dataTableCurrency.Columns.Add("Value");

            dataTableCurrency.Rows.Add("--SELECT--", 0);
            dataTableCurrency.Rows.Add("INR", 1);
            dataTableCurrency.Rows.Add("USD", 75);
            dataTableCurrency.Rows.Add("EUR", 85);
            dataTableCurrency.Rows.Add("SAR", 20);
            dataTableCurrency.Rows.Add("POUND", 5);
            dataTableCurrency.Rows.Add("DEM", 43);

            cmbFromCurrency.ItemsSource = dataTableCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";     //text "INR" "USD" etc.
            cmbFromCurrency.SelectedValuePath = "Value";    //value = numbers next to text
            cmbFromCurrency.SelectedIndex = 0;          //first item to show in combobox - here "SELECT"

            cmbToCurrency.ItemsSource = dataTableCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Declare ConvertedValue with double data type for store currency converted value
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information)
                cmbFromCurrency.Focus();
                return;
            }
            // Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

            //If From and To Combobox selects same value
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        { 
            Regex regex = new Regex("[^0-9]+");     //Allow only integer in TextBox
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cmbFromCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbToCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbFromCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void cmbToCurrency_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
        private void ClearControls()
        {
            try
            {
                txtCurrency.Text = string.Empty;

                if (cmbFromCurrency.Items.Count > 0)
                {
                    cmbFromCurrency.SelectedIndex = 0;
                }

                if (cmbToCurrency.Items.Count > 0)
                {
                    cmbToCurrency.SelectedIndex = 0;
                }

                lblCurrency.Content = "";

                txtCurrency.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
