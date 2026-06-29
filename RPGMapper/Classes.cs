using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RPGMapper
{
    internal class Tile
    {
        public int x {  get; set; }
        public int y { get; set; }

        public Tile(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }

    // Classe para as propriedades da grade
    internal class Grade : INotifyPropertyChanged
    {
        int _altura { get; set; }
        int _largura { get; set; }

        double _alturaGrade { get; set; }
        double _larguraGrade { get; set; }
        double _zoom { get; set; }

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

        public Grade()
        {
            _altura = 50;
            _largura = 50;
            _zoom = 1.0;

            Tiles = new ObservableCollection<Tile>();
            popularGrade();
            calcularTamanhoGrid();
        }

        void popularGrade()
        {
            Tiles.Clear();

            for (int i = 0; i < _largura; i++)
            {
                for (int j = 0; j < _altura; j++)
                {
                    Tiles.Add(new Tile(i, j));
                }
            }
        }

        // Recalcula o tamanho do controle Grid sempre que a altura ou largura é alterada
        void calcularTamanhoGrid()
        {
            AlturaGrade = (Altura * 25) * _zoom;
            LarguraGrade = (Largura * 25) * _zoom;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
