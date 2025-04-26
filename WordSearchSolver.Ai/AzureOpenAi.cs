using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using WordSearchSolver.Core;

namespace WordSearchSolver.Ai;

public record Credential(string Key, string Endpoint, string ModelId);

public class AzureOpenAi

{
    protected Kernel kernel;
    public AzureOpenAi(Credential configuration)
    {
        var kernelBuilder = Kernel.CreateBuilder();

        kernelBuilder.Services.AddAzureOpenAIChatCompletion(
            configuration.ModelId,
            configuration.Endpoint,
            configuration.Key);

        kernel = kernelBuilder.Build();


    }
    public class Solution
    {
        public required string Word { get; set; }

        public required int RowStart { get; set; }

        public required int ColumnStart { get; set; }

        public required int RowEnd { get; set; }

        public required int ColumnEnd { get; set; }
    }
    public class WordSearchJson
    {
        public required string[] Puzzle { get; set; }
        public required string[] WordBank { get; set; }
        public required Solution[] Solutions { get; set; }
    }
    public async Task<WordSearch> ExtractWordSearch(BinaryData image)
    {
        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var settings = new OpenAIPromptExecutionSettings()
        {
            MaxTokens = 1000,
            Temperature = 0,
            ResponseFormat = typeof(WordSearchJson),
        };

        var history = new ChatHistory();

        history.AddSystemMessage("You are an expert word search and word seek puzzler. Your task is to extract key information from the provided puzzle. Format your response as a JSON object, where the keys are the data point names and the values are the extracted information. Do not include any explanatory text or comments in your response, just the raw JSON. Process it carefully and provide the JSON output.");

        history.AddUserMessage([
            new TextContent("This is the puzzle"),
            new ImageContent(image, "image/png")]);

        var result = await chat.GetChatMessageContentAsync(history, settings)
            ?? throw new Exception();

        if (result.Content == null)
            throw new Exception();

        var model = JsonSerializer.Deserialize<WordSearchJson>(result.Content)
            ?? throw new Exception("Failed to deserialize the response");

        //https://medium.com/@johnidouglasmarangon/generating-structured-outputs-from-pdfs-with-semantic-kernel-and-gemini-aa4d4382e339
        return new WordSearch()
        {
            Letters = new(model.Puzzle),
            WordBank = model.WordBank
        };
    }


}
