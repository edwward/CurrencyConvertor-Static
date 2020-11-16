using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
        //Create object for SqlConnection
        SqlConnection con = new SqlConnection();

        //Create an object for SqlCommand
        SqlCommand cmd = new SqlCommand();

        //Create object for SqlDataAdapter
        SqlDataAdapter da = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            GetData();
            
        }
        public void MyConnection()
        {
            //Database connection string
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);

            //Open the connection
            con.Open();
        }
        private void BindCurrency()         //fill combobox "from"
        {
            MyConnection();
            DataTable dt = new DataTable();  //Create an object for DataTable
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);  //Write query for get data from Currency_Master table
            cmd.CommandType = CommandType.Text;     //CommandType define which type of command we use for write a query
            da = new SqlDataAdapter(cmd);           //It accepts a parameter that contains the command text of the object's selectCommand property.
            da.Fill(dt);

            DataRow newRow = dt.NewRow();               //Create an object for DataRow
            newRow["Id"] = 0;                           //Assign a value to Id column
            newRow["CurrencyName"] = "--SELECT--";      //Assign value to CurrencyName column
            dt.Rows.InsertAt(newRow, 0);                //Insert a new row in dt with the data at a 0 position
                                                        //The dt is not null and rows count greater than 0
            if (dt != null && dt.Rows.Count > 0)
            {
                cmbFromCurrency.ItemsSource = dt.DefaultView;       //Assign the datatable data to from currency combobox using ItemSource property.
                cmbToCurrency.ItemsSource = dt.DefaultView;         //Assign the datatable data to to currency combobox using ItemSource property.
            }
            con.Close();

            //To display the underlying datasource for cmbFromCurrency
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";


            //To use as the actual value for the items
            cmbFromCurrency.SelectedValuePath = "Id";

            //Show default item in Combobox
            cmbFromCurrency.SelectedValue = 0;

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedValue = 0;

        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Declare ConvertedValue with double data type for store currency converted value
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount Textbox is Null or Blank show the below message box
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //After click on OK button set focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or select default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus to From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            // Else if currency To is not selected or select default text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //Show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //Set focus to To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //If From and To Combobox selects same value
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //Amount textbox value set in ConvertedValue. double.parse is used for change datatype from string to double.
                //Textbox text have string and ConvertedValue as double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);
                //Show the label converted currency and converted currency name.
                //Tostring("N3") is used to place 000 after the dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                //Calculation for currency converter is From currency value multiplied(*) with the amount textbox value and then the total is divided(/) with To currency value.
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                //Show the label converted currency and converted currency name.
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
                txtCurrency.Text = string.Empty;        //Clear amount textbox text

                if (cmbFromCurrency.Items.Count > 0)        //From currency combobox items count greater than 0
                {
                    cmbFromCurrency.SelectedIndex = 0;      //Set from currency combobox selected item hint
                }

                if (cmbToCurrency.Items.Count > 0)          //To currency combobox items count greater than 0
                {
                    cmbToCurrency.SelectedIndex = 0;        //Set to currency combobox selected item hint
                }

                lblCurrency.Content = "";       //Clear a label text
                txtCurrency.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check the validation
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")  //is text box empty?
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else 
                {
                    if (CurrencyId != 0 && CurrencyId > 0)      //update button
                    {
                        if (MessageBox.Show("Are you sure you want to Update ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)      //show confirmation message
                        {
                            MyConnection();     //connection
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data Updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else 
                    {
                        if (MessageBox.Show("Are you sure you want to Save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)        //create new row to table
                        {
                            MyConnection();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        
        private void ClearMaster()  //Method is used to clear all the input which the user has entered in currency master tab
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        public void GetData()                   //Bind Data in DataGrid View.
        {
            MyConnection();                     //The method is used for connect with database and open database connection 
            DataTable dt = new DataTable();      //Create Datatable object
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con); //Write Sql Query for Get data from database table. Query written in double quotes and after comma provide connection
            cmd.CommandType = CommandType.Text; //CommandType define Which type of command execute like Text, StoredProcedure, TableDirect.
            da = new SqlDataAdapter(cmd);    //It is accept a parameter that contains the command text of the object's SelectCommand property.
            da.Fill(dt);                    //The DataAdapter serves as a bridge between a DataSet and a data source for retrieving and saving data. The Fill operation then adds the rows to destination DataTable objects in the DataSet 

            if (dt != null && dt.Rows.Count > 0)    //dt is not null and rows count greater than 0
            {   
                dgvCurrency.ItemsSource = dt.DefaultView;   //Assign DataTable data to dgvCurrency using ItemSource property.  
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            con.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //DataGrid selected cell changed event
        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;    //Create object for DataGrid from sender object
                DataRowView row_selected = grd.CurrentItem as DataRowView;  //Create object for DataRowView

                if (row_selected != null)
                {
                    if (dgvCurrency.Items.Count > 0)    //dgvCurrency items count greater than zero
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString()); //Get selected row Id column value and Set in CurrencyId variable
                            
                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)  //DisplayIndex is equal to zero than it is Edit cell
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();  //Get selected row Amount column value and Set in Amount textbox
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString();  //Get selected row CurrencyName column value and Set in CurrencyName textbox
                                btnSave.Content = "Update";  //Change save button text Save to Update
                            }
                                               
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)  //DisplayIndex is equal to one than it is Delete cell 
                            {
                                if (MessageBox.Show("Are you sure you want to delete ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)  //Show confirmation dialogue box
                                {
                                    MyConnection();
                                    DataTable dt = new DataTable();

                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);   //Execute delete query for delete record from table using Id
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);  //CurrencyId set in @Id parameter and send it in delete statement
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
