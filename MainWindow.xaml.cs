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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace FlashPicOrganizer2
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        /// <zmienne globalne klasy>
        ImageBrush ib = new ImageBrush();
        BitmapImage img;
        OpenFileDialog ofdPict = new OpenFileDialog();
        static double val = 1.0;
        System.Windows.Point point;
        bool menuslidermouseevent = false;
        System.Windows.Point ScaleMousePos;
        double menusliderx, menuslidery;
        bool activemouse;
        bool DrawTagEn = false;
        bool DeleteTagEn = false;
        Shape shape;
        /////////////////////////KLASY
        Tag tagelement = new Tag();
        Obrazek elementobrazek = new Obrazek();
        Kolekcja elementkolekcja = new Kolekcja();
        /////////////////////////////
        /// </summary globalne klasy>

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas1.Height = this.ActualHeight-22;
            canvas1.Width = this.ActualWidth;
            MainMenu.Width = this.ActualWidth;
        }

        private void Canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (activemouse)
            {
                translate.X += e.GetPosition(canvas1).X - point.X;
                translate.Y += e.GetPosition(canvas1).Y - point.Y;
            }
            ScaleMousePos.X = e.GetPosition(canvas1).X;
            ScaleMousePos.Y = e.GetPosition(canvas1).Y;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (DrawTagEn == true)
                {
                    
                    if (shape != null)
                    {
                        shape.Width = IsNonNegative(e.GetPosition(canvas1).X- tagelement.TagPoint.X);
                        shape.Height = IsNonNegative(e.GetPosition(canvas1).Y- tagelement.TagPoint.Y);
                    }
                }

            }
        }

        private void Canvas1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            menuslidermouseevent = true;
            menuslider1.Value += e.Delta / 100;
        }

        private void MenuItem_Click_Zamknij(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Click_Importuj_Obraz(object sender, RoutedEventArgs e)
        {
            canvas1.Height = this.ActualHeight;
            canvas1.Width = this.ActualWidth;
            ofdPict.Filter= "Image files*.bmp; *.jpg; *.jpeg; *.png; *.gif; *.tif; *.tiff)| *.bmp; *.jpg; *.jpeg; *.png; *.gif; *.tif; *.tiff | All files | *.* ";
            ofdPict.FilterIndex = 1;
            if (ofdPict.ShowDialog() == true)
            {
                img = new BitmapImage(new Uri(ofdPict.FileName));
                ib.ImageSource = img;
                canvas1.Background = ib;
            }
        }

        private void DrawTagEnButtonChecked(object sender, RoutedEventArgs e)
        {
            DrawTagEn = true;
            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            for (int i = 0; i < elementobrazek.taglist.Count; i++)
            {
                shape = new Rectangle();
                shape.Stroke = Brushes.Red;
                shape.Fill = scb;
                Canvas.SetLeft(shape, elementobrazek.taglist[i].TagPoint.X);
                Canvas.SetTop(shape, elementobrazek.taglist[i].TagPoint.Y);
                shape.Width = elementobrazek.taglist[i].TagWidth;
                shape.Height = elementobrazek.taglist[i].TagHeigth;
                try
                {
                    canvas1.Children.Add(shape);
                }
                catch
                {
                    MessageBox.Show("Wykryto pusty tag.", "Błąd wewnętrzny", MessageBoxButton.OK);
                }
            }
        }

        private void DrawTagEnButtonUnChecked(object sender, RoutedEventArgs e)
        {
            DrawTagEn = false;
            canvas1.Children.RemoveRange(0, canvas1.Children.Count);
        }

        private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                canvas1.CaptureMouse();
                point.X = e.GetPosition(canvas1).X;
                point.Y = e.GetPosition(canvas1).Y;
                activemouse = true;
            }
        }
        
        private void canvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                canvas1.ReleaseMouseCapture();
                activemouse = false;
            }
        }

        private void Menuslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            val = menuslider1.Value;
            if (st != null)
            {
                if (menuslidermouseevent == true)
                {
                    if (st.ScaleX == 1.0)
                    {
                        st.CenterX = ScaleMousePos.X;
                        st.CenterY = ScaleMousePos.Y;
                        menusliderx = st.CenterX;
                        menuslidery = st.CenterY;
                    }
                    else
                    {
                        st.CenterX = menusliderx;
                        st.CenterY = menuslidery;
                    }
                }
                else
                {
                    st.CenterX = canvas1.Width / 2;
                    st.CenterY = canvas1.Height / 2;
                }
                st.ScaleX = val;
                st.ScaleY = val;
                menuslidermouseevent = false;
            }
        }

        private void Canvas1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DeleteTagEn == true)
            {
                Point pt = e.GetPosition((Canvas)sender);
                HitTestResult res = VisualTreeHelper.HitTest(canvas1, pt);
                if (res != null)
                {
                    for (int i = 0; i < elementobrazek.taglist.Count; i++)
                    {
                        if (elementobrazek.taglist[i].TagPoint.X <= pt.X && elementobrazek.taglist[i].TagPoint.X + elementobrazek.taglist[i].TagWidth >= pt.X)
                        {
                            if (elementobrazek.taglist[i].TagPoint.Y <= pt.Y && elementobrazek.taglist[i].TagPoint.Y + elementobrazek.taglist[i].TagHeigth >= pt.Y)
                            {
                                elementobrazek.taglist.RemoveAt(i);
                            }
                        }
                    }
                   
                    canvas1.Children.Remove(res.VisualHit as Shape);
                }
            }
            else
            {
                
                Point pt = e.GetPosition((Canvas)sender);
                for (int i = 0; i < elementobrazek.taglist.Count; i++)
                {
                    if (elementobrazek.taglist[i].TagPoint.X <= pt.X && elementobrazek.taglist[i].TagPoint.X + elementobrazek.taglist[i].TagWidth >= pt.X)
                    {
                        if (elementobrazek.taglist[i].TagPoint.Y <= pt.Y && elementobrazek.taglist[i].TagPoint.Y + elementobrazek.taglist[i].TagHeigth >= pt.Y)
                        {
                            TagDescWindow tdw = new TagDescWindow(elementobrazek.taglist[i].OpisTagu);
                            tdw.ShowDialog();
                            elementobrazek.taglist[i].OpisTagu = tdw.TagTextBox.Text;
                        }
                    }
                }
                
            }
        }

        private void Canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            canvas1.CaptureMouse();
            if (DrawTagEn == true)
            {
                tagelement = new Tag();
                shape = new Rectangle();
                shape.Width = 1;
                shape.Height = 1;
                Canvas.SetLeft(shape, e.GetPosition(canvas1).X);
                Canvas.SetTop(shape, e.GetPosition(canvas1).Y);
                tagelement.TagPoint.X = e.GetPosition(canvas1).X;
                tagelement.TagPoint.Y = e.GetPosition(canvas1).Y;
                shape.Stroke = Brushes.Red;
                shape.Fill = scb;
                canvas1.Children.Add(shape);
            }
        }

        private void Canvas1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DrawTagEn == true)
            {
                TagDescWindow tdw = new TagDescWindow();
                tdw.ShowDialog();
                tagelement.OpisTagu = tdw.TagTextBox.Text;
                tagelement.TagHeigth = shape.Height;
                tagelement.TagWidth = shape.Width;
                elementobrazek.taglist.Add(tagelement);
            }
        }

        private void DeleteTagChecked(object sender, RoutedEventArgs e)
        {
            DeleteTagEn = true;
        }

        private void DeleteTagUnChecked(object sender, RoutedEventArgs e)
        {
            DeleteTagEn = false;
        }

        private void FlashPicOrganizerMW_KeyDown_1(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.D)
            {
                deltagcheckbox.IsChecked = true;
            }
        }

        private void FlashPicOrganizerMW_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                deltagcheckbox.IsChecked = false;
            }
        }

        private double IsNonNegative(double a)
        {
            if (a >= 0)
                return a;
            else
                return 0;
        }

    }
}
