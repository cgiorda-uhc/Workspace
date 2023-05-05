
using Autofac.Core;
using Autofac.Extras.Moq;
using IdentityModel.OidcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using ProjectManagerLibrary.Projects;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace XUnitParserTesting.Logic
{
    public class DelimitedParserTests //: IClassFixture<IDelimitedParser> //: IClassFixture<WebDriverFixture>
    {
        //READ appsettings.json
        private readonly IConfiguration _configuration;

        private readonly ITestOutputHelper? _testOutputHelper;
        //USED TO INJECT SELENIUM
        private readonly WebDriverFixture? _webDriverFixture;


        //ADD TO CONFIG SOLUTION?????
        private static string _strTestConnectionString = "";
        private static string _strTestSchema = "";
        private static int _intTestBulkSize = 100;
        private static string _strEnvironment = "";
        private static string _strFilePath = "";

        public IHost? TestHost { get; }


        public DelimitedParserTests()
        {
 
            _configuration = InitConfiguration();
            _strTestConnectionString = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
            _strTestSchema = _configuration.GetValue<string>("OtherSettings:Schema");
            _intTestBulkSize = _configuration.GetValue<int>("OtherSettings:BulkSize");
            _strEnvironment = _configuration.GetValue<string>("OtherSettings:Environment");
            _strFilePath = _configuration.GetValue<string>("OtherSettings:FilePath");
            //Task.Run(() => TestHost.RunAsync());

        }


        //public DelimitedParserTests(IConfiguration configuration, ITestOutputHelper testOutputHelper, WebDriverFixture webDriverFixture = null)
        //{
        //    _configuration = configuration;
        //    TestHost = CreateHostBuilder().Build();


        //    _testOutputHelper = testOutputHelper;
        //    _webDriverFixture = webDriverFixture;

        //    var s = _configuration.GetValue<string>("ConnectionStrings.DefaultConnection");



        //    //Task.Run(() => TestHost.RunAsync());

        //}

        //public static IHostBuilder CreateHostBuilder() =>
        // Host.CreateDefaultBuilder()
        //     .ConfigureAppConfiguration((hostingContext, _configuration) =>
        //     {
        //         _configuration.AddJsonFile("appsettings.json", optional: true);
        //         _configuration.AddEnvironmentVariables();
        //     });
        //.ConfigureServices((hostContext, services) =>
        //{
        //    services.AddOptions();
        //    services.Configure<AppConfiguration>(hostContext.Configuration.GetSection("AppConfiguration"));

        //    services.AddTransient<IServiceX, ServiceX>();
        //    services.AddTransient<ITemplateEngine, TemplateEngine>();

        //    //removed for brevity..
        //})
        //.ConfigureLogging((hostingContext, logging) =>
        //{
        //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        //    logging.AddConsole();
        //});
        public static IConfiguration InitConfiguration()
        {
            //var config = new ConfigurationBuilder()
            //   .AddJsonFile("appsettings.json")
            //   .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            //    .AddEnvironmentVariables()
            //    .Build();
            //    return config; 
            IConfigurationBuilder builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory) NEEDED FOR MANUAL SUPPORT OF JSON
                .AddJsonFile("appsettings.json");

#if DEBUG
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
            builder.AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true);
#endif

           builder.AddEnvironmentVariables();
           return builder.Build();

        }


        [Theory]
        [MemberData(nameof(DelimitedTestData))]
        public void parseDelimitedFiles_shouldGenerateTables (string strTestPath, string strTableName, char chrTestDelimiter, string strTestConnectionString, string strTestSchemaName, int intTestBulkSize, List<string> lstExpectedColumnNames, int intExpectedRowCnt)
        {
            //ARRANGE
            //ARRANGE
            //ARRANGE
            //concrete approach
            //DelimitedParser dp = new DelimitedParser();
            //dp.parseDelimitedFiles(strTestPath, chrTestDelimiter, strTestConnectionString, strTestSchemaName, intTestBulkSize);
            //abstract approach using Moq
            var mock = new Mock<IDelimitedParser>(MockBehavior.Strict);
            mock.Setup(f => f.parseDelimitedFiles(strTestPath, chrTestDelimiter, strTestConnectionString, strTestSchemaName, intTestBulkSize, 10000));
            //var tableName = mock.Object.TableName;


            //ACT
            //ACT
            //ACT
            var actualRowCnt = CheckResults.GetRowCounts(mock.Object, strTableName);
            var actualColCnt = CheckResults.GetColumnCounts(mock.Object, strTableName);
            var actualColNames = CheckResults.GetColumnNames(mock.Object, strTableName);

            //ASSERT
            //ASSERT
            //ASSERT
            Assert.Equal(actualRowCnt, intExpectedRowCnt);
            Assert.Equal(actualColCnt, lstExpectedColumnNames.Count);
            Assert.Equal(actualColNames, lstExpectedColumnNames);
            mock.VerifyAll();

        }
        public static IEnumerable<object[]> DelimitedTestData()
        {
            yield return new object[] { _strFilePath + @"\comma_in_quotes.csv", "COMMA_IN_QUOTES", ',',  _strTestConnectionString, _strTestSchema, _intTestBulkSize, new List<string> { "first", "last", "address", "city", "zip" }, 1 };
            yield return new object[] { _strFilePath + @"\empty.csv", "EMPTY",',', _strTestConnectionString, _strTestSchema, _intTestBulkSize, new List<string> { "a", "b", "c" }, 2 };
            yield return new object[] { _strFilePath + @"\escaped_quotes.csv","ESCAPED_QUOTES", ',', _strTestConnectionString, _strTestSchema, _intTestBulkSize, new List<string> { "a", "b" }, 2 };
        }


        class CheckResults
        {
            public static int GetRowCounts(IDelimitedParser dp, string tableName)
            {
                dp.TableName = tableName;
                return dp.getRowCount(_strTestConnectionString, _strTestSchema);
            }

            public static int GetColumnCounts(IDelimitedParser dp, string tableName)
            {
                dp.TableName = tableName;
                return dp.getColumnCount(_strTestConnectionString, _strTestSchema);
            }


            public static List<string?>? GetColumnNames(IDelimitedParser dp, string tableName)
            {
                dp.TableName = tableName;
                return dp.getColumnNames(_strTestConnectionString, _strTestSchema);
            }
        }



        //MOQ TEST SAMPLE 
        // [Fact]
        public void MockSample()
        {
            using (var mock = AutoMock.GetLoose())
            { }

        }

        //SELENIUM TEST SAMPLE USING IClassFixture<WebDriverFixture> INJECTION
       // [Fact]
        public void SeleniumSample()
        {
            _testOutputHelper.WriteLine("First test");
            _webDriverFixture.ChromeDriver.Navigate().GoToUrl("http://www.uhc.com");
        }

    }
}
