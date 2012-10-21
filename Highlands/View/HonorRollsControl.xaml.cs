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
    /// Interaction logic for HonorRollsControl.xaml
    /// </summary>
    public partial class HonorRollsControl : UserControl
    {
        private HonorRollsViewModel model;

        public HonorRollsControl()
        {
            InitializeComponent();
            model = new HonorRollsViewModel();
            this.DataContext = model;
        }
    }
}
