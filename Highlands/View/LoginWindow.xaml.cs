using Highlands.ViewModel;
using System.Windows;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            ViewUtils.ReadFromConfig(cmbUserName);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var teacher = cmbUserName.Text;
            if (!UserViewModel.ValidateTeacher(teacher, entPassword.Password))
            {
                MessageBox.Show("User " + teacher + " could not be validated! Contact your adminstrator.");
                return;
            }

            ViewUtils.WriteToConfig(cmbUserName);
            
            DialogResult = true;
            Close();
        }
    }
}
