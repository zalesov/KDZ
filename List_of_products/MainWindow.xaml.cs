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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace List_of_products
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String connectionString;//сторока подключения к БД
        string sql;//переменная для хранения запроса
        SqlDataAdapter adapter;
        DataSet ds;//переменная для хранения данных полученных из БД
        public MainWindow()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            InitializeComponent();
        }

        private void Avt_Click(object sender, RoutedEventArgs e)
        //функция авторизации
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                sql = "SELECT u.Password " +
                "FROM Users u " +
                "Where u.Name like\'" + TB_login.Text + "\'";
                adapter = new SqlDataAdapter(sql, connection);
                ds = new DataSet();
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                    MessageBox.Show( "*пароль или логин не верны");
                else
                    if (String.Compare(TB_password.Password.ToString(), ds.Tables[0].Rows[0].ItemArray[0].ToString(), false) == 0)
                    {

                        
                        DataList datalist = new DataList();
                        datalist.Show();
                        this.Close();
                    }
                    else
                        MessageBox.Show( "Пароль или логин не верны");
            }
        }
    }
}
