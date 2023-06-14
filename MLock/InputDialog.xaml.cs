using System;
using System.Windows;

namespace MLock
{ 
    public partial class InputDialog
    {
        public InputDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            Title = question;
            LblQuestion.Content = question;
            TxtAnswer.Text = defaultAnswer;
        }

        public string Answer => TxtAnswer.Text;

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TxtAnswer.SelectAll();
            TxtAnswer.Focus();
        }
    }
}