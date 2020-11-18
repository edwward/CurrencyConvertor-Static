using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
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
       
        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            GetValue();
        }

        Root value = new Root();

        private async void GetValue()
        {
            value = await GetDataGetMethod<Root>("https://openexchangerates.org/api/latest.json?app_id=69cb235f2fe74f03baeec270066587cf");      //reference to API openexchangerates.org
            BindCurrency();     //when GetValue gets response from API, call BindCurrency to fill in the fields in app
        }
        private async Task<Root> GetDataGetMethod<T>(string url)  //send a GET request for currency value
        {
            var s = new Root();
            try
            {
                using (var client = new HttpClient())       //HttpClient class provides a base class for receiving/sending the HTTP requests/responses from a URL
                {
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync();
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);
                        return ResponseObject;
                    }
                    return s;
                }
            }
            catch
            {
                return s;
            }
        }

        private void BindCurrency()         //fill combobox "from"
        {
            DataTable dataTableCurrency = new DataTable();
            dataTableCurrency.Columns.Add("Text");
            dataTableCurrency.Columns.Add("Rate");

            //Add rows in Datatable with text and value, set a value which are fetched from API
            dataTableCurrency.Rows.Add("--SELECT--", 0);
            dataTableCurrency.Rows.Add("INR", value.rates.INR);
            dataTableCurrency.Rows.Add("USD", value.rates.USD);
            dataTableCurrency.Rows.Add("NZD", value.rates.NZD);
            dataTableCurrency.Rows.Add("JPY", value.rates.JPY);
            dataTableCurrency.Rows.Add("EUR", value.rates.EUR);
            dataTableCurrency.Rows.Add("CAD", value.rates.CAD);
            dataTableCurrency.Rows.Add("ISK", value.rates.ISK);
            dataTableCurrency.Rows.Add("PHP", value.rates.PHP);
            dataTableCurrency.Rows.Add("DKK", value.rates.DKK);
            dataTableCurrency.Rows.Add("CZK", value.rates.CZK);

            cmbFromCurrency.ItemsSource = dataTableCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";     //text "INR" "USD" etc.
            cmbFromCurrency.SelectedValuePath = "Rate";    //value = numbers next to text
            cmbFromCurrency.SelectedIndex = 0;          //first item to show in combobox - here "SELECT"

            cmbToCurrency.ItemsSource = dataTableCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Rate";
            cmbToCurrency.SelectedIndex = 0;
        }


        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Declare ConvertedValue with double data type to store currency converted value
            double ConvertedValue;

            //Check if amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank then show this message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on the Messagebox's OK button set the focus on amount textbox
                txtCurrency.Focus();
                return;
            }

            //Else if currency From is not selected or default text as --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0 || cmbFromCurrency.Text == "--SELECT--")
            {
                //Then show this message box
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //Set the focus to From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //else if currency To is not Selected or default text as --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0 || cmbToCurrency.Text == "--SELECT--")
            {
                //Then show this message box
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set the focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //If From and To Combobox selects the same value
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value is set in ConvertedValue. double.parse is used to change datatype String To Double. Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show the label as converted currency and converted currency name. and ToString("N3") is used to placed 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currency converter is From currency value multiplied(*) with amount textbox value and then that total divided(/) with To currency value.
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());

                //Show the label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
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

            //Clear amount textbox text
            txtCurrency.Text = string.Empty;

            //From currency combobox items count greater than 0
            if (cmbFromCurrency.Items.Count > 0)
            {
                //Set from currency combobox selected item hint
                cmbFromCurrency.SelectedIndex = 0;
            }

            //To currency combobox items count greater than 0
            if (cmbToCurrency.Items.Count > 0)
            {
                //Set to currency combobox selected item hint
                cmbToCurrency.SelectedIndex = 0;
            }

            //Clear a label text
            lblCurrency.Content = "";

            //Set focus on amount textbox
            txtCurrency.Focus();
        }

    }
}

