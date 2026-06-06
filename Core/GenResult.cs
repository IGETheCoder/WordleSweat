namespace WordleSweat.Core
{
    public class GenResult (string word, double priority, LetterCell[] input, bool valid = true)
    {
        public string Word { get; } = word;
        public double Priority { get; } = priority;
        public LetterCell[] Input { get; } = [.. input
            .Select(c => new LetterCell
            {
                Value = c.Value,
                State = c.State
            })];

        public bool Valid { get; } = valid;

        public GenResult () : this(string.Empty, 0d, [], false) { }
    }
}