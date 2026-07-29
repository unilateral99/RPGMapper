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

        public static RoutedUICommand CancelarEntidade { get; set; }
        public static RoutedUICommand AdicionarEntidade { get; set; }

        static MyCommands()
        {
            InputGestureCollection Input = new InputGestureCollection();
            KeyGesture InimigoKey = new KeyGesture(Key.I, ModifierKeys.Control);
            Input.Add(InimigoKey);

            Inimigo = new RoutedUICommand("Inimigo", "Inimigo", typeof(MyCommands), Input);

            Imagem = new RoutedUICommand("Imagem", "Imagem", typeof(MyCommands));

            CancelarEntidade = new RoutedUICommand("Cancelar Entidade", "Cancelar Entidade", typeof (MyCommands));
            AdicionarEntidade = new RoutedUICommand("Adicionar Entidade", "Adicionar Entidade", typeof(MyCommands));
        }
    }
}
