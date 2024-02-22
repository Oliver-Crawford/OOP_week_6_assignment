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

namespace OOP_week_6_assignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string db = "DB.db";
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(db))
            {

            }
            
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window2 = new MainWindow();
            window2.Show();
        }
    }
}