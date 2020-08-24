using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowLayouterer.Domain;

namespace WindowLayouterer.Configuration
{
    public class KeyParser
    {
        private Dictionary<string, Keys> KnownKeys = new Dictionary<string, Keys>
        {
            { "PLUS", Keys.Add },
            { "MINUS", Keys.Subtract },
            { "/", Keys.Divide },
            { "[", Keys.OemOpenBrackets },
            { "]", Keys.OemCloseBrackets },
            { "|", Keys.OemPipe },            
            { "ESC", Keys.Escape }
        };

        public virtual HotKey Parse(string key)
        {
            var split = key.Split(' ', '+', '-');
            var k = split[split.Length - 1];

            Keys k1;
            if (KnownKeys.ContainsKey(k.ToUpper()))
                k1 = KnownKeys[k.ToUpper()];
            else if (Regex.IsMatch(k, @"^\d$"))
                k1 = Enum.Parse<Keys>("D" + k);
            else if (k.Length == 1 || (k.Length == 2 && k.StartsWith("f")))
                k1 = Enum.Parse<Keys>(k.ToUpper());
            else
                k1 = Enum.Parse<Keys>(k);

            KeyModifiers m = default;
            for (int i = 0; i < split.Length - 1; i++)
            {
                if (split[i].Equals("Ctrl", StringComparison.InvariantCultureIgnoreCase) || split[i].Equals("Control", StringComparison.InvariantCultureIgnoreCase))
                    m |= KeyModifiers.Control;
                if (split[i].Equals("Alt", StringComparison.InvariantCultureIgnoreCase))
                    m |= KeyModifiers.Alt;
                if (split[i].Equals("Shift", StringComparison.InvariantCultureIgnoreCase))
                    m |= KeyModifiers.Shift;
            }

            return new HotKey
            {
                Key = k1,
                Modifiers = m
            };
        }
    }
}
