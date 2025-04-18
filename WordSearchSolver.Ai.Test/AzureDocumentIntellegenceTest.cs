using Microsoft.Extensions.Configuration;

namespace WordSearchSolver.Ai.Test;


public class AzureDocumentIntellegenceTest
{
    public static readonly Credential Azure;
    static AzureDocumentIntellegenceTest()
    {
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder.AddUserSecrets<AzureDocumentIntellegenceTest>()
            .Build();
        Azure = configuration.GetSection("Azure").Get<Credential>()
            ?? throw new ApplicationException("You must define a user secret per the Credential record");
    }
    [Test]
    [Skip("using a different service")]
    public async Task TestExtractWordSearch()
    {
        var azureDocumentIntellegence = new AzureDocumentIntellegence(Azure);
        var image = File.ReadAllBytes("Data/sample_web.png");
        var wordSearch = await azureDocumentIntellegence.ExtractWordSearch(BinaryData.FromBytes(image));
        await Assert.That(wordSearch.Letters).IsNotNull();
        //Assert.That(wordSearch.WordBank, Is.Not.Empty);
    }
}
