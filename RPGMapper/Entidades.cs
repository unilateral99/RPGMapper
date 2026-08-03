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
    public class Entidades : INotifyPropertyChanged
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
        public string Nome
        {
            get { return _nome; }
            set
            {
                if (_nome != value)
                {
                    _nome = value;
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

    public class Inimigo : Entidades
    {
        internal int _iniciativa { get; set; }
        internal int _vida { get; set; }
        internal int _vidaAtual { get; set; }

        public int Vida
        {
            get
            {
                return _vida;
            }
            set
            {
                _vida = value;
                _vidaAtual = value;
            }
        }

        public int VidaAtual
        {
            get
            {
                return _vidaAtual;
            }
            set
            {
                _vidaAtual = value;
                OnPropertyChanged();
            }
        }

        public Inimigo()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\ADESIVO-9.png";
        }

        public Inimigo(string imagem, string nome)
        {
            Imagem = imagem;
            Nome = nome;
        }

        public Inimigo Clone()
        {
            return new Inimigo
            {
                Imagem = Imagem,
                Nome = Nome,
                Vida = Vida,
                _iniciativa = _iniciativa
            };
        }
    }

    public class Jogador : Entidades
    {
        public int Iniciativa { get; set; }

        public Jogador()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\Sebastiao.jpg";
        }

        public Jogador(string imagem, string nome)
        {
            Imagem = imagem;
            Nome = nome;
        }
    }

    public class Npc : Entidades
    {
        public Npc()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\Andre.jpg";
        }

        public Npc(string imagem, string nome)
        {
            Imagem = imagem;
            Nome = nome;
        }
    }

    public class Evento : Entidades
    {
        public string Descricao {  get; set; }

        public Evento()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\EventFlag.png";
        }

        public Evento(string nome)
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\EventFlag.png";
            Nome = nome;
        }

        public Evento(string nome, string descricao)
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\EventFlag.png";
            Nome = nome;
            Descricao = descricao;
        }
    }
}
