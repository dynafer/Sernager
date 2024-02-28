namespace Sernager.Terminal.Prompts.Plugins.Utilities;

internal readonly record struct PaginationRange(int Start, int End, int Prev, int Next);