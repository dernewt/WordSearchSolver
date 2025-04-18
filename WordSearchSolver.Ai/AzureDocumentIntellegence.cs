using Azure;
using Azure.AI.DocumentIntelligence;
using System.Text;
using WordSearchSolver.Core;

namespace WordSearchSolver.Ai;

public record Credential(string Key, string Endpoint);

public class AzureDocumentIntellegence
{
    protected DocumentIntelligenceClient client;

    public AzureDocumentIntellegence(Credential configuration)
    {
        var credential = new AzureKeyCredential(configuration.Key);
        client = new DocumentIntelligenceClient(new Uri(configuration.Endpoint), credential);
    }

    public async Task<WordSearch> ExtractWordSearch(BinaryData image)
    {
        var result = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", image);

        var lines = result.Value
            .Pages?[0]
            ?.Lines
            ?? throw new ArgumentException("Failed to understand image");

        var gridText = new StringBuilder();
        var bank = new List<string>();

        foreach (var line in lines)
        {
            var text = line.Content.Trim();

            if (text.Length > 20)
            {
                gridText.AppendLine(text);
            }
            else
            {
                bank.AddRange(text.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        return new WordSearch()
        {
            Letters = new LetterGrid(gridText.ToString()),
            WordBank = bank
        };
    }
}
