using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using System.Windows.Threading;

namespace RPGMapper
{
    /// <summary>
    /// Lógica interna para TelaDeJogoJogador.xaml
    /// </summary>
    public partial class TelaDeJogoJogador : Window
    {
        internal AdornerLayer layer;
        internal AdornerArrastar adorner;
        internal Ellipse imagemArrastar;


        // Timer para o banco de dados
        private DispatcherTimer _timer;

        Tile start;
        Tile end;

        SalaJogo sala {  get; set; }

        public TelaDeJogoJogador(SalaJogo salajogo)
        {
            InitializeComponent();

            this.sala = salajogo;

            this.DataContext = sala;

            sala.Mensagens.Add(new Mensagem("agora","Sys: ", "Sala iniciada!"));

            SetupTimer();
            sala.PopularPersonagens();
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

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            if (sala != null)
            {
                sala.EnviarDados();
                if (!sala.AtualizarJogadores())
                {
                    MessageBox.Show("O mestre fechou a sala!");
                    this.Close();
                }
                else if (sala.JogadoresSelecionar.Count > 0)
                {
                    PlayerSelector.IsHitTestVisible = true;
                    PlayerSelector.Visibility = Visibility.Visible;
                }
                else
                {
                    PlayerSelector.IsHitTestVisible = false;
                    PlayerSelector.Visibility = Visibility.Collapsed;

                }

                if (_timer != null)
                {
                    _timer.Start();
                }
            }
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
            if (e.ExtentHeightChange != 0 && (Chat_ScrollViewer.ExtentHeight - Chat_ScrollViewer.ViewportHeight - Chat_ScrollViewer.VerticalOffset) <=  10 + e.ExtentHeightChange)
            {
                Chat_ScrollViewer.ScrollToBottom();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var tile = grid.DataContext as Tile;
            Jogador entidade;

            if (tile.Entidade != null )
            {
                if (tile.Entidade.GetType() == typeof(Jogador))
                {
                    entidade = tile.Entidade as Jogador;

                    if (e.LeftButton == MouseButtonState.Pressed && entidade.idJogador == sala.id_jogador)
                    {
                        // Cria a imagem que segue o mouse quando segurado
                        layer = AdornerLayer.GetAdornerLayer(MainGrid);
                        DependencyObject obj = VisualTreeHelper.GetChild(grid, 1);
                        imagemArrastar = obj as Ellipse;
                        imagemArrastar.CaptureMouse();

                        adorner = new AdornerArrastar(imagemArrastar);

                        layer.Add(adorner);

                        start = tile;
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void EntidadeImagem_MouseMove(object sender, MouseEventArgs e)
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

        private void EntidadeImagem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Checa o objeto na posição do mouse até achar a grid
                Point p = Mouse.GetPosition(GradeGrid);
                HitTestResult result = VisualTreeHelper.HitTest(GradeGrid, p);
                DependencyObject obj = result.VisualHit;
                while (obj != null && obj.GetType() != typeof(Grid))
                {
                    obj = VisualTreeHelper.GetParent(obj);
                }

                Grid gradeTile = obj as Grid;
                end = gradeTile.DataContext as Tile;

                
                if (adorner != null)
                {
                    layer.Remove(adorner);

                    adorner = null;
                }
                if (imagemArrastar != null)
                {
                    imagemArrastar.ReleaseMouseCapture();
                }
                if (start != null && end != null)
                {
                    sala.Mover_entidades(start, end);
                }
            }
            catch (Exception ex)
            {
                layer.Remove(adorner);
            }
        }

        private void EscolherJogador_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var but = sender as Button;
            var jogador = but.DataContext as Jogador;

            if (jogador.NomeJogador == null)
            {
                e.CanExecute = true;
                return;
            }
            e.CanExecute = false;
        }

        private void EscolherJogador_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var but = sender as Button;
            var jogador = but.DataContext as Jogador;

            sala.JogadorAtual = jogador;
            jogador.NomeJogador = sala.nome_jogador;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            sala = null;
            _timer.Stop();
            _timer = null;
        }

        private void Selecionar_Click(object sender, RoutedEventArgs e)
        {
            var but = sender as Button;
            var jogador = but.DataContext as Jogador;

            jogador.NomeJogador = sala.nome_jogador;
            sala.EscolherJogador(jogador);
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
