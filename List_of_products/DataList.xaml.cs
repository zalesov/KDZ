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
    /// Логика взаимодействия для DataList.xaml
    /// </summary>
    public partial class DataList : Window
    {
        String connectionString;
        string sql;
        SqlDataAdapter adapter;
        DataSet dataSet,dataSetUsers,dataSetProduct;
        bool obvDG = true;//показатель нужно ли обноввлять список

        public DataList()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            InitializeComponent();
            zapCB();
        }

        private void zapCB()
            //заполнение комбо боксов
        {
            obvDG = false;
            ListUsers.Items.Clear();
            ListProduct.Items.Clear();
            ListUsers.Items.Add("Вывести всех");
            ListProduct.Items.Add("Вывести всё");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                sql = "SELECT * From Customers";
                adapter = new SqlDataAdapter(sql, connection);
                dataSetUsers = new DataSet();
                adapter.Fill(dataSetUsers);
                foreach (DataRow dr in dataSetUsers.Tables[0].Rows)
                {
                    ListUsers.Items.Add(dr.ItemArray[1].ToString());
                }
                sql = "SELECT * From Products";
                adapter = new SqlDataAdapter(sql, connection);
                dataSetProduct = new DataSet();
                adapter.Fill(dataSetProduct);
                foreach (DataRow dr in dataSetProduct.Tables[0].Rows)
                {
                    ListProduct.Items.Add(dr.ItemArray[1].ToString());
                }
            }
            ListUsers.SelectedIndex = 0;
            ListProduct.SelectedIndex = 0;
            obvDG = true;
            zapDG();

        }
        private void zapDG()
            //заполнение дата грид
        {
            if(obvDG)
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    if (ListUsers.SelectedIndex == 0 && ListProduct.SelectedIndex == 0)
                    {
                        sql = " SELECT c.Customers_id,c.Name \'Покупатель\',p.Id_products,p.Name \'Товар\',SUM(pc.Count_prod) \'Количество купленного\' " +
                            " FROM Customers c, Products p,Prod_Cust pc " +
                            " Where c.Customers_id=pc.Customers_id AND p.Id_products=pc.Id_products " +
                            " Group by p.Name,p.Id_products,c.Name,c.Customers_id ";
                       
                    }
                    else
                    {
                        if(ListUsers.SelectedIndex == 0 || ListProduct.SelectedIndex == 0)
                        {
                            if (ListUsers.SelectedIndex != 0)
                            {
                                sql = " SELECT c.Customers_id,c.Name \'Покупатель\',p.Id_products,p.Name \'Товар\',SUM(pc.Count_prod) \'Количество купленного\' " +
                           " FROM Customers c, Products p,Prod_Cust pc " +
                           " Where c.Customers_id=pc.Customers_id AND p.Id_products=pc.Id_products AND c.Customers_id=" + dataSetUsers.Tables[0].Rows[ListUsers.SelectedIndex - 1].ItemArray[0]  +
                           " Group by p.Name,p.Id_products,c.Name,c.Customers_id ";
                            }
                            else
                            {
                                sql = " SELECT c.Customers_id,c.Name \'Покупатель\',p.Id_products,p.Name \'Товар\',SUM(pc.Count_prod) \'Количество купленного\' " +
                           " FROM Customers c, Products p,Prod_Cust pc " +
                           " Where c.Customers_id=pc.Customers_id AND p.Id_products=pc.Id_products AND p.Id_products=" + dataSetProduct.Tables[0].Rows[ListProduct.SelectedIndex - 1].ItemArray[0] +
                           " Group by p.Name,p.Id_products,c.Name,c.Customers_id ";
                            }
                        }
                        else
                        {
                            sql = " SELECT c.Customers_id,c.Name \'Покупатель\',p.Id_products,p.Name \'Товар\',SUM(pc.Count_prod) \'Количество купленного\' " +
                           " FROM Customers c, Products p,Prod_Cust pc " +
                           " Where c.Customers_id=pc.Customers_id AND p.Id_products=pc.Id_products AND c.Customers_id=" + dataSetUsers.Tables[0].Rows[ListUsers.SelectedIndex - 1].ItemArray[0] + "AND p.Id_products=" + dataSetProduct.Tables[0].Rows[ListProduct.SelectedIndex - 1].ItemArray[0] +
                           " Group by p.Name,p.Id_products,c.Name,c.Customers_id ";
                        }
                    }
                         adapter = new SqlDataAdapter(sql, connection);
                        dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        DG.ItemsSource = dataSet.Tables[0].DefaultView;
                }
        }
        private void DG_SelectionChanged(object sender, SelectionChangedEventArgs e)
            //выделение кнопки удалить запись, если выбрана строчка в списке данных
        {
            if (DG.SelectedIndex > -1)
                Delete.Visibility = Visibility.Visible;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
            //добавление покупателя
        {
            AddCustomers addCustomers = new AddCustomers();
            addCustomers.ShowDialog();

            zapCB();
            if (addCustomers.isAdd)
                zapDG();

        }
        private void Delete_Click(object sender, RoutedEventArgs e)
            //удаление выбранной в данных записи
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataRowView drv = (DataRowView)DG.SelectedItems[0];
                if(connection.State!=ConnectionState.Open)
                    connection.Open();
                sql = "Delete from Prod_Cust  Where Id_products=" + drv.Row.ItemArray[2]+
                    " AND Customers_id=" + drv.Row.ItemArray[0];
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
            zapDG();
        }
        private void Window_Closed(object sender, EventArgs e)
            //действия при закрытии окна
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void DG_AutoGeneratedColumns(object sender, EventArgs e)
            //скрытие колонок с id в списке данных
        {
            DG.Columns[0].Visibility = Visibility.Hidden;
            DG.Columns[2].Visibility = Visibility.Hidden;
        }

        private void ListProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
            //обновление данных при смене выбора выводимого продукта
        {
            zapDG();    
        }

        private void ListUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //обновление данных при смене выбора выводимого пользователя
        {
            zapDG();  
        }

    }
}
