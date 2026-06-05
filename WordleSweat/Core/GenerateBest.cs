namespace WordleSweat.Core
{
    internal static class GenerateBest
    {
        public static async Task<(string, string)> Gen (LetterCell[] inputs, WordListService wordService)
        {
            string errorCode = "";

            foreach (var cell in inputs)
            {
                if (cell.Value.Length == 0)
                {
                    errorCode = "Incomplete input";
                    return ("", errorCode);
                }
                if (!IsEnglishLetter(cell.Value[0]))
                {
                    errorCode = "Letters must be 'A-Z'";
                    return ("", errorCode);
                }
            }

            List<string> answers = await Task.Run(() => FilterWords(wordService.Words, inputs));
            return await GenRaw(answers);
        }

        public static async Task<(string, string)> GenRaw (List<string> answers)
        {
            string errorCode = "";

            if (answers.Count == 0)
            {
                errorCode = "No more moves left";
                return ("", errorCode);
            }

            Dictionary<string, long> scores = []; //answer , score

            await Task.Run(() =>
            {
                foreach (string word in answers)
                {
                    long score = ScoreGuess(word, answers);
                    scores.Add(word, score);
                }
            });

            string finalWord =
                scores.MaxBy(entry => entry.Value).Key;

            return (finalWord.ToUpper(), errorCode);
        }
        private static long ScoreGuess (string guess, List<string> remainingAnswers)
        {
            Dictionary<int, long> buckets = []; //pattern , score

            // fancy stuff

            return 0;
        }

        private static bool IsEnglishLetter (char c)
        {
            c = char.ToUpper(c);
            return c >= 'A' && c <= 'Z';
        }
        private static List<string> FilterWords (List<string> guesses, LetterCell[] filter)
        {
            List<string> filtered = [];

            foreach (string guess in guesses)
            {
                if (WordIsValid(guess, filter))
                    filtered.Add(guess);
            }

            return filtered;
        }
        public static bool WordIsValid (string word, LetterCell[] filter)
        {
            word = word.ToUpper();

            Dictionary<char, int> yellowRepeats = [];
            byte forbiddenIndexs = 0;

            for (int i = 0; i < filter.Length; i++)
            {
                LetterCell cell = filter[i];
                char cellChar = cell.Value[0];

                if (cell.State == 1) // yellow
                {
                    if (word[i] == cellChar)
                        return false;

                    if (!yellowRepeats.ContainsKey(cellChar))
                        yellowRepeats.Add(cellChar, 1);
                    else
                        yellowRepeats[cellChar]++;
                }
                else if (cell.State == 2) // green
                {
                    if (word[i] != cellChar)
                        return false;
                    forbiddenIndexs += (byte) (1 << i);
                }
                else // grey
                {
                    if (word[i] == cellChar)
                        return false;
                }
            }

            if (yellowRepeats.Count == 0)
                return true;

            foreach (var pair in yellowRepeats)
            {
                char targetChar = pair.Key;
                int targetCount = pair.Value;
                int count = 0;
                for (int i = 0; i < word.Length; i++)
                {
                    if ((forbiddenIndexs & (1 << i)) != 0) continue;

                    if (word[i] == targetChar)
                    {
                        count++;
                        if (count >= targetCount)
                            break;
                    }
                }
                if (count < targetCount)
                    return false;
            }

            return true;
        }
    }
}
