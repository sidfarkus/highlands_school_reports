using Highlands.StaticModel;
using Highlands.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for EditGrade.xaml
    /// </summary>
    public partial class EditGrade : Window
    {
        private string _studentName;
        private GradeViewModel _grade;

        public EditGrade(string studentName, GradeViewModel grade)
        {
            _studentName = studentName;
            _grade = grade;
            
            InitializeComponent();

            if (CourseViewModel.HasSpecialGrade(grade.Subject))
            {
                entSpecialGrade.Visibility = System.Windows.Visibility.Visible;
            }

            Maintenance.LetterGrades.ToList().ForEach(g => cmbLetterGrade.Items.Add(g));

            Maintenance.Comments.ToList().ForEach(g => cmbComment.Items.Add(g));
            cmbComment.SelectedIndex = 0;

            Title = _studentName;
            staCourse.Content = _grade.Subject + " for " + _grade.Quarter + " " + _grade.Group + " with " + _grade.Teacher;
            cmbLetterGrade.Text = _grade.LetterGrade;
            entSpecialGrade.Text = _grade.SpecialGrade;
            
            var comment = _grade.Comment;
            foreach (var c in Maintenance.Comments)
            {
                var formattedComment = Maintenance.FormatCommentFromList(c);
                if (string.IsNullOrWhiteSpace(formattedComment))
                    continue;
                if (comment.StartsWith(formattedComment))
                {
                    cmbComment.Text = c;
                    comment = comment.Substring(formattedComment.Length).Trim();
                    break;
                }
            }
            entComment.Text = comment;
        }

        private void cmbComment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count != 1)
                return;
            var text = e.AddedItems[0] as string;
            text = Maintenance.FormatCommentFromList(text);

            if (string.IsNullOrWhiteSpace(text))
                staComment.Text = entComment.Text;
            else
                staComment.Text = text + " " + entComment.Text;
        }

        private void entComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            staComment.Text = Maintenance.FormatCommentFromList(cmbComment.Text) + " " + entComment.Text;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var diffs = new List<Change>();

            // TODO persist "other" grade
            // TODO persist semester flag if present

            if (_grade.Comment != staComment.Text)
                diffs.Add(new Change(_grade, "Comment", _grade.Comment, staComment.Text));
            if (_grade.LetterGrade != cmbLetterGrade.Text)
                diffs.Add(new Change(_grade, "Grade", _grade.LetterGrade, cmbLetterGrade.Text));
            
            if (diffs.Count() == 0)
            {
                Close();
                return;
            }
 
            var result = MessageBox.Show(diffs.Count().ToString() + " changes!\n" + Change.FormatDiffs(diffs) + Environment.NewLine + "Save?", "Confirm Save", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            else if (result == MessageBoxResult.No)
            {
                Close();
                return;
            }
            _grade.Comment = staComment.Text;
            _grade.LetterGrade = cmbLetterGrade.Text;
            _grade.Save(diffs);
            Close();
        }

        private void btnFreeForm_Click(object sender, RoutedEventArgs e)
        {
            var comment = staComment.Text;
            cmbComment.SelectedIndex = 0;
            entComment.Text = comment;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
            return;
        }

        private void radSemester_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
