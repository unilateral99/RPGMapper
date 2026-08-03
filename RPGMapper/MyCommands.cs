using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RPGMapper
{
    static class MyCommands
    {
        public static RoutedUICommand Imagem {  get; set; }

        public static RoutedUICommand Inimigo { get; set; }
        public static RoutedUICommand Jogador { get; set; }
        public static RoutedUICommand Npc {  get; set; }
        public static RoutedUICommand Evento { get; set; }

        public static RoutedUICommand Mapa {  get; set; }

        public static RoutedUICommand CancelarEntidade { get; set; }
        public static RoutedUICommand AdicionarEntidade { get; set; }

        static MyCommands()
        {
            InputGestureCollection InputE = new InputGestureCollection();
            KeyGesture KeyG = new KeyGesture(Key.I, ModifierKeys.Control);
            InputE.Add(KeyG);

            Inimigo = new RoutedUICommand("Inimigo", "Inimigo", typeof(MyCommands), InputE);

            InputGestureCollection InputJ = new InputGestureCollection();
            KeyG = new KeyGesture (Key.J, ModifierKeys.Control);
            InputJ.Add(KeyG);

            Jogador = new RoutedUICommand("Jogador", "Jogador", typeof(MyCommands), InputJ);

            InputGestureCollection InputN = new InputGestureCollection();
            KeyG = new KeyGesture(Key.N, ModifierKeys.Control);
            InputN.Add(KeyG);

            Npc = new RoutedUICommand("Npc", "Npc", typeof(MyCommands), InputN);

            InputGestureCollection InputM = new InputGestureCollection();
            KeyG = new KeyGesture(Key.M, ModifierKeys.Control);
            InputM.Add(KeyG);

            Mapa = new RoutedUICommand("Mapa", "Mapa", typeof(MyCommands), InputM);

            InputGestureCollection InputEv = new InputGestureCollection();
            KeyG = new KeyGesture(Key.V, ModifierKeys.Control);
            InputEv.Add(KeyG);

            Evento = new RoutedUICommand("Evento", "Evento", typeof(MyCommands), InputEv);

            Imagem = new RoutedUICommand("Imagem", "Imagem", typeof(MyCommands));

            CancelarEntidade = new RoutedUICommand("Cancelar Entidade", "Cancelar Entidade", typeof (MyCommands));
            AdicionarEntidade = new RoutedUICommand("Adicionar Entidade", "Adicionar Entidade", typeof(MyCommands));
        }
    }
}
