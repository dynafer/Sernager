namespace Sernager.Terminal.Managers;

internal static class PossibilityManager
{
    internal static string[] Find(string value, params string[] searchables)
    {
        if (value.Length == 0)
        {
            return searchables;
        }

        const byte BEST_POSSIBILITY = 90;
        const byte ALTERNATIVE_POSSIBILITY = 50;

        List<string> possibles = new List<string>();
        List<string> alternatives = new List<string>();
        List<byte> possibilities = new List<byte>();

        foreach (string searchable in searchables)
        {
            byte possibility = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                if (searchable.Length <= i)
                {
                    break;
                }

                if (searchable[i] == value[i])
                {
                    ++possibility;
                }
            }

            possibility = (byte)(possibility / value.Length * 100);
            possibilities.Add(possibility);

            if (possibility >= BEST_POSSIBILITY)
            {
                possibles.Add(searchable);
            }

            if (possibility >= ALTERNATIVE_POSSIBILITY)
            {
                alternatives.Add(searchable);
            }
        }

        if (possibles.Count != 0)
        {
            return possibles.ToArray();
        }

        if (alternatives.Count > 0)
        {
            return alternatives.ToArray();
        }

        return searchables;
    }
}
