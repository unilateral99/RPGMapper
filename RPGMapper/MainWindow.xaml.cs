using Microsoft.Win32;
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

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Imagem (*.png, *.jpg)|*.png;*.jpg|Todos os arquivos (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                if (grade != null)
                {
                    MessageBoxResult confirmacao = MessageBox.Show("Descartar mapa atual?", "Novo arquivo", MessageBoxButton.YesNo);
                    switch (confirmacao)
                    {
                        case MessageBoxResult.Yes:
                            break;
                        case MessageBoxResult.No:
                            return;
                    }
                }
                else
                {
                    grade = new Grade(50, 50, openFileDialog.FileName);

                    gradeIC.ItemsSource = grade.Tiles;

                    this.DataContext = grade;
                }
            }
        }
    }
}


