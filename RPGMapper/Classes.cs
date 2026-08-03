using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RPGMapper
{
    public class Tile : INotifyPropertyChanged
    {
        public int x {  get; set; }
        public int y { get; set; }
        Entidades _entidade {  get; set; }
        public Entidades Entidade 
        {
            get => _entidade;
            set
            {
                if (_entidade != value)
                {
                    _entidade = value;
                    OnPropertyChanged();
                }
            }
        }

        public Tile(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public void AdicionarEntidade(Entidades temp)
        {
            Entidade = temp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Classe para as propriedades da grade
    public class Grade : INotifyPropertyChanged
    {
        int _altura { get; set; }
        int _largura { get; set; }

        double _alturaGrade { get; set; }
        double _larguraGrade { get; set; }
        double _zoom { get; set; }

        string _nome { get; set; }
        string _imagem { get; set; }

        public ObservableCollection<Tile> Tiles;

        public int Altura
        {
            get => _altura;
            set
            {
                _altura = value;
                popularGrade();
                calcularTamanhoGrid();
                OnPropertyChanged();
            }
        }

        public int Largura
        {
            get => _largura;
            set
            {
                _largura = value;
                popularGrade();
                calcularTamanhoGrid();
                OnPropertyChanged();
            }
        }

        public double AlturaGrade
        {
            get => _alturaGrade;
            set
            {
                _alturaGrade = value;
                OnPropertyChanged();
            }
        }
        public double LarguraGrade
        {
            get => _larguraGrade;
            set
            {
                _larguraGrade = value;
                OnPropertyChanged();
            }
        }

        public double Zoom
        {
            get => _zoom;
            set
            {
                if (value >= 0.5 && value <= 2.5)
                {
                    _zoom = value;
                    calcularTamanhoGrid();
                    OnPropertyChanged();
                }
            }
        }

        public string Imagem
        {
            get => _imagem;
            set
            {
                _imagem = value;
                OnPropertyChanged();
            }
        }

        public string Nome
        {
            get => _nome;
            set
            {
                if (value != _nome)
                {
                    _nome = value;
                    OnPropertyChanged();
                }
            }
        }

        public Grade()
        {
            
        }

        public Grade(int altura, int largura, string imagem)
        {
            _altura = altura;
            _largura = largura;
            _imagem = imagem;
            _zoom = 1.0;

            Tiles = new ObservableCollection<Tile>();
            popularGrade();
            calcularTamanhoGrid();
            nomearGrade();
        }

        // Limpa a coleção atual e recria as Tiles
        void popularGrade()
        {
            Tiles.Clear();

            for (int i = 0; i < Largura; i++)
            {
                for (int j = 0; j < Altura; j++)
                {
                    Tiles.Add(new Tile(i, j));
                }
            }
        }

        // Recalcula o tamanho do controle Grid sempre que a altura ou largura é alterada
        void calcularTamanhoGrid()
        {
            AlturaGrade = (Altura * 25) * Zoom;
            LarguraGrade = (Largura * 25) * Zoom;
        }

        // Seta o nome inicial da Grade como o nome da imagem
        void nomearGrade()
        {
            string buffer = "";

            foreach (char c in Imagem)
            {
                if (c == '.')
                {
                    break;
                }
                buffer += c;
                if (c == '\\')
                {
                    buffer = "";
                }
            }

            _nome = buffer;
        }

        // Código necessário para o PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Adorner para o movimento das imagens das entidades
    public class AdornerArrastar : Adorner
    {
        private readonly VisualBrush _brush;

        public double LeftOffset { get; set; }
        public double TopOffset { get; set; }

        public AdornerArrastar(UIElement elementoAdornado) : base(elementoAdornado)
        {
            _brush = new VisualBrush(elementoAdornado);

            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(
                _brush,
                null,
                new Rect(
                    LeftOffset,
                    TopOffset,
                    AdornedElement.RenderSize.Width,
                    AdornedElement.RenderSize.Height));
        }
    }
}
