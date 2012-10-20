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
            entConfirmPassword.Password = "";
            UserViewModel.Reset();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = cmbUserName.Text;
            UserViewModel.ValidationEnum result;
            if (entConfirmPassword.Visibility == System.Windows.Visibility.Visible)
                result = UserViewModel.SetPassword(user, entPassword.Password, entConfirmPassword.Password);
            else
                result = UserViewModel.ValidateUser(user, entPassword.Password);

            if (result == UserViewModel.ValidationEnum.FailedUserNotFound)
            {
                MessageBox.Show("User " + user + " not found! Contact your adminstrator.");
                return;
            }
            else if (result == UserViewModel.ValidationEnum.FailedPassword)
            {
                MessageBox.Show("Password invalid! Contact your adminstrator.");
                return;
            }
            else if (result == UserViewModel.ValidationEnum.PasswordChanged)
            {
                MessageBox.Show("Password changed!");
            }
            else if (result == UserViewModel.ValidationEnum.PasswordsDoNotMatch)
            {
                MessageBox.Show("Passwords do not match! Retry.");
            }
            else if (result == UserViewModel.ValidationEnum.PasswordRequired)
            {
                MessageBox.Show("Empty passwords are not allowed.");
            }
            else if (result == UserViewModel.ValidationEnum.RequirePasswordReset)
            {
                MessageBox.Show("Password reset required!");
                entConfirmPassword.Visibility = Visibility.Visible;
                staConfirmPassword.Visibility = Visibility.Visible;
                return;
            }

            ViewUtils.WriteToConfig(cmbUserName);
            LoggedIn(this, new EventArgs());
        }
    }
}
