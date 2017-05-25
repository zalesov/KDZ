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
    /// Логика взаимодействия для UpdProduct.xaml
    /// </summary>
    public partial class UpdProduct : Window
    {
         String connectionString;
        string sql;
        SqlDataAdapter adapter;
        DataSet dataSet;
        int vid;
        public bool isAct = false;//показатель, который показывает было ли совершено добавление/редактирование/удаление данных о продукте
        public UpdProduct(int vid,DataSet ds)
            //vid - режим работы окна 0-добавление 1-редактирование 2 -удаление
            //ds - данные о продуктах из БД
        {
            this.vid = vid;
            dataSet=ds;
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            InitializeComponent();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products.Items.Add(dr.ItemArray[1].ToString());
            }
            if(vid==0)
            {
                DP1.Visibility = Visibility.Collapsed;
                OK.Content = "Добавить";
                window.Title = "Добавить продукт";
                window.Height = 110;
            }
            if(vid==2)
            {
                DP2.Visibility = Visibility.Collapsed;
                OK.Content = "Удалить";
                window.Title = "Удалить продукт";
                window.Height = 110;
            }

        }

        private void OK_Click(object sender, RoutedEventArgs e)
            //действие добавление/редактирования/удаления в зависимости от вида
        {

            if(vid==0)
            {
                if(NewName.Text!="")
                    sql = "INSERT INTO Products(Name) " +
                       "VALUES (N\'" + NewName.Text + "\')";
                else
                {
                    MessageBox.Show("Не введено название");
                    return;
                }
            }
            if(vid==1)
            {
                if (Products.SelectedIndex > -1 && NewName.Text != "")
                    sql = "UPDATE Products Set Name=N\'" + NewName.Text + "\' WHERE Id_products=" + dataSet.Tables[0].Rows[Products.SelectedIndex].ItemArray[0];
                else
                {
                    MessageBox.Show("Не указаны все параметры");
                    return;
                }
            }
            if(vid==2)
            {
                if (Products.SelectedIndex>-1)
                    sql = "DELETE FROM Products " +
                       "WHERE Id_products="+dataSet.Tables[0].Rows[Products.SelectedIndex].ItemArray[0];
                else
                {
                    MessageBox.Show("Не выбрано название");
                    return;
                }
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
            isAct = true;
            this.Close();
        }

        private void Cansel_Click(object sender, RoutedEventArgs e)
            //Отмена действия и закрытие окна
        {
            this.Close();
        }
    }
}
