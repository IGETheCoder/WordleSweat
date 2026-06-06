namespace WordleSweat.Core
{
    public class QuizStateService
    {
        public GenResult[] Results { get; set; } = [];
        public LetterCell[] CurrentPattern { get; set; } = [];
    }
}
