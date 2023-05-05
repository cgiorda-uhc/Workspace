

namespace VCPortal_UnitTests.ChemoPx;
public class ChemotherapyPXGridTests
{
    //[Fact]
    //public void GridShouldPopulateOnLoad()
    //{

    //    //Arrange
    //    var ctx = new TestContext();

    //    //var mock = ctx.Services.AddMockHttpClient();
    //    //mock.When("/proc_codes").RespondJson(new List<ProcCodesModel> 
    //    //{
    //    //    new ProcCodesModel { Proc_Cd  = "G8760", Proc_Desc = "ALL QUAL ACTIONS EPILEPSY MSR GRP PRFORM THIS PT", Proc_Cd_Type = "HCPCS", Proc_Cd_Date = DateTime.Parse("12/31/9999")},
    //    //    new ProcCodesModel { Proc_Cd  = "0D9K80Z", Proc_Desc = "DRAINAGE ASC COLON DRAIN DEVC NAT/ART OP ENDO", Proc_Cd_Type = "ICD10", Proc_Cd_Date = DateTime.Parse("12/31/9999")},
    //    //    new ProcCodesModel { Proc_Cd  = "33417", Proc_Desc = "AORTOPLASTY SUPRAVALVULAR STENOSIS", Proc_Cd_Type = "CPT-4", Proc_Cd_Date = DateTime.Parse("12/31/9999")},

    //    //});


    //    //CONVERT HTTPCLIENT TO CLIENTFACTORY VIA EXTENSION METHOD
    //    //IHttpClientFactory factory = mock.ToHttpClientFactory();

    //    //mock.When("/chemotherapypx").RespondJson(new List<ProcCodesModel>
    //    //{
    //    //    new ProcCodesModel { Proc_Cd  = "G8760", Proc_Desc = "ALL QUAL ACTIONS EPILEPSY MSR GRP PRFORM THIS PT", Proc_Cd_Type = "HCPCS", Proc_Cd_Date = DateTime.Parse("12/31/9999")},
    //    //    new ProcCodesModel { Proc_Cd  = "0D9K80Z", Proc_Desc = "DRAINAGE ASC COLON DRAIN DEVC NAT/ART OP ENDO", Proc_Cd_Type = "ICD10", Proc_Cd_Date = DateTime.Parse("12/31/9999")},
    //    //    new ProcCodesModel { Proc_Cd  = "33417", Proc_Desc = "AORTOPLASTY SUPRAVALVULAR STENOSIS", Proc_Cd_Type = "CPT-4", Proc_Cd_Date = DateTime.Parse("12/31/9999")},

    //    //});
    //    //ctx.Services.AddScoped<IVCPortal_Services,VCPortal_Services>();
    //    //ctx.Services.AddScoped<IChemotherapyPX_Services,ChemotherapyPX_Services> ();

    //    //ctx.Services.AddSingleton<IVCPortal_Services>(new VCPortal_Services(factory));
    //    //ctx.Services.AddSingleton<IChemotherapyPX_Services>(new ChemotherapyPX_Services(factory));


    //    ctx.Services
    //    .AddHttpClient<ChemotherapyPX_Services>("ChemotherapyPX_Services", client => client.BaseAddress = new Uri("https://localhost:7129"));

    //    ctx.Services
    //        .AddHttpClient<VCPortal_Services>("VCPortal_Services", client => client.BaseAddress = new Uri("https://localhost:7129"));

    //    //.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
    //    //{
    //    //    TimeSpan.FromSeconds(1),
    //    //    TimeSpan.FromSeconds(5)
    //    //}));

    //    ctx.Services.AddTransient<IChemotherapyPX_Services, ChemotherapyPX_Services>();
    //    //builder.Services.AddTransient<MHPUniverse_Services>();
    //    ctx.Services.AddTransient<IVCPortal_Services, VCPortal_Services>();
    //    ctx.Services.AddSingleton<IVCPortal_Globals, VCPortal_Globals>();
    //    ctx.Services.AddLogging();






    //    var cut = ctx.RenderComponent<ChemotherapyPXGrid>();







    //    //await cut.Instance
    //    var result = cut.Instance.GridRef;

    //    //cut.MarkupMatches("<p>Current count: 0</p>");

    //    //cut.Find("p").MarkupMatches("<p>Current count: 0</p>");

    //    // Act
    //    //var element = cut.Find("button");
    //    //element.Click();

    //    ////Assert
    //    //cut.Find("p").MarkupMatches("<p>Current count: 1</p>");


    //}


}
