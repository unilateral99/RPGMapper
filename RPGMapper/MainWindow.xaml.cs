using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPGMapper
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        internal Grade grade {  get; set; }

        public MainWindow()
        {
            InitializeComponent();

            grade = new Grade();

            gradeIC.ItemsSource = grade.Tiles;
            
            this.DataContext = grade;
        }

        public void PreviewMouseWheel_ScrollViewer(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (e.Delta > 0)
                {
                    grade.Zoom += 0.1;
                }
                else if (e.Delta < 0)
                {
                    grade.Zoom -= 0.1;
                }
            }
        }
    }
}


