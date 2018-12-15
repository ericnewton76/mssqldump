using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mssqldump
{
	public interface ICommand
	{
		int Execute();
	}
}
