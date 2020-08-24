using System.Windows.Forms;

namespace WindowLayouterer.Domain
{
    public class HotKey
    {
        public KeyModifiers Modifiers { get; set; }
        public Keys Key { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HotKey key &&
                   Modifiers == key.Modifiers &&
                   Key == key.Key;
        }
    }
}
