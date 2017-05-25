using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace List_of_products
{
    /// <summary>
    /// Логика взаимодействия для AddCustomers.xaml
    /// </summary>
    public partial class AddCustomers : Window
    {
        public bool isAdd=false;//показатель, который указывает было ли совершено добавление данных
        String connectionString;
        string sql;
        SqlDataAdapter adapter;
        DataSet dataSet, dataSetUsers, dataSetProduct;
        public AddCustomers()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            InitializeComponent();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                sql = "SELECT * From Customers";
                adapter = new SqlDataAdapter(sql, connection);
                dataSetUsers = new DataSet();
                adapter.Fill(dataSetUsers);
                foreach (DataRow dr in dataSetUsers.Tables[0].Rows)
                {
                    NameOldCust.Items.Add(dr.ItemArray[1].ToString());
                }
            }
            getDSProduct();
        }
        private void getDSProduct()
            //получение данных о всех продуктах
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                sql = "SELECT * From Products";
                adapter = new SqlDataAdapter(sql, connection);
                dataSetProduct = new DataSet();
                adapter.Fill(dataSetProduct);
            }
        }
        private void OK_Click(object sender, RoutedEventArgs e)
            //добавление данных в БД
        {
            int idUsers;
            if (ListBuy.Items.Count > 0)
            {
                if(NewCust.IsChecked==true)
                { 
                    if(NameNewCust.Text!="")
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            if (connection.State != ConnectionState.Open)
                                connection.Open();
                            sql = "INSERT INTO Customers(Name) " +
                        "VALUES (N\'" + NameNewCust.Text + "\')";
                            var cmd = new SqlCommand(sql, connection);
                            cmd.ExecuteNonQuery();
                            DataSet ds = new DataSet();
                            sql = "SELECT MAX(c.Customers_id) From Customers c";
                            adapter = new SqlDataAdapter(sql, connection);
                            adapter.Fill(ds);
                            idUsers =Convert.ToInt16(ds.Tables[0].Rows[0].ItemArray[0]);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не выбран покупатель");
                        return;
                    }
                }
                else
                { 
                    if(NameOldCust.SelectedIndex>-1)
                        idUsers=Convert.ToInt16(dataSetUsers.Tables[0].Rows[NameOldCust.SelectedIndex].ItemArray[0]);
                    else
                    {
                        MessageBox.Show("Не выбран покупатель");
                        return;
                    }
                }
                isAdd = true;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    foreach (DockPanel listBoxItem in ListBuy.Items)
                    {
                        ComboBox cb=(ComboBox)listBoxItem.Children[1];
                        TextBox tb=(TextBox)listBoxItem.Children[0];
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        sql = "INSERT INTO Prod_Cust(Id_products,Customers_id,Count_prod) "+
                        "VALUES ("+dataSetProduct.Tables[0].Rows[cb.SelectedIndex].ItemArray[0]+","+idUsers+","+Convert.ToInt16(tb.Text)+")" ;
                        var cmd = new SqlCommand(sql, connection);
                        cmd.ExecuteNonQuery();
                    }
                }
                this.Close();
            }
            else
                MessageBox.Show("Не добавлено ни одного купленного продукта!");
        }

        private void Cansel_Click(object sender, RoutedEventArgs e)
            //отмена действий
        {
            this.Close();
        }

        private void OldCust_Checked(object sender, RoutedEventArgs e)
        {
            NameOldCust.IsEnabled = true;
            NameNewCust.IsEnabled = false;
        }

        private void NewCust_Checked(object sender, RoutedEventArgs e)
        {
            if (NameNewCust != null)
            {
                NameOldCust.IsEnabled = false;
                NameNewCust.IsEnabled = true;
            }
        }

        private void AddBuy_Click(object sender, RoutedEventArgs e)
            //добавление новой купленной вещи
        {
            DockPanel dockPanel = new DockPanel();
            dockPanel.MinWidth = 500;
            TextBox textBox = new TextBox();
            textBox.ToolTip="Количество купленного";
            textBox.Margin = new Thickness(5);
            textBox.Text = "0";
            textBox.MinWidth = 50;
            DockPanel.SetDock(textBox,Dock.Right);
            ComboBox comboBox = new ComboBox();
            comboBox.Margin = new Thickness(5);
            comboBox.SelectedIndex = 0;
            comboBox.ToolTip = "Купленный продукт";
            foreach (DataRow dr in dataSetProduct.Tables[0].Rows)
            {
                comboBox.Items.Add(dr.ItemArray[1].ToString());
            }
            DockPanel.SetDock(comboBox, Dock.Right);
            dockPanel.Children.Add(textBox);
            dockPanel.Children.Add(comboBox);

            ListBuy.Items.Add(dockPanel);
        }

        private void ListBuy_SelectionChanged(object sender, SelectionChangedEventArgs e)
            // появления кнопки удаления купленного при выборе записи в списке
        {
            if (ListBuy.SelectedIndex > -1)
                DelBuy.IsEnabled = true;
            else
                DelBuy.IsEnabled = false;
        }

        private void DelBuy_Click(object sender, RoutedEventArgs e)
            //удаление купленного
        {
            ListBuy.Items.RemoveAt(ListBuy.SelectedIndex);
        }

        private void AddProd_Click(object sender, RoutedEventArgs e)
            //добавление продукта
        {
            UpdProduct updProduct = new UpdProduct(0,dataSetProduct);
            updProduct.ShowDialog();
            getDSProduct();
            if(updProduct.isAct)
            {
                foreach (DockPanel listBoxItem in ListBuy.Items)
                {
                    ComboBox cb = (ComboBox)listBoxItem.Children[1];
                    int index = cb.SelectedIndex;
                    cb.Items.Clear();
                    foreach (DataRow dr in dataSetProduct.Tables[0].Rows)
                    {
                        cb.Items.Add(dr.ItemArray[1].ToString());
                    }
                    cb.SelectedIndex = index;
                }
            }
        }

        private void UpdProd_Click(object sender, RoutedEventArgs e)
            //редактирование продукта
        {
            UpdProduct updProduct = new UpdProduct(1, dataSetProduct);
            updProduct.ShowDialog();
            getDSProduct();
            if (updProduct.isAct)
            {
                foreach (DockPanel listBoxItem in ListBuy.Items)
                {
                    ComboBox cb = (ComboBox)listBoxItem.Children[1];
                    int index = cb.SelectedIndex;
                    cb.Items.Clear();                 
                    foreach (DataRow dr in dataSetProduct.Tables[0].Rows)
                    {
                        cb.Items.Add(dr.ItemArray[1].ToString());
                    }
                    cb.SelectedIndex = index;
                }
            }
        }

        private void DelProd_Click(object sender, RoutedEventArgs e)
            //удаление продукта
        {
            UpdProduct updProduct = new UpdProduct(2, dataSetProduct);
            updProduct.ShowDialog();
            getDSProduct();
            if (updProduct.isAct)
            {
                foreach (DockPanel listBoxItem in ListBuy.Items)
                {
                    ComboBox cb = (ComboBox)listBoxItem.Children[1];
                    int index = cb.SelectedIndex;
                    cb.Items.Clear();                 
                    foreach (DataRow dr in dataSetProduct.Tables[0].Rows)
                    {
                        cb.Items.Add(dr.ItemArray[1].ToString());
                    }
                    cb.SelectedIndex = index;
                }
            }
        }
    }
}
