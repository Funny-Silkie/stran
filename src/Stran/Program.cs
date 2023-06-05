using CuiLib;
using Stran.Commands;
using System;

namespace Stran
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var command = new MainCommand();

            if (args.Length == 0)
            {
                command.WriteHelp(SR.StdOut);
                return;
            }

            try
            {
                command.Invoke(args);
            }
            catch (ArgumentAnalysisException e)
            {
                SR.StdErr.WriteError(e.Message);
            }
            catch (Exception e)
            {
                SR.StdErr.WriteError(e);
            }
        }
    }
}
