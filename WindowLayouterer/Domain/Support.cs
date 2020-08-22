using System;
using System.Collections.Generic;
using System.Text;

namespace WindowLayouterer.Domain
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}
