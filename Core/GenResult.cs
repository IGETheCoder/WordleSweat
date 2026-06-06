namespace WordleSweat.Core
{
    public class GenResult (string word, double priority, bool valid = true)
    {
        public string word = word;
        public double priority = priority;
        public bool valid = valid;

        public GenResult () : this(string.Empty, 0d, false) { }
    }
}
