using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlashPicOrganizer2
{
    /// <summary>
    /// Logika interakcji dla klasy TagDescWindow.xaml
    /// </summary>
    public partial class TagDescWindow : Window
    {
        public TagDescWindow()
        {
            InitializeComponent();
           
        }
        public TagDescWindow(string txt)
        {
            InitializeComponent();
            TagTextBox.Text = txt;
        }

        private void Button_Click(object sender, RoutedEventArgs e)//ok
        {
            if (TagTextBox.Text == "Opisz Tag")
                TagTextBox.Text = "";
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)//cancel
        {
            if (TagTextBox.Text == "Opisz Tag")
                TagTextBox.Text = "";
            this.Close();
        }
    }
}
