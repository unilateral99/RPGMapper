using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RPGMapper
{
    /// <summary>
    /// Lógica interna para TelaDeJogoMestre.xaml
    /// </summary>
    public partial class TelaDeJogoMestre : Window
    {
        internal AdornerLayer layer;
        internal AdornerArrastar adorner;
        internal Ellipse imagemArrastar;

        SalaJogo sala { get; set; }

        // Timer para o banco de dados
        private DispatcherTimer _timer;

        // Timer para checar click
        private DispatcherTimer timerClick;

        Tile start;
        Tile end;

        public TelaDeJogoMestre(SalaJogo sala)
        {
            InitializeComponent();

            this.sala = sala;

            this.DataContext = sala;

            sala.Mensagens.Add(new Mensagem("agora", "Sys: ", "Sala iniciada!"));

            SetupTimer();
            SetupClickTimer();

            sala.CarregarEntidades();

            CbbInimigo.SelectedIndex = 0;
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.5)
            };
            _timer.Tick += OnTimerElapsed;
            _timer.Start();
        }

        private void SetupClickTimer()
        {
            timerClick = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };
            timerClick.Tick += OnTimerElapsedClick;
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            sala.EnviarDados();

            if (_timer != null)
            {
                _timer.Start();
            }
        }

        private void OnTimerElapsedClick(object sender, EventArgs e)
        {
            timerClick.Stop();
        }

        private void Mensagem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Pega a mensagem e cria um objeto Mensagem, que é enviado para o banco de dados
                Mensagem msg = new Mensagem(Mensagem_TextBox.Text);

                sala.MensagensEnviar.Add(msg);

                Mensagem_TextBox.Text = "";
            }
        }

        private void Chat_ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Scrolla para baixo quando vier um novo elemento
            if (e.ExtentHeightChange != 0 && (Chat_ScrollViewer.ExtentHeight - Chat_ScrollViewer.ViewportHeight - Chat_ScrollViewer.VerticalOffset) <= 10 + e.ExtentHeightChange)
            {
                Chat_ScrollViewer.ScrollToBottom();
            }
        }

        private bool atualizando;
        private void ResetarCbb(object sender, SelectionChangedEventArgs e)
        {
            if (atualizando)
            {
                return;
            }

            atualizando = true;

            var Cbb = sender as ComboBox;

            if (Cbb != CbbEvento)
            {
                CbbEvento.SelectedIndex = 0;
            }
            if (Cbb != CbbInimigo)
            {
                CbbInimigo.SelectedIndex = 0;
            }
            if (Cbb != CbbJogador)
            {
                CbbJogador.SelectedIndex = 0;
            }
            if (Cbb != CbbNpc)
            {
                CbbNpc.SelectedIndex = 0;
            }
            atualizando = false;
        }

        private void GridEntidades_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var tile = grid.DataContext as Tile;

            timerClick.Start();

            // Cria a imagem que segue o mouse quando segurado
            if (tile.Entidade != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    layer = AdornerLayer.GetAdornerLayer(MainGrid);
                    DependencyObject obj = VisualTreeHelper.GetChild(grid, 1);
                    imagemArrastar = obj as Ellipse;
                    imagemArrastar.CaptureMouse();

                    adorner = new AdornerArrastar(imagemArrastar);

                    layer.Add(adorner);

                    start = tile;
                }
            }

            // Checa se tem alguma entidade selecionada e adiciona se sim
            if (CbbInimigo.SelectedIndex != 0)
            {
                sala.AdicionarEntidades(tile, sala.inimigoSelecionado);
            }
            if (CbbJogador.SelectedIndex != 0)
            {
                sala.Mover_entidades(sala.jogadorSelecionado, tile);
            }
            if (CbbNpc.SelectedIndex != 0)
            {
                sala.AdicionarEntidades(tile, sala.npcSelecionado);
            }
            if (CbbEvento.SelectedIndex != 0)
            {
                sala.AdicionarEntidades(tile, sala.eventoSelecionado);
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Mouse.OverrideCursor = null;
                start = null;
                return;
            }
        }

        private void GridEntidades_MouseMove(object sender, MouseEventArgs e)
        {
            // Move a imagem com o mouse enquanto segurado
            if (adorner != null)
            {
                Point p = Mouse.GetPosition(adorner.AdornedElement);

                adorner.LeftOffset = p.X - imagemArrastar.ActualWidth / 2;
                adorner.TopOffset = p.Y - imagemArrastar.ActualHeight / 2;

                adorner.InvalidateVisual();
            }
        }

        private void GridEntidades_MouseUp(object sender, MouseButtonEventArgs e)
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

                end = gradeTile.DataContext as Tile;

                // Se clicado em uma entidade
                if (timerClick.IsEnabled)
                {
                    if (start != null && start.Entidade != null)
                    {
                        if (start.Entidade.GetType() == typeof(Inimigo))
                        {
                            // Enemy viewer precisa de mudanças no Banco de Dados, deixar para próxima versão.

                            //start.Entidade.Localizacao = start;
                            //EnemyViewer.DataContext = start.Entidade;
                            //EnemyViewer.Visibility = Visibility.Visible;
                        }
                    }
                }
                if (adorner != null)
                {
                    layer.Remove(adorner);

                    adorner = null;
                }
                if (imagemArrastar != null)
                {
                    imagemArrastar.ReleaseMouseCapture();
                }
                if (!timerClick.IsEnabled && start != null && end != null)
                {
                    sala.Mover_entidades(start, end);
                }
            }
            catch (Exception ex)
            {

            }
        }

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

            sala.Deletar_Entidades(inimigo);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void LimparJogador_Click(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var jogador = but.DataContext as Jogador;

            sala.RetirarJogador(jogador);
        }

        private void tabControlMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sala.GradeSelecionada = tabControlMaps.SelectedIndex;
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var tile = e.Parameter as Tile;
            if (tile == null)
            {
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
                    tile = gradeTile.DataContext as Tile;
                }
                catch
                {
                    return;
                }
            }

            sala.deletarEntidades(tile.Entidade);
            tile.Entidade = null;
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var tile = e.Parameter as Tile;
            if (tile == null)
            {
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
                    tile = gradeTile.DataContext as Tile;
                }
                catch
                {
                    return;
                }
            }

            if (tile.Entidade != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (e.Delta > 0)
                {
                    ZoomSlider.Value += 0.1;
                }
                else if (e.Delta < 0)
                {
                    ZoomSlider.Value -= 0.1;
                }
            }
        }
    }
}
