using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RPGMapper
{
    // Classe pai para as entidades futuras, incluindo inimigos, jogadores e eventos
    internal class Entidades : INotifyPropertyChanged
    {
        internal Tile _localizacao {  get; set; }
        internal string _imagem { get; set; }
        internal string _nome { get; set; }

        public Tile Localizacao
        {
            get => _localizacao;
            set
            {
                if (_localizacao != value)
                {
                    _localizacao = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Imagem
        {
            get => _imagem;
            set
            {
                if (Imagem != value)
                {
                    _imagem = value;
                    OnPropertyChanged();
                }
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class Inimigo : Entidades
    {
        internal int _iniciativa { get; set; }
        internal int _vida { get; set; }

        public Inimigo()
        {
            Imagem = "C:\\Users\\User\\Documents\\Projeto\\RPGMapper\\Imagens\\ADESIVO-9.png";
        }
    }

    internal class Jogador : Entidades
    {
        internal int _iniciativa { get; set; }

        public Jogador()
        {
            Imagem = "C:\\Users\\User\\Documents\\Projeto\\RPGMapper\\Imagens\\Sebastiao.jpg";
        }
    }

    internal class Npc : Entidades
    {
        public Npc()
        {
            Imagem = "C:\\Users\\User\\Documents\\Projeto\\RPGMapper\\Imagens\\Andre.jpg";
        }
    }
}
