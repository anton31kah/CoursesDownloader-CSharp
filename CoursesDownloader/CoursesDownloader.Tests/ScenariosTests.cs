using System.IO;
using System.Text;
using System.Threading.Tasks;
using CoursesDownloader.AdvancedIO.ConsoleHelpers;
using CoursesDownloader.Downloader.Implementation;
using CoursesDownloader.Tests.Helpers;
using Xunit;

namespace CoursesDownloader.Tests
{
    public class ScenariosTests
    {
        /// <summary>
        /// Tests should be written in the following way: each test will have its own config, after the run, it'll check the output
        /// Tests should cover NOT api calls, neither test the library, but test the whole program (integration tests)
        /// </summary>
        
        private StringBuilder StringWriterSb { get; set; }
        private StringReader StringReader { get; set; }
        private StringWriter StringWriter { get; set; }

        private void Init(string input = "")
        {
            StringReader = new StringReader(input);
            StringWriterSb = new StringBuilder();
            StringWriter = new StringWriter(StringWriterSb);

            ConsoleUtils.MockConsole(StringReader, StringWriter);
        }

        [Fact]
//        [Fact(Skip = "Not a test, just an auto runner with specific input")]
        public async Task BasicRun()
        {
            var input = new InputBuilder()
                .WithAction(InputBuilder.ActionWord.Add, "2", InputBuilder.Confirmation.Yes, InputBuilder.Confirmation.No) // add skit
                .WithAction(InputBuilder.ActionWord.Download) // start download
                .WithSingleOption(2) // select courses naming
                .WithConfirmation(InputBuilder.Confirmation.No) // no repeat
                .GetResult();

            Init(input);

            await CoursesDownloaderManual.Init();
            await CoursesDownloaderManual.Run();
        }

        [Fact]
        public async Task DownloadMultipleLinks()
        {
            var input = new InputBuilder()
                .WithAction(InputBuilder.ActionWord.SwitchSemester)
                .WithSingleOption(5) // switch to semester 5
                .WithSingleOption(7) // open mpip
                .WithAction(InputBuilder.ActionWord.Add, "4,14", InputBuilder.Confirmation.Yes, InputBuilder.Confirmation.No) // add exercises 1 and labs (external links, pages, files)
                .WithAction(InputBuilder.ActionWord.Home) // go home
                .WithSingleOption(5) // open mis
                .WithSingleOption(5) // open course materials
                .WithAction(InputBuilder.ActionWord.Add, "2", InputBuilder.Confirmation.Yes, InputBuilder.Confirmation.No) // add homework assignments (folder)
                .WithAction(InputBuilder.ActionWord.Download) // start download
                .WithSingleOption(2) // select courses naming
                .WithConfirmation(InputBuilder.Confirmation.No) // no repeat
                .GetResult();
            
            Init(input);

            await CoursesDownloaderManual.Init();
            await CoursesDownloaderManual.Run();

            var output = StringWriterSb.ToString(); // new StringReader(StringWriterSb.ToString());
            // TODO DEBUG TEST NOW
            Assert.True(true);
        }
    }
}
