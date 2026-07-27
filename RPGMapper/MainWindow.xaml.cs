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
        internal Grade grade;
        internal Entidades temp;
        internal ObservableCollection<Entidades> ListaEntidades;
        internal AdornerLayer layer;
        internal AdornerArrastar adorner;
        internal Image imagemArrastar;
        internal Tile tileOrigem;
        internal Tile tileDestino;

        public MainWindow()
        {
            InitializeComponent();
            ListaEntidades = new ObservableCollection<Entidades>();
        }

        // Evento para o zoom
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

        // Evento para detectar click em uma tile especifica e começar a mover caso houver uma entidade
        private void Tile_Click(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var tile = grid.DataContext as Tile;

            // MessageBox.Show($"{tile.x}, {tile.y}");

            if (temp != null)
            {
                ListaEntidades.Add(temp);
                tile.AdicionarEntidade(temp);

                temp = null;
                Mouse.OverrideCursor = null;
                return;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                tileOrigem = null;
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                layer = AdornerLayer.GetAdornerLayer(MainGrid);
                DependencyObject obj = VisualTreeHelper.GetChild(grid, 1);
                imagemArrastar = obj as Image;
                imagemArrastar.CaptureMouse();

                adorner = new AdornerArrastar(imagemArrastar);

                layer.Add(adorner);

                tileOrigem = tile;
            }
        }

        // Move a imagem com o Mouse
        private void EntidadeImagem_MouseMove(object sender, MouseEventArgs e)
        {
            if (adorner != null)
            {
                Point p = Mouse.GetPosition(adorner.AdornedElement);

                adorner.LeftOffset = p.X - imagemArrastar.ActualWidth / 2;
                adorner.TopOffset = p.Y - imagemArrastar.ActualHeight / 2;

                adorner.InvalidateVisual();
            }        
        }

        // Solta a imagem quando o mouse for solto
        private void EntidadeImagem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (adorner != null)
            {
                layer.Remove(adorner);

                adorner = null;
            }
            if (imagemArrastar  != null)
            {
                imagemArrastar.ReleaseMouseCapture();
            }
            if (tileOrigem != null)
            {
                // Checa o objeto na posição do mouse até achar a grid

                try
                {
                    Point p = Mouse.GetPosition(MainGrid);
                    HitTestResult result = VisualTreeHelper.HitTest(MainGrid, p);
                    DependencyObject obj = result.VisualHit;
                    while (obj != null && obj.GetType() != typeof(Grid))
                    {
                        obj = VisualTreeHelper.GetParent(obj);
                    }

                    Grid gradeTile = obj as Grid;
                    tileDestino = gradeTile.DataContext as Tile;

                    TrocarTiles(tileOrigem, tileDestino);
                    tileOrigem = null;
                    tileDestino = null;
                } catch 
                {
                    return;
                }
            }
        }

        // Função para fazer a troca de entidades entre duas tiles
        private void TrocarTiles(Tile start, Tile end)
        {
            if (end.Entidade != start.Entidade)
            {
                end.Entidade = start.Entidade;
                start.Entidade = null;
            }
        }

        // Botão novo
        private void Novo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Imagem (*.png, *.jpg)|*.png;*.jpg|Todos os arquivos (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                if (grade != null)
                {
                    MessageBoxResult confirmacao = MessageBox.Show("Descartar mapa atual?", "Novo", MessageBoxButton.YesNo);
                    switch (confirmacao)
                    {
                        case MessageBoxResult.Yes:
                            break;
                        case MessageBoxResult.No:
                            return;
                    }
                }
                grade = new Grade(50, 50, openFileDialog.FileName);

                gradeIC.ItemsSource = grade.Tiles;

                this.DataContext = grade;
            }
        }

        private void Novo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Adicionar Inimigos
        private void Inimigo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (temp?.GetType() == typeof(Inimigo))
            {
                Mouse.OverrideCursor = null;
                temp = null;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;

                Inimigo inimigo = new Inimigo();

                temp = inimigo;
            }
        }

        private void Inimigo_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            if (grade != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }
    }
}


