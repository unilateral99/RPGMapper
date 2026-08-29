using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RPGMapper
{
    /// <summary>
    /// Lógica interna para MenuPrincipal.xaml
    /// </summary>
    public partial class MenuPrincipal : Window
    {
        Random random = new Random();
        public string codigo { get; set; }

        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void EditarMapas_Click(object sender, RoutedEventArgs e)
        {
            EditorDeMapa editorDeMapa = new EditorDeMapa();
            this.Hide();
            editorDeMapa.ShowDialog();
            this.Show();
        }

        private void CriarSala_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            SalaJogo salaJogo = new SalaJogo(null, true);
            while (salaJogo.Codigo == null && i < 10 && salaJogo.TestarConexao())
            {
                salaJogo = new SalaJogo(GerarCodigo(), true);
                salaJogo.Criar_Sala();
                i++;
            }
            if (salaJogo.Codigo == null)
            {
                MessageBox.Show("Erro ao criar sala");
                return;
            }
            if (salaJogo.TestarConexao())
            {
                Sala sala = new Sala(salaJogo);
                this.Hide();
                sala.ShowDialog();
                this.Show();
            }
        }
        private string GerarCodigo()
        {
            codigo = random.Next(999999).ToString();

            return codigo;
        }

        // Três botões para procurar sala
        private void EntrarSala_Click(Object sender, RoutedEventArgs e)
        {
            EntrarButton.IsEnabled = false;
            EntrarButton.IsEnabled = false;
            EditarButton.IsEnabled = false;

            ProcurarSala.Visibility = Visibility.Visible;
        }

        private void Cancelar_click(Object sender, RoutedEventArgs e)
        {
            ProcurarSala.Visibility = Visibility.Collapsed;

            EntrarButton.IsEnabled = true;
            EntrarButton.IsEnabled = true;
            EditarButton.IsEnabled = true;
        }

        private void Procurar_Click(Object sender, RoutedEventArgs e)
        {
            SalaJogo salaJogo = new SalaJogo(RoomCode_TextBox.Text.Trim(), false);
            if(salaJogo.PodeEntrar(UserName_TextBox.Text))
            {
                ProcurarSala.Visibility = Visibility.Collapsed;
                Sala sala = new Sala(salaJogo);
                this.Hide();
                sala.ShowDialog();

                EntrarButton.IsEnabled = true;
                EntrarButton.IsEnabled = true;
                EditarButton.IsEnabled = true;

                this.Show();
            }
        }
        

        private void RoomCode_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");

            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
