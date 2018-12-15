using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;

namespace mssqldump
{
    class Program
    {
		static int Main(string[] args)
		{
			ICommand command = null;

			int exitcode = 
				CommandLine.Parser.Default.ParseArguments<DataOptions,BogusOptions>(args)
					.MapResult(
						(DataOptions options) => {
							DataCommand cmd = new DataCommand(options);
							cmd.SetWriter(Console.Out);
							command = cmd;
							return 0;
						},
						(BogusOptions options) => {
							return -1;
						},
						errs => 1
					);

			if(exitcode == 0)
			{
				try
				{
					exitcode = command.Execute();
				}
				catch(Exception ex)
				{
					Console.Error.WriteLine("Failed:\n" + ex.ToString());
					exitcode = 2;
				}
			}

			return exitcode;
		}
    }
}
