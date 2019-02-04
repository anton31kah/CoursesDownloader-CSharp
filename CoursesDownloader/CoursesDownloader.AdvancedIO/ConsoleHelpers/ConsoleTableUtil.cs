using System.Linq;
using ConsoleTables;

namespace CoursesDownloader.AdvancedIO.ConsoleHelpers
{
    public class ConsoleTableUtil
    {
        private ConsoleTable Table { get; }

        public ConsoleTableUtil(params string[] headers)
        {
            Table = new ConsoleTable(headers.ToArray());
        }

        public void AddRow(params object[] elements)
        {
            Table.AddRow(elements);
        }

        public override string ToString()
        {
            return Table.ToMarkDownString();
        }
    }
}
