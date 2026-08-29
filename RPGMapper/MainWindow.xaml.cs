using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace RPGMapper
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Entidades temp;
        public Transicao transicao;
        internal AdornerLayer layer;
        internal AdornerArrastar adorner;
        internal Image imagemArrastar;
        internal Tile tileOrigem;
        internal Tile tileDestino;
        internal string file;

        private DispatcherTimer timer;

        public Grade grade {  get; set; }
        ObservableCollection<Grade> _grades = new ObservableCollection<Grade>();
        public ObservableCollection<Grade> grades
        {
            get { return _grades; }
            set
            {
                _grades = value;
                OnPropertyChanged();
                tabControlMaps.SelectedIndex = 0;
                StartButton.Visibility = Visibility.Collapsed;
            }
        }

        // Listas para as diversas entidades
        ObservableCollection<Inimigo> _inimigos = new ObservableCollection<Inimigo>();
        public ObservableCollection<Inimigo> Inimigos
        {
            get { return _inimigos; }
            set
            {
                _inimigos = value;
                OnPropertyChanged();
                CbbInimigo.SelectedIndex = 0;
            }
        }
        public Inimigo inimigoSelecionado { get; set; }

        ObservableCollection<Jogador> _jogadores = new ObservableCollection<Jogador>();
        public ObservableCollection<Jogador> Jogadores
        {
            get { return _jogadores; }
            set
            {
                _jogadores = value;
                OnPropertyChanged();
                CbbJogador.SelectedIndex = 0;
            }
        }
        public Jogador jogadorSelecionado { get; set; }

        ObservableCollection<Npc> _npcs = new ObservableCollection<Npc>();
        public ObservableCollection<Npc> Npcs
        {
            get { return _npcs; }
            set
            {
                _npcs = value;
                OnPropertyChanged();
                CbbNpc.SelectedIndex = 0;
            }
        }
        public Npc npcSelecionado { get; set; }

        ObservableCollection<Evento> _eventos = new ObservableCollection<Evento>();
        public ObservableCollection<Evento> Eventos
        {
            get { return _eventos; }
            set
            {
                _eventos = value;
                OnPropertyChanged();
                CbbEvento.SelectedIndex = 0;
            }
        }
        public Evento eventoSelecionado { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(75);
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

            CbbEvento.DataContext = this;
            Eventos = new ObservableCollection<Evento>();
            Evento evento = new Evento("Eventos");
            Eventos.Add(evento);
            CbbEvento.SelectedIndex = 0;

            TransicaoEditor.DataContext = this;
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
            if (CbbEvento.SelectedIndex != 0)
            {
                tile.AdicionarEntidade(eventoSelecionado);
            }
            if (transicao != null)
            {
                Mouse.OverrideCursor = null;
                tile.AdicionarEntidade(transicao);
                MessageBox.Show(transicao.grade.Nome);
                transicao = null;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                Mouse.OverrideCursor = null;
                tileOrigem = null;
                temp = null;
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

                // Se clicado
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
            if (tabControlMaps.Items.Count > 0 && grades.Count > 0 && tabControlMaps.SelectedIndex >= 0)
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
                if (end.Entidade != null)
                {
                    return;
                }
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
            MessageBoxResult confirmacao = MessageBox.Show("Descartar mapa atual?\nIsso irá descartar todos os mapas e entidades criados", "Novo", MessageBoxButton.YesNo);
            switch (confirmacao)
            {
                case MessageBoxResult.Yes:
                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    return;
            }
        }

        private void Novo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (grades.Count > 0)
            {
                e.CanExecute = true;
            }
            else { e.CanExecute = false; }
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

            EnemyEditor.Visibility = Visibility.Collapsed;
            TransicaoEditor.Visibility = Visibility.Collapsed;
            EventEditor.Visibility = Visibility.Collapsed;

            PlayerNpcEditor.DataContext = temp;
            PlayerNpcEditor.Visibility = Visibility.Visible;
        }

        // Adicionar Npc
        private void Npc_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            temp = new Npc();

            EnemyEditor.Visibility = Visibility.Collapsed;
            TransicaoEditor.Visibility = Visibility.Collapsed;
            EventEditor.Visibility = Visibility.Collapsed;

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
            if (grades.Count == 1)
            {
                tabControlMaps.SelectedIndex = 0;
                StartButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Mapa_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // Adicionar Evento
        private void Evento_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            temp = new Evento();

            EnemyEditor.Visibility = Visibility.Collapsed;
            PlayerNpcEditor.Visibility = Visibility.Collapsed;
            TransicaoEditor.Visibility = Visibility.Collapsed;

            EventEditor.DataContext = temp;
            EventEditor.Visibility = Visibility.Visible;
        }

        // Adicionar Transição
        private void Transicao_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            transicao = new Transicao();

            EnemyEditor.Visibility = Visibility.Collapsed;
            PlayerNpcEditor.Visibility = Visibility.Collapsed;
            EventEditor.Visibility = Visibility.Collapsed;

            TransicaoMapas.SelectedIndex = 0;
            TransicaoEditor.Visibility = Visibility.Visible;
        }

        private void Transicao_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (grades.Count > 1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
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
            if (temp != null)
            {
                if (temp.GetType() == typeof(Inimigo))
                {
                    Inimigo inimigo = (Inimigo)temp;
                    Inimigos.Add(inimigo);

                    MessageBox.Show($"{inimigo.Vida} {inimigo.Nome}");

                    EnemyEditor.Visibility = Visibility.Collapsed;
                    temp = null;
                }
                else if (temp.GetType() == typeof(Jogador))
                {
                    Jogador jogador = (Jogador)temp;
                    Jogadores.Add(jogador);

                    MessageBox.Show($"{jogador.Nome}");

                    PlayerNpcEditor.Visibility = Visibility.Collapsed;
                    temp = null;
                }
                else if (temp.GetType() == typeof(Npc))
                {
                    Npc npc = (Npc)temp;
                    Npcs.Add(npc);

                    MessageBox.Show($"{npc.Nome}");

                    PlayerNpcEditor.Visibility = Visibility.Collapsed;
                    temp = null;

                }
                else if (temp.GetType() == typeof(Evento))
                {
                    Evento evento = (Evento)temp;
                    Eventos.Add(evento);

                    MessageBox.Show($"{evento.Nome}: {evento.Descricao}");

                    EventEditor.Visibility = Visibility.Collapsed;
                    temp = null;
                }
            }
            else if (transicao != null)
            {
                transicao.grade = grades[TransicaoMapas.SelectedIndex];
                TransicaoEditor.Visibility = Visibility.Collapsed;

                Mouse.OverrideCursor = Cursors.Cross;
            }
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
                    else if(temp.GetType() == typeof(Evento))
                    {
                        Evento evento = (Evento)temp;
                        if (evento.Nome != null)
                        {
                            e.CanExecute = true;
                        }
                    }
                    else
                    {
                        e.CanExecute = false;
                    }
                }
                else if (transicao != null)
                {
                    if (TransicaoMapas.SelectedIndex != tabControlMaps.SelectedIndex)
                    {
                        e.CanExecute = true;
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
            TransicaoEditor.Visibility = Visibility.Collapsed;
            EventEditor.Visibility = Visibility.Collapsed;
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
            else if (entidade.GetType() == typeof(Jogador))
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
            else if (entidade.GetType() == typeof(Npc))
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
            else if (entidade.GetType() == typeof(Grade))
            {
                for (int i = 0; i < grades.Count; i++)
                {
                    if (entidade == grades[i])
                    {
                        if (tabControlMaps.SelectedIndex == i)
                        {
                            tabControlMaps.SelectedIndex = i - 1;
                        }
                        if (grades.Count > 1)
                        {
                            MessageBoxResult result = MessageBox.Show("Removar mapa?", "Confirmação", MessageBoxButton.YesNoCancel);
                            if (result == MessageBoxResult.Yes)
                            {
                                grades.RemoveAt(i);
                            }
                        }
                        else
                        {
                            MessageBox.Show("É necessárrio ter pelo menos 1 mapa", "Falha", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        return;
                    }
                }
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

        private void Salvar_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            salvarDados();
        }
        private void SalvarComo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            escolherCaminhoSave();
            salvarDados();
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            carregarArquivo();
        }
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Salvar_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {
            if (grades.Count > 0)
            {
                e.CanExecute = true;
            }
            else { e.CanExecute = false; }
        }

        // Função para salvar os dados no arquivo
        void salvarDados()
        {
            if (file == null)
            {
                escolherCaminhoSave();
            }
            try
            {
                Arquivo arquivo = new Arquivo(grades, Inimigos, Jogadores, Npcs, Eventos);
                var json = JsonSerializer.Serialize(arquivo, new JsonSerializerOptions
                {
                    WriteIndented = true,
                });

                File.WriteAllText(file, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante o salvamento\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Função para alterar o arquivo de salvamento
        void escolherCaminhoSave()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Arquivo JSON (*.json)|*.json|Todos os arquivos(*.*)|*.*";
            saveFileDialog.DefaultExt = "json";
            saveFileDialog.FileName = "RpgMap";

            if (saveFileDialog.ShowDialog() == true)
            {
                file = saveFileDialog.FileName;
            }
        }
        void escolherCaminhoOpen()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Arquivo JSON (*.json)|*.json|Todos os arquivos(*.*)|*.*";
            openFileDialog.DefaultExt = "json";

            if (openFileDialog.ShowDialog() == true)
            {
                file = openFileDialog.FileName;
            }
        }

        // Função para carregar um arquivo
        void carregarArquivo()
        {
            escolherCaminhoOpen();
            if (file != null)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    Arquivo arquivoCarregado = JsonSerializer.Deserialize<Arquivo>(json);
                    
                    grades = arquivoCarregado.Grades;
                    Inimigos = arquivoCarregado.Inimigos;
                    Jogadores = arquivoCarregado.Jogadores;
                    Npcs = arquivoCarregado.Npcs;
                    Eventos = arquivoCarregado.Eventos;

                    grade = grades[0];

                    this.DataContext = grade;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro durante o carregamento\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AtivarEvento_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void AtivarEvento_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void AtivarTransicao_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void AtivarTransicao_CanExecute(Object sender, CanExecuteRoutedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


