using System;
using the_depot.Interfaces;

namespace the_depot
{
	public static class OutputStatic
	{
		public static IConsole Output { get; set; } = new ConsoleOutput();
	}
}

