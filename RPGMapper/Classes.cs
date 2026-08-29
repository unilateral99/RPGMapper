using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public int numero { get; set; }
        Entidades _entidade {  get; set; }
        public Entidades Entidade 
        {
            get => _entidade;
            set
            {
                if (_entidade != value)
                {
                    _entidade = value;
                    
                    if (_entidade != null)
                    {
                        _entidade.Localizacao = this;
                    }
                    OnPropertyChanged();
                }
            }
        }

        public Tile(int X, int Y, int numero)
        {
            x = X;
            y = Y;
            this.numero = numero;
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
        int maxSize = 100;

        double _alturaGrade { get; set; }
        double _larguraGrade { get; set; }
        double _zoom { get; set; }

        string _nome { get; set; }
        string _imagem { get; set; }

        [JsonIgnore]
        public ObservableCollection<Tile> Tiles { get; set; } = new ObservableCollection<Tile>();

        public int Altura
        {
            get => _altura;
            set
            {
                if (value <= maxSize)
                {
                    _altura = value;
                    popularGrade();
                    calcularTamanhoGrid();
                    OnPropertyChanged();
                }
            }
        }

        public int Largura
        {
            get => _largura;
            set
            {
                if (value <= maxSize)
                {
                    _largura = value;
                    popularGrade();
                    calcularTamanhoGrid();
                    OnPropertyChanged();
                }
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
                    Tiles.Add(new Tile(i, j, ((i * Largura) + j)));
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

    // Classe para salvar o arquivo dos mapas quando finalizado
    public class Arquivo
    {
        public ObservableCollection<Grade> Grades { get; set; } = new ObservableCollection<Grade>();
        public ObservableCollection<Inimigo> Inimigos { get; set; } = new ObservableCollection<Inimigo>();
        public ObservableCollection<Jogador> Jogadores { get; set; } = new ObservableCollection<Jogador>();
        public ObservableCollection<Npc> Npcs { get; set; } = new ObservableCollection<Npc>();
        public ObservableCollection<Evento> Eventos { get; set; } = new ObservableCollection<Evento>();
        public ObservableCollection<ObservableCollection<Tile>> TileEntidades { get; set; } = new ObservableCollection<ObservableCollection<Tile>>();

        public Arquivo()
        {

        }

        public Arquivo(ObservableCollection<Grade> grade, ObservableCollection<Inimigo> inimigos, ObservableCollection<Jogador> jogadores,
            ObservableCollection<Npc> npcs, ObservableCollection<Evento> eventos)
        {
            Grades = grade;
            Inimigos = inimigos;
            Jogadores = jogadores;
            Npcs = npcs;
            Eventos = eventos;

            TileEntidades = SalvarEntidades(grade);
        }

        // Método para salvar apenas as Tiles que contem entidades
        ObservableCollection<ObservableCollection<Tile>> SalvarEntidades(ObservableCollection<Grade> grades)
        {
            ObservableCollection<ObservableCollection<Tile>> gradeTiles = new ObservableCollection<ObservableCollection<Tile>>();
            ObservableCollection<Tile> tiles = new ObservableCollection<Tile>();

            foreach (Grade grade in grades)
            {
                tiles = new ObservableCollection<Tile>();
                foreach (Tile tile in grade.Tiles)
                {
                    if (tile.Entidade != null && tile.Entidade.GetType() != typeof(Jogador))
                    {
                        tiles.Add(tile);
                    }
                }
                gradeTiles.Add(tiles);
            }
            if (gradeTiles.Count > 0)
            {
                return gradeTiles;
            }
            return null;
        }
    }

    // Classe para a sala online
    public class SalaJogo : INotifyPropertyChanged
    {
        // Variaveis de conexão com o banco de dados
        private readonly string datasource = "server=localhost;user id=root;password=;database=rpgMapper";
        private MySqlConnection connection;

        // Listas carregadas do arquivo
        public ObservableCollection<Grade> Grades { get; set; } = new ObservableCollection<Grade>();
        public Grade _mapaAtual {  get; set; }
        public int _gradeSelecionada { get; set; } = 0;
        public ObservableCollection<Inimigo> Inimigos { get; set; } = new ObservableCollection<Inimigo>();
        public ObservableCollection<Jogador> Jogadores { get; set; } = new ObservableCollection<Jogador>();
        public ObservableCollection<Jogador> JogadoresSelecionar { get; set; } = new ObservableCollection<Jogador>();
        public ObservableCollection<Npc> Npcs { get; set; } = new ObservableCollection<Npc>();
        public ObservableCollection<Evento> Eventos { get; set; } = new ObservableCollection<Evento>();
        public ObservableCollection<ObservableCollection<Tile>> TileEntidades { get; set; } = new ObservableCollection<ObservableCollection<Tile>>();

        // Objetos para as comboboxs

        public Inimigo inimigoSelecionado { get; set; }
        public Jogador jogadorSelecionado { get; set; }
        public Npc npcSelecionado { get; set; }
        public Evento eventoSelecionado { get; set; }

        // Objetos para as comboboxs

        // Listas carregadas do arquivo

        // Lista para carregar as mensagens do Banco de dados
        public ObservableCollection<Mensagem> Mensagens { get; set; } = new ObservableCollection<Mensagem>();

        // Lista das mensagens ainda não enviadas ao banco de dados
        public List<Mensagem> MensagensEnviar {  get; set; } = new List<Mensagem>();

        // Guarda a id da ultima mensagem carregada para buscar apenas mensagens novas
        public int UltimaIdMsg { get; set; } = 0;
        public int UltimaIdEntidade { get; set; } = 0;
        public int UltimaAtualizacao { get; set; } = 0;


        // Guarda o nome dos jogadores para depois ser associado com os Objetos Jogadores em jogo
        public ObservableCollection<String> JogadoresTemp {  get; set; } = new ObservableCollection<String>();
        public ObservableCollection<Entidades> JogadoresInimigos { get; set; } = new ObservableCollection<Entidades>();
        public ObservableCollection<Jogador> JogadoresPlayer { get; set; } = new ObservableCollection<Jogador>();
        public List<Entidades> EntidadesMapa { get; set; } = new List<Entidades>();
        public List<Entidades> EntidadesMovidas { get; set; } = new List<Entidades>();
        public Jogador JogadorAtual { get; set; }
        public bool Mestre { get; set; }
        public int id_jogador { get; set; }
        public string nome_jogador { get; set;  }

        public Grade MapaAtual
        {
            get
            {
                if (_mapaAtual == null)
                {
                    _mapaAtual = Grades[0];
                }
                return _mapaAtual;
            }
            set
            {
                _mapaAtual = value;
                OnPropertyChanged();
            }
        }

        public int GradeSelecionada
        {
            get { return _gradeSelecionada; }
            set
            {
                _gradeSelecionada = value;
                MapaAtual = Grades[GradeSelecionada];
                OnPropertyChanged();
            }
        }

        public string _codigo {  get; set; }
        public string Codigo
        {
            get { return _codigo; }
            set
            {
                if (value != null)
                {
                    _codigo = value;
                }
            }
        }

        public string _mapa;
        public string Mapa
        {
            get
            {
                return _mapa;
            }
            set
            {
                if (_mapa != value)
                {
                    if (value != null)
                    {
                        _mapa = value;
                        Enviar_Mapa();
                    }
                }
            }
        }

        public SalaJogo(string codigo, bool mestre)
        {
            Codigo = codigo;
            Mestre = mestre;
        }

        public bool CarregarArquivo()
        {
            if (!string.IsNullOrEmpty(_mapa))
            {
                try
                {
                    // Primeira parte do carregamento, carregando os mapas e entidades
                    string json = File.ReadAllText(_mapa);
                    Arquivo arquivoCarregado = JsonSerializer.Deserialize<Arquivo>(json);

                    Grades = arquivoCarregado.Grades;
                    Inimigos = arquivoCarregado.Inimigos;
                    Jogadores = arquivoCarregado.Jogadores;
                    Npcs = arquivoCarregado.Npcs;
                    Eventos = arquivoCarregado.Eventos;
                    TileEntidades = arquivoCarregado.TileEntidades;

                    JogadoresPlayer.Clear();
                    foreach (var entidade in Jogadores)
                    {
                        if (entidade == Jogadores[0])
                        {
                            continue;
                        } 
                        JogadoresPlayer.Add(entidade);
                    }

                    JogadoresInimigos.Clear();

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro durante o carregamento\n{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
            return false;
        }

        // Função para enviar dados ao banco de dados
        public void EnviarDados()
        {
            try
            {
                connection = new MySqlConnection(datasource);
                MySqlCommand command;
                string sql;

                // Mensagens:

                for (int i = MensagensEnviar.Count - 1; i >= 0; i--)
                {
                    sql = "INSERT INTO mensagens(texto, id_jogador, codigo_sala) VALUES (@texto, @id_jogador, @codigo_sala)";
                    command = new MySqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@texto", MensagensEnviar[i].Texto);
                    command.Parameters.AddWithValue("@id_jogador", id_jogador);
                    command.Parameters.AddWithValue("@codigo_sala", Codigo);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MensagensEnviar.RemoveAt(i);
                }


                // Entidades:
                // Enviar as mudanças feitas ao banco
                if (EntidadesMovidas.Count > 0)
                {
                    connection.Open();
                    foreach (var entidade in EntidadesMovidas)
                    {
                        if (entidade != null)
                        {
                            // Chama mover_entidade
                            sql = "CALL mover_entidade(@id_entidade, @localizacao, @codigo)";

                            command = new MySqlCommand(sql, connection);

                            command.Parameters.AddWithValue("@id_entidade", entidade.Id);
                            command.Parameters.AddWithValue("@localizacao", entidade.Localizacao.numero);
                            command.Parameters.AddWithValue("@codigo", Codigo);

                            command.ExecuteNonQuery();

                            if (entidade.GetType() == typeof(Jogador))
                            {
                                sql = "UPDATE entidade SET mapa = @mapaatual WHERE id_entidade = @id_entidade";

                                command = new MySqlCommand(sql, connection);

                                command.Parameters.AddWithValue("@mapaatual", GradeSelecionada);
                                command.Parameters.AddWithValue("@id_entidade", entidade.Id);

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    connection.Close();

                    EntidadesMovidas.Clear();
                }

                // Enviar mudança de mapas
                if (Mestre)
                {
                    sql = "UPDATE sala SET mapa_atual = @GradeSelecionada WHERE codigo = @codigo";

                    using (connection = new MySqlConnection(datasource))
                    using (command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@GradeSelecionada", GradeSelecionada);
                        command.Parameters.AddWithValue("@codigo", Codigo);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro Envio:\n{ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            ReceberDados();
        }

        // Função para receber os dados mais recentes do banco de dados em relação a sala
        public void ReceberDados()
        {
            try
            {
                string sql;
                sql = "CALL listar_mensagens(@id_ultimaMsg, @codigo_sala)";

                using (connection = new MySqlConnection(datasource))
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_ultimaMsg", UltimaIdMsg);
                    command.Parameters.AddWithValue("@codigo_sala", Codigo);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        // Mensagens
                        while (reader.Read())
                        {
                            // Cria uma nova mensagem e adiciona ás mensagens
                            Mensagens.Add(new Mensagem(reader["data_msg"].ToString(), reader["nome"].ToString(), reader["texto"].ToString()));

                            // Atualiza o campo ultima id para evitar ler mensagens antigas
                            UltimaIdMsg = int.Parse(reader["id_mensagem"].ToString());
                        }
                    }
                    connection.Clone();
                }

                // Chamar listar_mudancas
                sql = "CALL listar_mudancas(@codigo, @ultima_atualizacao)";

                using (connection = new MySqlConnection(datasource))
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@codigo", Codigo);
                    command.Parameters.AddWithValue("@ultima_atualizacao", UltimaAtualizacao);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int id;
                        int mapa;
                        int local;
                        while (reader.Read())
                        {
                            if (!int.TryParse(reader["id_entidade"].ToString(), out id))
                            {
                                return;
                            }
                            if (!int.TryParse(reader["mapa"].ToString(), out mapa))
                            {
                                mapa = -1;
                            }
                            if (!int.TryParse(reader["localizacao"].ToString(), out local))
                            {
                                local = -1;
                            }

                            foreach (Entidades entidades in EntidadesMapa)
                            {
                                if (entidades.Id == id)
                                {
                                    if (entidades.Localizacao != null)
                                    {
                                        entidades.Localizacao.Entidade = null;
                                    }
                                    
                                    if (mapa >= 0 && local >= 0)
                                    {
                                        //MessageBox.Show($"Entidade de id {entidades.Id} movida para tile de numero {local}");

                                        Grades[mapa].Tiles[local].Entidade = entidades;
                                    }
                                }
                            }
                            UltimaAtualizacao = int.Parse(reader["id_mudanca"].ToString());
                        }
                    }
                    connection.Clone();
                }

                // Carrega as entidades
                // EntidadesMapa
                // Chama listar_novas_entidades, adiciona elas a lista de entidades e atualiza o UltimaIdEntidade
                sql = "CALL listar_novas_entidades(@codigo, @ultima_id)";

                using (connection = new MySqlConnection(datasource))
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@codigo", Codigo);
                    command.Parameters.AddWithValue("@ultima_id", UltimaIdEntidade);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        int idtemp, mapatemp, localizacaotemp;

                        while (reader.Read())
                        {
                            if (!int.TryParse(reader["id_entidade"].ToString(), out idtemp))
                            {
                                //MessageBox.Show("nao lido id");
                                return;
                            }
                            if (!int.TryParse(reader["mapa"].ToString(), out mapatemp))
                            {
                                //MessageBox.Show("nao lido mapa");
                                mapatemp = -1;
                            }
                            if (!int.TryParse(reader["localizacao"].ToString(), out localizacaotemp))
                            {
                                //MessageBox.Show("nao lido local");
                                localizacaotemp = -1;
                            }

                            if (reader["tipo"].ToString() == "jogador")
                            {
                                Jogador entidade;
                                // Jogador
                                if (mapatemp > -1 && localizacaotemp > -1)
                                {
                                    entidade = new Jogador(idtemp, reader["nome"].ToString(), reader["imagem"].ToString(), Grades[mapatemp].Tiles[localizacaotemp]);
                                }
                                else
                                {
                                    entidade = new Jogador(idtemp, reader["nome"].ToString(), reader["imagem"].ToString());
                                }
                                //MessageBox.Show($"{entidade.Nome}, {entidade.Imagem}");
                                EntidadesMapa.Add(entidade);
                                //MessageBox.Show($"{Grades[mapatemp].Tiles[localizacaotemp].x}, {Grades[mapatemp].Tiles[localizacaotemp].y}");
                            }
                            else if (reader["tipo"].ToString() == "entidade")
                            {
                                // Inimigo
                                Inimigo entidade = new Inimigo(idtemp, reader["nome"].ToString(), reader["imagem"].ToString(), Grades[mapatemp].Tiles[localizacaotemp]);
                                //MessageBox.Show($"{entidade.Nome}, {entidade.Imagem}");
                                EntidadesMapa.Add(entidade);
                                Grades[mapatemp].Tiles[localizacaotemp].AdicionarEntidade(entidade);
                                //MessageBox.Show($"{Grades[mapatemp].Tiles[localizacaotemp].x}, {Grades[mapatemp].Tiles[localizacaotemp].y}");
                            }
                            else if (reader["tipo"].ToString() == "transicao")
                            {
                                // Transição
                                Transicao entidade = new Transicao(idtemp, reader["nome"].ToString(), reader["imagem"].ToString(), Grades[mapatemp].Tiles[localizacaotemp], Grades[int.Parse(reader["mapa_destino"].ToString())]);
                                //MessageBox.Show($"{entidade.Nome}, {entidade.Imagem}");
                                EntidadesMapa.Add(entidade);
                                Grades[mapatemp].Tiles[localizacaotemp].AdicionarEntidade(entidade);
                                //MessageBox.Show($"{Grades[mapatemp].Tiles[localizacaotemp].x}, {Grades[mapatemp].Tiles[localizacaotemp].y}");
                            }
                            else if (reader["tipo"].ToString() == "evento")
                            {
                                // Evento
                                Evento entidade = new Evento(idtemp, reader["nome"].ToString(), reader["imagem"].ToString(), Grades[mapatemp].Tiles[localizacaotemp]);
                                //MessageBox.Show($"{entidade.Nome}, {entidade.Imagem}");
                                EntidadesMapa.Add(entidade);
                                Grades[mapatemp].Tiles[localizacaotemp].AdicionarEntidade(entidade);
                                //MessageBox.Show($"{Grades[mapatemp].Tiles[localizacaotemp].x}, {Grades[mapatemp].Tiles[localizacaotemp].y}");
                            }
                            else if (reader["tipo"].ToString() == "npc")
                            {
                                // Npc
                                Npc entidade = new Npc(idtemp, reader["nome"].ToString(), reader["imagem"].ToString(), Grades[mapatemp].Tiles[localizacaotemp]);
                                //MessageBox.Show($"{entidade.Nome}, {entidade.Imagem}");
                                EntidadesMapa.Add(entidade);
                                Grades[mapatemp].Tiles[localizacaotemp].AdicionarEntidade(entidade);
                                //MessageBox.Show($"{Grades[mapatemp].Tiles[localizacaotemp].x}, {Grades[mapatemp].Tiles[localizacaotemp].y}");
                            }
                            UltimaIdEntidade = int.Parse(reader["id_entidade"].ToString());
                        }
                    }
                    connection.Clone();
                }

                // Checar se teve alguma mudança nas associações de player
                sql = "SELECT id_entidade, personagem_id FROM entidade WHERE codigo_sala = @codigo AND tipo = 'jogador'";
                using (connection = new MySqlConnection(datasource))
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@codigo", Codigo);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            
                            foreach (var entidade in EntidadesMapa)
                            {
                                if (entidade.Id == int.Parse(reader["id_entidade"].ToString()))
                                {
                                    var marcelo_god = entidade as Jogador;
                                    if (reader["personagem_id"] != DBNull.Value)
                                    {
                                        marcelo_god.idJogador = Convert.ToInt16(reader["personagem_id"]);
                                    }
                                }
                            }
                        }
                    }
                    connection.Clone();
                }

                // Retorna todos os jogadores que estão sem um representante na sala
                sql = "SELECT id_entidade FROM entidade WHERE codigo_sala = @codigo AND personagem_id IS NULL AND tipo = 'jogador' AND localizacao IS NOT NULL";

                using (connection = new MySqlConnection(datasource))
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@codigo", Codigo);

                    connection.Open();
                    JogadoresSelecionar.Clear();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (var jogador in Jogadores)
                            {
                                if (jogador.Id == int.Parse(reader["id_entidade"].ToString()))
                                {
                                    jogador.NomeJogador = null;
                                    JogadoresSelecionar.Add(jogador);
                                }
                            }
                        }
                    }
                    connection.Clone();
                }

                // Retorna os jogadores que estão com um representante na sala
                sql = @"SELECT e.id_entidade, e.personagem_id, j.nome FROM entidade e
                        JOIN jogador j ON e.personagem_id = j.id
                        WHERE e.codigo_sala = @codigo";

                using (connection = new MySqlConnection(datasource))
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@codigo", Codigo);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (var jogador in Jogadores)
                            {
                                if (jogador.Id == int.Parse(reader["id_entidade"].ToString()))
                                {
                                    jogador.NomeJogador = reader["nome"].ToString();
                                    jogador.idJogador = int.Parse(reader["personagem_id"].ToString());
                                    JogadoresInimigos.Add(jogador);
                                }
                            }
                        }
                    }
                    connection.Clone();
                }

                // Verificar se a grade selecionada mudou
                if (!Mestre)
                {
                    sql = "SELECT * FROM sala WHERE codigo = @codigo LIMIT 1";
                    using (connection = new MySqlConnection(datasource))
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@codigo", Codigo);

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GradeSelecionada = Convert.ToInt16(reader["mapa_atual"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao receber dados:\n{ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Função para fazer a troca de tiles e guardar e enviar ao banco de dados
        public void Mover_entidades(Tile start, Tile end)
        {
            if (end.Entidade == null)
            {
                end.Entidade = start.Entidade;
                start.Entidade = null;

                EntidadesMovidas.Add(end.Entidade);
            }
        }

        // Variação da função para caso tiver a entidade
        public void Mover_entidades(Entidades entidade, Tile end)
        {
            if (end.Entidade == null)
            {
                end.Entidade = entidade;

                EntidadesMovidas.Add(end.Entidade);
                //MessageBox.Show(entidade.Localizacao.numero.ToString());
            }
        }

        public void Deletar_Entidades(Entidades entidades)
        {
            EntidadesMovidas.Add(entidades);
        }

        public void AdicionarEntidades(Tile tile, Entidades entidades)
        {
            try
            {
                if (entidades != null)
                {
                    connection = new MySqlConnection(datasource);
                    string sql = "CALL adicionar_entidade(@codigo, @mapa, @nome, @imagem, @visibilidade, @tipo, @localizacao)";

                    MySqlCommand command = new MySqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@codigo", Codigo);
                    command.Parameters.AddWithValue("@mapa", GradeSelecionada);
                    command.Parameters.AddWithValue("@nome", entidades.Nome);
                    command.Parameters.AddWithValue("@imagem", entidades.Imagem);
                    command.Parameters.AddWithValue("@visibilidade", true);
                    command.Parameters.AddWithValue("@localizacao", tile.numero);

                    // Gambiarra
                    if (!(entidades.GetType() == typeof(Inimigo)))
                    {
                        command.Parameters.AddWithValue("@tipo", entidades.GetType().ToString().Remove(0, 10).ToLower());
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@tipo", "entidade");
                    }

                    // Envia a entidade ao banco de dados
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar Entidades:\n{ex.Message}");
            }
            finally { connection.Close(); }

        }

        // Função receber dados necessários para a sala de espera (Mapa e estado de jogo)
        public bool AtualizarJogadores()
        {
            try
            {
                // Apenas necessário para os jogadores
                if (!Mestre)
                {
                    connection = new MySqlConnection(datasource);
                    string sql = "SELECT mapa, estado FROM sala WHERE codigo = @codigo";

                    MySqlCommand comando = new MySqlCommand(sql, connection);

                    comando.Parameters.AddWithValue("@codigo", Codigo);

                    connection.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while(reader.Read())
                    {
                        Mapa = reader["mapa"].ToString();

                        if (reader["estado"].ToString().Equals("em jogo"))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar jogadores:\n" + ex.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        // Função para receber a lista de jogadores do banco de dados
        public void ListarJogadores()
        {
            try
            {
                connection = new MySqlConnection(datasource);
                string sql = "CALL listar_jogadores(@codigo)";

                MySqlCommand comando = new MySqlCommand(sql, connection);

                comando.Parameters.AddWithValue("@codigo", Codigo);

                connection.Open();

                MySqlDataReader reader = comando.ExecuteReader();

                JogadoresTemp.Clear();
                while (reader.Read())
                {
                    JogadoresTemp.Add(reader["nome"].ToString());
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao listar jogadores: {ex.Message}");
            }
            finally { connection.Close(); }
        }

        // Função chamada para criar a sala
        public void Criar_Sala()
        {
            try
            {
                connection = new MySqlConnection(datasource);
                string sql = "INSERT INTO sala(codigo) VALUES (@code)";

                MySqlCommand comando = new MySqlCommand(sql, connection);

                comando.Parameters.AddWithValue("@code", Codigo);

                connection.Open();
                comando.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                _codigo = null;
            }
            catch (Exception ex)
            {
                _codigo = null;
                MessageBox.Show($"Erro ao criar sala: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void Enviar_Mapa()
        {
            if (CarregarArquivo() && Mestre)
            {
                try
                {
                    connection = new MySqlConnection(datasource);
                    string sql = "UPDATE sala SET mapa = @mapa WHERE codigo = @codigo";

                    MySqlCommand comando = new MySqlCommand(sql, connection);

                    comando.Parameters.AddWithValue("@codigo", Codigo);
                    comando.Parameters.AddWithValue("@mapa", _mapa);

                    connection.Open();
                    comando.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao enviar mapa: {ex.Message}");
                    Mapa = null;
                }
                finally { connection.Close(); }
            }
        }

        // Função para determinar se um usuário pode ou não entrar na sala
        public bool PodeEntrar(string nome_jogador)
        {
            try
            {
                // Checa se a sala existe e se ela não está cheia
                connection = new MySqlConnection(datasource);
                string sql = "CALL pode_entrar(@codigo, @nome)";

                MySqlCommand comando = new MySqlCommand(sql, connection);

                comando.Parameters.AddWithValue("@codigo", Codigo);
                comando.Parameters.AddWithValue("@nome", nome_jogador);

                connection.Open();

                MySqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["saida"].ToString().Equals("0"))
                    {
                        MessageBox.Show($"Erro ao entrar:\n{reader["mensagem"].ToString()}");
                        return false;
                    }
                    else if (reader["saida"].ToString().Equals("1"))
                    {
                        id_jogador = int.Parse(reader["id_jogador"].ToString());
                        this.nome_jogador = reader["nome"].ToString();
                        return true;
                    }
                }
                return false;
            }
            catch ( Exception ex )
            {
                MessageBox.Show("Erro ao entrar:\n" + ex.Message);
                return false;
            }
            finally { connection.Close(); }
        }

        public void Sair_Sala()
        {
            try
            {
                connection = new MySqlConnection(datasource);


                if (Mestre)
                {
                    // Se for o mestre deletar a sala
                    string sql = "DELETE FROM sala WHERE codigo = @codigo";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@codigo", Codigo);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                else
                {
                    // Se for jogador só tirar ele
                    string sql = "DELETE FROM jogador WHERE id = @id_jogador";
                    MySqlCommand command = new MySqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@id_jogador", id_jogador);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally { connection.Close(); }
        }

        public void ComecarJogo()
        {
            try
            {
                connection = new MySqlConnection(datasource);

                string sql = "UPDATE sala SET estado = 'em jogo' WHERE codigo = @codigo";
                MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@codigo", Codigo);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao comecar jogo:\n" + ex.Message);
            }
            finally { connection.Close(); }
        }

        public void CarregarEntidades()
        {
            // Envia os jogadores ao banco de dados
            foreach (var jogador in Jogadores)
            {
                try
                {
                    if (jogador == Jogadores[0])
                    {
                        continue;
                    }
                    connection = new MySqlConnection(datasource);

                    string sql = "CALL carregar_jogadores(@codigo, @nome, @imagem)";

                    MySqlCommand command = new MySqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@codigo", Codigo);
                    command.Parameters.AddWithValue("@nome", jogador.Nome);
                    command.Parameters.AddWithValue("@imagem", jogador.Imagem);

                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        jogador.Id = int.Parse(reader["id"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar jogadores:\n" + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

            int tileLocal;

            // Envia as entidades que já estão posicionadas no mapa (não incluindo jogadores) ao banco de dados
            for (int i = 0; i < TileEntidades.Count; i++)
            {
                for (int j = 0; j < TileEntidades[i].Count; j++)
                {
                    tileLocal = TileEntidades[i][j].numero;
                    AdicionarEntidades(Grades[i].Tiles[tileLocal], Grades[i].Tiles[tileLocal].Entidade);

                }
            }
        }

        public bool TestarConexao()
        {
            try
            {
                connection = new MySqlConnection(datasource);

                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally { connection.Close(); }
        }

        public void EscolherJogador(Jogador jogador)
        {
            try
            {
                connection = new MySqlConnection(datasource);

                string sql = "UPDATE entidade SET personagem_id = @id_atual WHERE id_entidade = @id_entidade";

                MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@id_atual", id_jogador);
                command.Parameters.AddWithValue("@id_entidade", jogador.Id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao escolher jogador:\n" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void RetirarJogador(Jogador jogador)
        {
            try
            {
                string sql = "UPDATE entidade SET personagem_id = NULL WHERE id_entidade = @id_entidade";
                using (connection = new MySqlConnection(datasource))
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_entidade", jogador.Id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao escolher jogador:\n" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void PopularPersonagens()
        {
            Jogadores.Clear();

            connection = new MySqlConnection(datasource);

            string sql = "SELECT id_entidade, nome, imagem FROM entidade WHERE codigo_sala = @codigo AND tipo = 'jogador'";

            MySqlCommand command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@codigo", Codigo);

            connection.Open();

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Jogador jogador = new Jogador(int.Parse(reader["id_entidade"].ToString()), reader["nome"].ToString(), reader["imagem"].ToString());
                Jogadores.Add(jogador);
            }
        }

        public void AdicionarMestre()
        {
            string sql = "CALL adicionar_mestre(@codigo)";

            using (connection = new MySqlConnection(datasource))
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@codigo", Codigo);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id_jogador = Convert.ToInt16(reader["id"]);
                    }
                }
            }
        }

        public void deletarEntidades(Entidades entidades)
        {
            string sql = "CALL mover_entidade(@id_entidade, @localizacao, @codigo)";
            using (connection = new MySqlConnection(datasource))
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id_entidade", entidades.Id);
                command.Parameters.AddWithValue("@localizacao", DBNull.Value);
                command.Parameters.AddWithValue("@codigo", Codigo);

                connection.Open();
                command.ExecuteNonQuery();
            }

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
