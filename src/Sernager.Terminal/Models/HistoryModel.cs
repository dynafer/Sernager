using Sernager.Terminal.Models.Histories;

namespace Sernager.Terminal.Models;

internal sealed class HistoryModel
{
    internal Guid Id { get; private init; }
    private IHistoryPromptModel mPromptModel { get; init; }
    private HistoryResultHandler mResultHandler { get; init; }

    internal HistoryModel(IHistoryPromptModel promptModel, HistoryResultHandler resultHandler)
    {
        Id = Guid.NewGuid();
        mPromptModel = promptModel;
        mResultHandler = resultHandler;
    }

    internal void RunWithPrompt()
    {
        object result = mPromptModel.Prompt();
        mResultHandler(result);
    }

    internal void RunWithResult(object result)
    {
        mResultHandler(result);
    }
}
