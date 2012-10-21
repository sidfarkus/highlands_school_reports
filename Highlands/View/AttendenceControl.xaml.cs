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
using Highlands.ViewModel;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for AttendeesControl.xaml
    /// </summary>
    public partial class AttendeesControl : UserControl
    {
        private AttendenceViewModel model;

        public AttendeesControl()
        {
            InitializeComponent();
            model = new AttendenceViewModel();
            this.DataContext = model;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            model.Save();
        }
        public void Refresh()
        {
            colStatus.IsReadOnly = !UserViewModel.CurrentUser.CanEditAttendance;
            btnSave.IsEnabled = UserViewModel.CurrentUser.CanEditAttendance;
        }
    }
}
