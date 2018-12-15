using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mssqldump
{
	public class BaseOptions
	{

		[Option('c',"ConnectionString", HelpText="Connection string to use.  If --config is specified, this can be the name of the connectionstring to retrieve.")]
		public string ConnectionString { get; set; }

		[Option("config", HelpText="Specifies a config file to load a connectionstring from. Used with --Name option.")]
		public string ConfigFile { get; set; }

		[Option('S',"server")]
		public string Server { get; set; }

		[Option('d',"database")]
		public string Database { get; set; }

		[Option('E')]
		public bool? TrustedConnection { get; set; }

		[Option('U')]
		public string LoginId { get; set; }

		[Option('P')]
		public string Password { get; set; }


	}
}
