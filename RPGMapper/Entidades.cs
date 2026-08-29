using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RPGMapper
{
    // Classe pai para as entidades futuras, incluindo inimigos, jogadores e eventos
    public class Entidades : INotifyPropertyChanged
    {
        [JsonIgnore] internal Tile _localizacao {  get; set; }
        internal string _imagem { get; set; }
        internal string _nome { get; set; }
        internal Visibility _visivel { get; set; } = Visibility.Visible;

        public int Id { get; set; }

        [JsonIgnore]
        public Tile Localizacao
        {
            get => _localizacao;
            set
            {
                if (_localizacao == null)
                {
                    _localizacao = value;
                }
                else if (_localizacao != value)
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
        public Visibility Visivel
        {
            get
            {
                return _visivel;
            }
            set
            {
                _visivel = value;
                OnPropertyChanged();
            }
        }

        public Entidades(int id, string nome, string imagem, Tile localizacao)
        {
            Id = id;
            Nome = nome;
            Imagem = imagem;
            Localizacao = localizacao;
        }

        public Entidades()
        {
            
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

        public Inimigo(int id, string nome, string imagem, Tile localizacao) : base(id, nome, imagem, localizacao) 
        {
            
        }
    }

    public class Jogador : Entidades
    {
        public int Iniciativa { get; set; }
        public string _nomeJogador { get; set; }
        public int idJogador { get; set; }

        public string NomeJogador
        {
            get
            {
                return _nomeJogador;
            }
            set
            {
                _nomeJogador = value;
                OnPropertyChanged();
            }
        }

        public Jogador Clone()
        {
            return new Jogador
            {
                Imagem = Imagem,
                Nome = Nome,
                NomeJogador = NomeJogador,
                Iniciativa = Iniciativa
            };
        }

        public Jogador()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\Sebastiao.jpg";
        }

        public Jogador(string imagem, string nome)
        {
            Imagem = imagem;
            Nome = nome;
        }

        public Jogador(int id, string nome, string imagem, Tile localizacao) : base(id, nome, imagem, localizacao)
        {
            
        }

        public Jogador(int id, string nome, string imagem) 
        {
            Id = id;
            Nome = nome;
            Imagem = imagem;
        }
    }

    public class Npc : Entidades
    {
        public Npc Clone(int id)
        {
            return new Npc
            {
                Id = id,
                Imagem = Imagem,
                Nome = Nome,
            };
        }

        public Npc()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\Andre.jpg";
        }

        public Npc(string imagem, string nome)
        {
            Imagem = imagem;
            Nome = nome;
        }

        public Npc(int id, string nome, string imagem, Tile localizacao) : base(id, nome, imagem, localizacao)
        {
            
        }
    }

    public class Evento : Entidades
    {
        public string Descricao {  get; set; }

        public Evento Clone(int id)
        {
            return new Evento
            {
                Descricao = Descricao,
                Id = id,
                Imagem = Imagem,
                Nome = Nome,
            };
        }

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

        public Evento(int id, string nome, string imagem, Tile localizacao) : base(id, nome, imagem, localizacao)
        {
            
        }
    }

    public class Transicao : Entidades
    {
        public Grade grade { get; set; }

        public Transicao()
        {
            Imagem = System.AppDomain.CurrentDomain.BaseDirectory + "\\Imagens\\transicao.png";
        }

        public Transicao(int id, string nome, string imagem, Tile localizacao, Grade grade) : base(id, nome, imagem, localizacao)
        {
            this.grade = grade;
        }
    }

    public class Mensagem
    {
        public string Data {  get; set; }
        public string NomeJogador { get; set; }
        public string Texto {  get; set; }

        public Mensagem(string data, string nomeJogador, string texto)
        {
            Data = data;
            NomeJogador = nomeJogador;
            Texto = texto;
        }

        public Mensagem(string texto)
        {
            Texto = texto;
        }
    }
}
