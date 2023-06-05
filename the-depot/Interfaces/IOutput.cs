using System;
namespace the_depot.Interfaces
{

    public class ConsoleOutput : IConsole
    {
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string ReadKey()
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }


    public interface IConsole
	{

		void WriteLine(string message);
        string ReadKey();
        string ReadLine();
        void Clear();

    }
}

