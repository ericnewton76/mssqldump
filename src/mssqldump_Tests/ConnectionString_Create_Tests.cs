using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mssqldump;
using NUnit.Framework;
using FluentAssertions;

namespace mssqldump_Tests
{

	[TestFixture]
	public class ConnectionString_Create_Tests
	{

		[Test]
		public void Use_ConnectionString()
		{
			string expected = "$$connstring$$";

			//act
			var options = new TestOptions() {
				ConnectionString = "$$connstring$$"
			};

			var cstring = ConnectionHelper.CreateConnectionString(options);

			//assert
			cstring.Should().Be(expected);
		}

		[Test]
		public void Use_IndividualValues()
		{
			string expectedStr = "Server=$$$server$$$;User ID=$username$;Database=$$database$$" ;
			string[] expectedVals = expectedStr.Split(';');

			//act
			var testVals = new TestOptions() {
				Server = "$$$server$$$",
				LoginId = "$username$",
				Database = "$$database$$"
			};

			var cstring = ConnectionHelper.CreateConnectionString(testVals);

			//assert
			cstring.Split(';').Should().BeEquivalentTo(expectedVals);
		}

		[Test]
		public void Use_ConnectionString_From_Config()
		{
			string expected = "$$$connstring222$$$";

			string assemblyFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

			//act
			var options = new TestOptions() {
				ConnectionString = "key2",
				ConfigFile = System.IO.Path.Combine(assemblyFolder, "App.config")
			};

			var cstring = ConnectionHelper.CreateConnectionString(options);

			//assert
			cstring.Should().Be(expected);
		}

		[Test]
		public void Specific_Options()
		{

		}

		private class TestOptions : BaseOptions
		{

		}

	}
}
