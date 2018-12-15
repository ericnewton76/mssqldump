using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mssqldump
{
	[Verb("tables", HelpText="dumps table data")]
	public class DataOptions : BaseOptions
	{
		[Value(0)]
		public IEnumerable<string> Tables { get; set; }

		[Option('q',"query")]
		public string Query { get; set; }

		[Option('f',"format", HelpText="Specifies format to use:TSV (tab separated values) (default), CSV, json, xml")]
		public FormatOptions Format { get; set; }
	}

	public enum FormatOptions
	{
		TSV = 0,
		CSV = 1,
		JSON = 2,
		XML = 3,
		Inserts = 4
	}
}
