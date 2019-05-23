using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

namespace SqlLite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        SQLiteConnection con = new SQLiteConnection($"Data Source={"Users.sqlite"}");



        private void FillDataGrid()
        {
            con.Open();

            string q = "SELECT * From tblUsers";
            DataSet dataSet = new DataSet();
            SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(q, con);
            dataAdapter.Fill(dataSet);
            dgViewDB.ItemsSource = dataSet.Tables[0].DefaultView;
            con.Close();

        }
        private void BtnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (txtLogin.Text != "" && txtPassword.Password != "")
            {
                con.Open();
                string Login = txtLogin.Text;
                string pass = txtPassword.Password;
                string hashed = BCrypt.HashPassword(pass, BCrypt.GenerateSalt(12));

                string query = $"Insert into tblUsers(Login,Password) values('{Login}','{ hashed}')";
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
                FillDataGrid();


            }
            else
                MessageBox.Show("Fill in all the fields");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView d = dgViewDB.SelectedItem as DataRowView;

                SQLiteCommand cmd;
                con.Open();

                string query = $"Delete FROM tblUsers where Login='{d["Login"].ToString()}'";
                cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
                FillDataGrid();
            }
            catch (Exception ex)
            {
            }
        }

        private void DgViewDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView d = dgViewDB.SelectedItem as DataRowView;
                txtLogin.Text = d["Login"].ToString();
                txtPassword.Password = d["Password"].ToString();
            }
            catch { }
        }
        private void BtnSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string Login = txtLogin.Text;

                string q = $"SELECT* FROM tblUsers WHERE Login = '{Login}'";
                con.Open();
                SQLiteCommand cmd;

                cmd = new SQLiteCommand(q, con);
                SQLiteDataReader reader = cmd.ExecuteReader();

                string pas = "";
                if (reader.Read())
                    pas = reader["Password"].ToString();
                bool matches = BCrypt.CheckPassword(txtPassword.Password, pas);
                if (matches)
                    MessageBox.Show("OK");
                else
                    MessageBox.Show("wrong password or login");
            }
            catch
            {
                MessageBox.Show("user does not exist");

            }




            con.Close();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();

        }



        private void DgViewDB_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

            //   string query = $"Insert into tblUsers(Login,Password) values('{Login}','{ hashed}')";
            //  SQLiteCommand cmd = new SQLiteCommand(query, con);
            //  cmd.ExecuteNonQuery();

            // FillDataGrid();

        }

        private void DgViewDB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void DgViewDB_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // MessageBox.Show(e.NewValue.ToString());

        }

        private void DgViewDB_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {


            if (dgViewDB.SelectedItems.Count > 0)
            {
                //DataRowView drv = (DataRowView)dgViewDB.SelectedItem;
                //string id = drv.Row[0].ToString();

                //con.Open();
                //SQLiteCommand comm = new SQLiteCommand($"update tblUsers set Login={txtLogin.Text} where Id = @Id", con);
                //comm.Parameters.AddWithValue("@id", id);
                //comm.Parameters.AddWithValue("@Login", e.EditingElement);
                //comm.Parameters.AddWithValue("@Password", txtPassword.Password);

                //comm.ExecuteNonQuery();
                //con.Close();

                //FillDataGrid();
                //try
                //{

                //    con.Open();
                //    SQLiteCommand scmd = new SQLiteCommand("SELECT * FROM tblUsers");
                //  //  dgViewDB.CommitEdit();

                //    SQLiteDataAdapter sda = new SQLiteDataAdapter(scmd);
                //    sda.UpdateCommand = new SQLiteCommand("SELECT * FROM tblUsers");
                //  //  sda.Update(((DataView)dgViewDB.ItemsSource).Table);
                //    DataTable dt = new DataTable("tblUsers");
                //    SQLiteCommandBuilder builder = new SQLiteCommandBuilder(sda);
                //    sda.UpdateCommand = builder.GetUpdateCommand();
                //    sda.Update(dt);
                //    con.Close();
                //    MessageBox.Show("New entry is updated successfully");
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("You can't update empty row" + ex.Message);
                //}
            }
        }
    }
}
