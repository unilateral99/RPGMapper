using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Timers;
using System.Windows.Threading;

namespace RPGMapper
{
    /// <summary>
    /// Lógica interna para Sala.xaml
    /// </summary>
    public partial class Sala : Window
    {
        private DispatcherTimer _timer;

        public SalaJogo sala {  get; set; }


        public Sala(SalaJogo sala)
        {
            InitializeComponent();

            this.sala = sala;

            this.DataContext = sala;

            if (sala.Mestre)
            {
                MestreHud.Visibility = Visibility.Visible;
                sala.AdicionarMestre();
            }

            SetupTimer();
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, EventArgs e)
        {
            if (sala != null)
            {
                sala.ListarJogadores();
                if (sala.AtualizarJogadores())
                {
                    // Retorna verdadeiro apenas quando o estado muda de "Em espera" para "Em jogo", inicia a partida
                    MessageBox.Show("Partida começada!");
                    TelaDeJogoJogador telaJogo = new TelaDeJogoJogador(sala);
                    this.Hide();
                    telaJogo.ShowDialog();
                    this.Close();
                }

                if (sala.JogadoresTemp.Count == 0 && !sala.Mestre)
                {
                    MessageBox.Show("O mestre fechou a sala");
                    this.Close();
                    return;
                }

                if (_timer != null)
                {
                    _timer.Start();
                }
            }
        }

        private void SairSala_Click(object sender, RoutedEventArgs e)
        {
            // Sai da sala se jogador, exclui ela se for o mestre, e atualiza o banco de dados
            sala.Sair_Sala();
            this.Close();
        }

        private void EscolherMapa_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Arquivo JSON (*.json)|*.json|Todos os arquivos(*.*)|*.*";
            openFileDialog.DefaultExt = "json";
            openFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory + "Mapas\\";

            if (openFileDialog.ShowDialog() == true)
            {
                sala.Mapa = openFileDialog.FileName;
                sala.Enviar_Mapa();

                ImagemMapa.DataContext = sala.Grades[0];
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            sala.Sair_Sala();
            _timer.Stop();
            _timer.Tick -= OnTimerElapsed;
            _timer = null;
        }

        private void ComecarJogo_Click(object sender, EventArgs e)
        {
            if (sala.Mapa != null && sala.JogadoresTemp.Count > 0)
            {
                sala.ComecarJogo();

                if (sala.Mestre)
                {
                    TelaDeJogoMestre telaDeJogoMestre = new TelaDeJogoMestre(sala);
                    this.Hide();
                    telaDeJogoMestre.ShowDialog();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("É preciso ter pelo menos 1 jogador e um mapa enviado\nVocê pode fazer o mapa na opção 'Editar Mapa' no menu.");
            }
        }
    }
}
