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
        public static RoutedUICommand Inimigo { get; set; }

        static MyCommands()
        {
            InputGestureCollection Input = new InputGestureCollection();
            KeyGesture InimigoKey = new KeyGesture(Key.I, ModifierKeys.Control);
            Input.Add(InimigoKey);

            Inimigo = new RoutedUICommand("Inimigo", "Inimigo", typeof(MyCommands), Input);
        }
    }
}
