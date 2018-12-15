using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace mssqldump
{
	public static class ConnectionHelper
	{
		public static string CreateConnectionString(BaseOptions options)
		{
			if(options.ConnectionString != null)
			{
				if(options.ConfigFile == null)
					return options.ConnectionString;
				else
				{
					XDocument xdoc = null;

					using(var fs = System.IO.File.OpenRead(options.ConfigFile))
					{
						xdoc = XDocument.Load(fs);
					}

					XElement configurationEl = xdoc.Element("configuration");
					if(configurationEl == null)
						configurationEl = xdoc.Root;

					XElement connectionStringsEl = configurationEl.Element("connectionStrings");

					XElement connStrEl = connectionStringsEl.Elements("add")
						.FirstOrDefault(_ => _.Attribute("name")?.Value == options.ConnectionString);

					if(connStrEl != null)
					{
						var connStr = connStrEl.Attribute("connectionString")?.Value;
						return connStr;
					}
				}
			}

			List<string> csb = new List<string>();

			if(options.Server != null)
				csb.Add(string.Format("Server={0}", options.Server));

			if(options.Database != null)
				csb.Add(string.Format("Database={0}", options.Database));

			if(options.LoginId != null)
				csb.Add(string.Format("User ID={0}", options.LoginId));

			if(options.Password != null)
				csb.Add(string.Format("Password={0}", options.Password));

			if(options.TrustedConnection.GetValueOrDefault(false))
			{
				csb.Add(string.Format("Integrated Security=true"));
			}

			if(csb.Count == 0)
			{
				//look in environment variables
				return Environment.GetEnvironmentVariable("MSSQL_CONN");
			}
			return String.Join(";", csb);
		}
	}
}
