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
            ViewUtils.ReadFromConfig(cmbUserName);
            entPassword.Password = "";
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
            LoggedIn(this, new EventArgs());
        }
    }
}
