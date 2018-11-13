using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A class containing extension methods to allow debugging.
/// </summary>
static class DebugExtensions
{
    public static void Debug(this object o)
    {
        if (Debugger.Launch())
            Debugger.Break();
    }
}