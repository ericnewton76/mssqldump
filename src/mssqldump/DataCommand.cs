using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace mssqldump
{
	public class DataCommand : ICommand
	{
		public DataCommand(DataOptions options)
		{
			this.options = options;
		}

		private DataOptions options;
		private System.IO.TextWriter _writer;
		public void SetWriter(System.IO.TextWriter writer) => this._writer = writer;

		public int Execute()
		{
			string connStr = ConnectionHelper.CreateConnectionString(options);

			using(SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();

				if(options.Query != null)
				{
					SqlCommand cmd = new SqlCommand(options.Query, conn);
					RunSqlCommand(cmd, "(query)");
				}
				else if(options.Tables != null && options.Tables.Count() > 0)
				{
					foreach(var tablename in options.Tables)
					{
						string sql = string.Format("SELECT * FROM {0}", tablename);
						SqlCommand cmd = new SqlCommand(sql, conn);

						RunSqlCommand(cmd, tablename);
					}
				}


			}

			return 0;
		}

		private void RunSqlCommand(SqlCommand cmd, string tablename)
		{
			switch(options.Format)
			{
				case FormatOptions.TSV:
					WriteTsv(cmd, _writer);
					break;

				case FormatOptions.JSON:
					break;

				case FormatOptions.Inserts:
					WriteInserts(cmd, _writer, tablename);
					break;
			}
		}
		

		private void WriteTsv(SqlCommand cmd, System.IO.TextWriter writer)
		{
			using(var dr = cmd.ExecuteReader())
			{
				bool hasNextResult = false;

				do
				{
					bool writtenHeaders = false;

					int fieldCount = dr.FieldCount;

					if(true)
					{
						for(int i = 0; i < fieldCount; i++)
						{
							if(i > 0) _writer.Write('\t');
							_writer.Write(dr.GetName(i));
						}
						_writer.WriteLine();
					}

					while(dr.Read())
					{
						for(int i = 0; i < fieldCount; i++)
						{
							if(i > 0) _writer.Write('\t');
							_writer.Write(dr[i]);
						}
						_writer.WriteLine();
					}

					//write a blank line between resultsets
					hasNextResult = dr.NextResult();
					_writer.WriteLine();

				} while(hasNextResult);
			}
		}

		private void WriteInserts(SqlCommand cmd, System.IO.TextWriter writer, string tablename)
		{
			var InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;

			const int bufferSize = 2^20;
			byte[] buffer = new byte[bufferSize];

			using(var dr = cmd.ExecuteReader())
			{
				bool hasNextResult = false;

				do
				{
					bool writtenHeaders = false;

					int fieldCount = dr.FieldCount;

					if(true)
					{
						_writer.Write("INSERT INTO {0} ", tablename);

						_writer.Write("(");
						for(int i = 0; i < fieldCount; i++)
						{
							if(i > 0) _writer.Write(',');
							_writer.Write(dr.GetName(i));
						}
						_writer.Write(")");
						_writer.WriteLine();
					}

					while(dr.Read())
					{
						_writer.Write("VALUES (");
						for(int i = 0; i < fieldCount; i++)
						{
							if(i > 0) _writer.Write(',');

							if(dr.IsDBNull(i))
								_writer.Write("NULL");
							else
							{
								string x = dr.GetDataTypeName(i);
								switch(dr.GetDataTypeName(i))
								{
									case "int":
									case "long":
									case "":
										_writer.Write(Convert.ToString(dr[i], InvariantCulture));
										break;
									case "varbinary":
										_writer.Write("0x");
										int bytes = 0;
										long dataIndex = 0;
										while((bytes = (int)dr.GetBytes(i, dataIndex, buffer, 0, bufferSize)) > 0)
										{
											_writer.Write(BitConverter.ToString(buffer, 0, bytes).Replace("-", ""));
											dataIndex += bytes;
										}
										break;
									case "nvarchar":
									case "varchar":
										_writer.Write('\'');
										_writer.Write(dr.GetString(i).Replace("'","''"));
										_writer.Write('\'');
										break;
									case "datetime":
									case "datetime2":
										_writer.Write('\'');
										_writer.Write(dr.GetDateTime(i).ToString("O"));
										_writer.Write('\'');
										break;
									case "bit":
										_writer.Write(dr.GetBoolean(i) ? "1" : "0");
										break;
									default:
						System.Diagnostics.Debug.WriteLine("GetDataTypeName=" + x);
										_writer.Write(Convert.ToString(dr[i], InvariantCulture));
										break;
								}
								
							}
						}
						_writer.Write(")");
						_writer.WriteLine();
					}

					//write a blank line between resultsets
					hasNextResult = dr.NextResult();
					_writer.WriteLine();

				} while(hasNextResult);
			}
		}

	}
}
