using Highlands.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : UserControl
    {
        public event EventHandler LoggedIn = (o, s) => { };

        public LoginWindow()
        {
            InitializeComponent();
            Reset();
        }

        public void Reset()
        {
            this.Visibility = System.Windows.Visibility.Visible;
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

            this.Visibility = System.Windows.Visibility.Hidden;
            LoggedIn(this, new EventArgs());
        }
    }
}
