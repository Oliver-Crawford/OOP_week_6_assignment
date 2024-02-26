using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;

namespace OOP_week_6_assignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string dbName = "DB.db";
        public static string dbConnectionName = $"Data Source={dbName};";
        public static string tableName = "user";
        public string user = "";
        public bool isAdmin = false;
        public int userId;
        public DataTable Dt = new DataTable();
        public MainWindow()
        {
            if (!File.Exists(dbName))
            {
                try
                {
                    //program doesn't notice that the file has been created later if I just use File.Create().
                    using (FileStream fs = File.Create(dbName))
                    {

                    }
                    //create user table
                    using (SQLiteConnection conn = new SQLiteConnection(dbConnectionName))
                    {
                        SQLiteCommand cmd = conn.CreateCommand();
                        cmd.CommandText = $"create table {tableName} (id integer primary key autoincrement, username text, password text, first_name text, last_name text, telephone text, created_at text, modified_at text, admin boolean)";
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    //create default admin
                    using (SQLiteConnection conn = new SQLiteConnection(dbConnectionName))
                    {
                        conn.Open();
                        insertUser(conn, true);
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            InitializeComponent();
            

            
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            using(SQLiteConnection conn = new SQLiteConnection(dbConnectionName))
            {
                conn.Open();
                loginUser(conn);
                conn.Close();
            }
        }
        public void loginUser(SQLiteConnection conn)
        {
            user = "";
            isAdmin = false;
            string userLoginName = "";
            bool userLoginAdmin = false;
            int userIdTemp = -1;
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = $"select username, admin, id from {tableName} where username = '{txtBUsername.Text}' and password = '{txtBPassword.Text}';";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (userLoginName != "")
                            {
                                MessageBox.Show("Duplicate users");
                            }
                            userLoginName = reader.GetString(0);
                            userLoginAdmin = reader.GetBoolean(1);
                            userIdTemp = reader.GetInt32(2);
                        }
                    }
                }
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
            user = userLoginName;
            isAdmin = userLoginAdmin;
            userId = userIdTemp;
            if(user != "")
            {
                MessageBox.Show($"Welcome {user}\nAdmin status: {isAdmin}");
                changeView();
                txtBUsername.Text = "";
                txtBPassword.Text = "";

            } else
            {
                MessageBox.Show("Invalid username/password");
            }
            selectAll(conn);
            return;
        }
        public void insertUser(SQLiteConnection conn, bool defaultAdmin)
        {
            using(SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                if(defaultAdmin)
                {
                    cmd.CommandText = $"insert into {tableName} (username, password, first_name, last_name, telephone, created_at, modified_at, admin) values ('admin', 'abcd', 'default', 'admin', '5555555555', date(), date(), true);";
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = $"insert into {tableName} (username, password, first_name, last_name, telephone, created_at, modified_at, admin) values ('{txtBUsername.Text}', '{txtBPassword.Text}', '{txtBFirstName.Text}', '{txtBLastName.Text}', '{txtBTelephone.Text}', date(), date(), {chkBIsAdmin.IsChecked});";
                    cmd.ExecuteNonQuery();
                }
                
            }
        }
        public void selectAll(SQLiteConnection conn)
        {
            using(SQLiteCommand cmd = new SQLiteCommand(conn))
            {
                cmd.CommandText = $"select * from {tableName};";
                Dt.Clear();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {
                    da.Fill(Dt);
                    DataGrid0.ItemsSource = Dt.AsDataView();
                }
                


            }
        }
        public void changeView()
        {
            btnLogin.Visibility = Visibility.Hidden;
            DataGrid0.Visibility = Visibility.Visible;
            chkBIsAdmin.Visibility = Visibility.Visible;
            labelIsAdmin.Visibility = Visibility.Visible;
            txtBTelephone.Visibility = Visibility.Visible;
            labelTelephone.Visibility = Visibility.Visible;
            txtBLastName.Visibility = Visibility.Visible;
            labelLastName.Visibility = Visibility.Visible;
            txtBFirstName.Visibility = Visibility.Visible;
            labelFirstName.Visibility = Visibility.Visible;
            txtBId.Visibility = Visibility.Visible;
            labelId.Visibility = Visibility.Visible;
            labelById.Visibility = Visibility.Visible;
            btnCreate.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Visible;
            labelNoId.Visibility = Visibility.Visible;

        }
        private void txtBUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (isAdmin)
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbConnectionName))
                {
                    conn.Open();
                    insertUser(conn, false);
                    selectAll(conn);
                    conn.Close();
                }
            } else
            {
                MessageBox.Show("You are not an Admin!");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (isAdmin || (isAdmin == chkBIsAdmin.IsChecked && userId == int.Parse(txtBId.Text)))
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbConnectionName))
                {
                    conn.Open();
                    using(SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = $"update {tableName} set username = '{txtBUsername.Text}', password = '{txtBPassword.Text}', first_name = '{txtBFirstName.Text}', last_name = '{txtBLastName.Text}', telephone = '{txtBTelephone.Text}', modified_at = date(), admin = {chkBIsAdmin.IsChecked} where id = {txtBId.Text};";
                        cmd.ExecuteNonQuery();

                    }
                    selectAll(conn);
                    conn.Close();
                }

            }
            else
            {
                MessageBox.Show("You are not an Admin!");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (isAdmin)
            {
                using (SQLiteConnection conn = new SQLiteConnection(dbConnectionName))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = $"delete from {tableName} where id = {txtBId.Text}";
                        cmd.ExecuteNonQuery();

                    }
                    selectAll(conn);
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("You are not an Admin!");
            }
        }
    }
}