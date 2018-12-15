using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mssqldump
{
	public static class ConnectionHelper
	{
		public static string CreateConnectionString(BaseOptions options)
		{
			if(options.ConnectionString != null && options.ConfigFile == null)
				return options.ConnectionString;

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
