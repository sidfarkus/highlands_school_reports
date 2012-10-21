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
using Highlands.ViewModel;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for EditGradeWindow.xaml
    /// </summary>
    public partial class EditGradeWindow : Window
    {
        public EditGradeWindow(string studentName, GradeViewModel grade)
        {
            InitializeComponent();
            gradeEditor.LoadGrade(studentName, grade);
        }

        private void GradeSaved(object sender, EventArgs e)
        {
            Close();
        }

        private void EditCanceled(object sender, EventArgs e)
        {
            Close();
        }
    }
}
