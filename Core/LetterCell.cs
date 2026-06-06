namespace WordleSweat.Core
{
    public class LetterCell
    {
        public string Value { get; set; } = "";
        public int State { get; set; } = 0; // 0=grey, 1=yellow, 2=green
    }
}
