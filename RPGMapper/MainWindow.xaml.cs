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
using System.Windows.Threading;

namespace RPGMapper
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public Entidades temp;
        internal AdornerLayer layer;
        internal AdornerArrastar adorner;
        internal Image imagemArrastar;
        internal Tile tileOrigem;
        internal Tile tileDestino;

        private DispatcherTimer timer;

        public Grade grade {  get; set; }
        public ObservableCollection<Grade> grades { get; set; } = new ObservableCollection<Grade>();

        // Listas para as diversas entidades
        public ObservableCollection<Inimigo> Inimigos { get; set; }
        public Inimigo inimigoSelecionado { get; set; }

        public ObservableCollection<Jogador> Jogadores { get; set; }
        public Jogador jogadorSelecionado { get; set; }

        public ObservableCollection<Npc> Npcs { get; set; }
        public Npc npcSelecionado { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += Timer_Tick;

            tabControlMaps.DataContext = this;

            CbbInimigo.DataContext = this;
            Inimigos = new ObservableCollection<Inimigo>();
            Inimigo inimigo = new Inimigo(System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\espada.png", "Inimigos");
            Inimigos.Add(inimigo);
            CbbInimigo.SelectedIndex = 0;

            CbbJogador.DataContext = this;
            Jogadores = new ObservableCollection<Jogador>();
            Jogador jogador = new Jogador(System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\controle.png", "Jogadores");
            Jogadores.Add(jogador);
            CbbJogador.SelectedIndex = 0;

            CbbNpc.DataContext = this;
            Npcs = new ObservableCollection<Npc>();
            Npc npc = new Npc(System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\homem.png", "Npcs");
            Npcs.Add(npc);
            CbbNpc.SelectedIndex = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
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

            timer.Start();

            if (tile.Entidade != null)
            {
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

            if (CbbInimigo.SelectedIndex != 0)
            {
                Inimigo inimigo = inimigoSelecionado.Clone();
                tile.AdicionarEntidade(inimigo);
            }
            if (CbbJogador.SelectedIndex != 0)
            {
                tile.AdicionarEntidade(jogadorSelecionado);
            }
            if (CbbNpc.SelectedIndex != 0)
            {
                tile.AdicionarEntidade(npcSelecionado);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                tileOrigem = null;
                return;
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
            try
            {
                // Checa o objeto na posição do mouse até achar a grid
                Point p = Mouse.GetPosition(MainGrid);
                HitTestResult result = VisualTreeHelper.HitTest(MainGrid, p);
                DependencyObject obj = result.VisualHit;
                while (obj != null && obj.GetType() != typeof(Grid))
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }

                Grid gradeTile = obj as Grid;
                tileDestino = gradeTile.DataContext as Tile;

                // Se clicado ou segurado
                if (timer.IsEnabled)
                {
                    if (tileOrigem != null && tileOrigem.Entidade.GetType() == typeof(Inimigo))
                    {
                        tileOrigem.Entidade.Localizacao = tileOrigem;
                        EnemyViewer.DataContext = tileOrigem.Entidade;
                        EnemyViewer.Visibility = Visibility.Visible;
                    }
                }
                // Se segurado
                if (adorner != null)
                {
                    layer.Remove(adorner);

                    adorner = null;
                }
                if (imagemArrastar != null)
                {
                    imagemArrastar.ReleaseMouseCapture();
                }
                if (tileOrigem != null)
                {
                    TrocarTiles(tileOrigem, tileDestino);
                    tileOrigem = null;
                    tileDestino = null;
                }
            }
            catch
            {
                return;
            }
            
        }

        // Função para trocar de mapas no tabControl
        private void tabControlMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControlMaps.Items.Count > 0)
            {
                grade = grades[tabControlMaps.SelectedIndex];
                MainGrid.DataContext = grade;
                gradeIC.ItemsSource = grade.Tiles;
            }
        }

        // Eventos dos botões do EnemyViewer
        private void FecharEnemyViewer(object sender, RoutedEventArgs e)
        {
            EnemyViewer.Visibility = Visibility.Collapsed;
        }

        private void DeletarInimigo(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            Inimigo inimigo = but.DataContext as Inimigo;
            EnemyViewer.Visibility = Visibility.Collapsed;
            inimigo.Localizacao.Entidade = null;
            inimigo.Localizacao = null;
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

        // Função para abrir uma imagem e retornar o camingo como String
        private string AbrirImagem()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Imagem (*.png, *.jpg)|*.png;*.jpg|Todos os arquivos (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            
            return null;
        }

        // Botão novo
        private void Novo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string FilePath = AbrirImagem();

            if (FilePath != null)
            {
                if (grade != null)
                {
                    MessageBoxResult confirmacao = MessageBox.Show("Descartar mapa atual?\nIsso irá descartar todos os mapas e entidades criados", "Novo", MessageBoxButton.YesNo);
                    switch (confirmacao)
                    {
                        case MessageBoxResult.Yes:
                            break;
                        case MessageBoxResult.No:
                            return;
                    }
                }
                grade = new Grade(50, 50, FilePath);
                grades.Add(grade);
                tabControlMaps.SelectedIndex = 0;

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
            temp = new Inimigo();

            EnemyEditor.DataContext = temp;
            EnemyEditor.Visibility = Visibility.Visible;
        }

        private void Adicionar_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
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

        // Adicionar Jogador
        private void Jogador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            temp = new Jogador();

            PlayerNpcEditor.DataContext = temp;
            PlayerNpcEditor.Visibility = Visibility.Visible;
        }

        // Adicionar Npc
        private void Npc_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            temp = new Npc();

            PlayerNpcEditor.DataContext = temp;
            PlayerNpcEditor.Visibility = Visibility.Visible;
        }

        // Adicionar Mapa
        private void Mapa_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string img = AbrirImagem();
            
            if (img != null)
            {
                Grade mapa = new Grade(50, 50, img);
                grades.Add(mapa);
            }
        }

        // Carregar uma imagem
        private void Imagem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            temp.Imagem = AbrirImagem();
        }

        private void Imagem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Comandos para adicionar ou cancelar a criação de uma Entidade
        private void AdicionarEntidade_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (temp.GetType() == typeof(Inimigo))
            {
                Inimigo inimigo = (Inimigo)temp;
                Inimigos.Add(inimigo);

                MessageBox.Show($"{inimigo.Vida} {inimigo.Nome}");

                EnemyEditor.Visibility = Visibility.Collapsed;
                
            }
            if (temp.GetType() == typeof(Jogador))
            {
                Jogador jogador = (Jogador)temp;
                Jogadores.Add(jogador);

                MessageBox.Show($"{jogador.Nome}");

                PlayerNpcEditor.Visibility = Visibility.Collapsed;

            }
            if (temp.GetType() == typeof(Npc))
            {
                Npc npc = (Npc)temp;
                Npcs.Add(npc);

                MessageBox.Show($"{npc.Nome}");

                PlayerNpcEditor.Visibility = Visibility.Collapsed;

            }
            temp = null;
        }

        private void AdicionarEntidade_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (temp != null && temp.Nome != null)
                {
                    if (temp.GetType() == typeof(Inimigo))
                    {
                        Inimigo inimigo = (Inimigo)temp;

                        if (inimigo.Imagem != null && inimigo.Vida >= 0 && inimigo.Nome.Length > 0)
                        {
                            e.CanExecute = true;
                        }

                    }
                    else if (temp.GetType() == typeof(Jogador))
                    {
                        Jogador jogador = (Jogador)temp;

                        if (jogador.Imagem != null)
                        {
                            e.CanExecute = true;
                        }

                    }
                    else if (temp.GetType() == typeof(Npc))
                    {
                        Npc npc = (Npc)temp;

                        if (npc.Imagem != null)
                        {
                            e.CanExecute = true;
                        }

                    }
                    else
                    {
                        e.CanExecute = false;
                    }
                }
                else
                {
                    e.CanExecute = false;
                }
                
            }
            catch (Exception)
            {
                e.CanExecute = false;
            }
            
        }

        private void CancelarEntidade_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            temp = null;

            EnemyEditor.Visibility = Visibility.Collapsed;
            PlayerNpcEditor.Visibility = Visibility.Collapsed;
        }

        private void CancelarEntidade_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DeletarEntidade_Click(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var entidade = but.DataContext;

            if (entidade.GetType() == typeof(Inimigo))
            {
                for (int i = 1; i < Inimigos.Count; i++)
                {
                    if (entidade == Inimigos[i])
                    {
                        Inimigos.RemoveAt(i);
                        return;
                    }
                }
            }
            if (entidade.GetType() == typeof(Jogador))
            {
                for (int i = 1; i < Jogadores.Count; i++)
                {
                    if (entidade == Jogadores[i])
                    {
                        Jogadores.RemoveAt(i);
                        return;
                    }
                }
            }
            if (entidade.GetType() == typeof(Npc))
            {
                for (int i = 1; i < Npcs.Count; i++)
                {
                    if (entidade == Npcs[i])
                    {
                        Npcs.RemoveAt(i);
                        return;
                    }
                }
            }
            if (entidade.GetType() == typeof(Grade))
            {
                for (int i = 1; i < grades.Count; i++)
                {
                    if (entidade == grades[i])
                    {
                        grades.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}


