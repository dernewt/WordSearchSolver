using Microsoft.Extensions.Configuration;

namespace WordSearchSolver.Ai.Test;

public class AzureOpenAiTest
{
    public static readonly Credential Config;
    static AzureOpenAiTest()
    {
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.AddUserSecrets<AzureOpenAiTest>()
            .Build();
        Config = configuration.GetSection("Azure").Get<Credential>()
            ?? throw new ApplicationException("You must define a user secret per the Credential record");
    }
    [Test]
    public async Task TestExtractWordSearch()
    {
        var azure = new AzureOpenAi(Config);
        var image = File.ReadAllBytes("Data/sample_web.png");
        var wordSearch = await azure.ExtractWordSearch(BinaryData.FromBytes(image));
        await Assert.That(wordSearch.Letters).IsNotNull();
        //Assert.That(wordSearch.WordBank, Is.Not.Empty);
    }
}
