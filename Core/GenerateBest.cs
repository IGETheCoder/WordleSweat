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
            return await GenRaw(answers, wordService.Words);
        }

        public static async Task<(string, string)> GenRaw (List<string> unformattedAnswers, List<string> unformattedGuesses)
        {
            string errorCode = "";

            if (unformattedAnswers.Count == 0)
            {
                errorCode = "No more moves left";
                return ("", errorCode);
            }
            // format lists ToUpper
            var answersArray = new string[unformattedAnswers.Count];
            for (int i = 0; i < unformattedAnswers.Count; i++)
                answersArray[i] = unformattedAnswers[i].ToUpper();
            List<string> answers = answersArray.ToList();

            Dictionary<string, long> scores = []; //answer , score

            await Task.Run(() =>
            {
                int bucketSize = (int) Math.Pow(3, answers[0].Length);

                foreach (string w in unformattedGuesses)
                {
                    string word = w.ToUpper();

                    long score = ScoreGuess(word, answers, bucketSize);
                    scores.Add(word, score);

                    Console.WriteLine($"word: {word} score: {score}");
                }
            });

            string finalWord =
                scores.MinBy(entry => entry.Value).Key;

            return (finalWord.ToUpper(), errorCode);
        }
        private static long ScoreGuess (string guess, List<string> remainingAnswers, int bucketSize)
        {
            long[] buckets = new long[bucketSize];

            for (int i = 0; i < buckets.Length; i++)
                buckets[i] = 0; // reset all buckets

            foreach (string answer in remainingAnswers)
            {
                // if (guess == answer) continue;

                int pattern = GetPattern(guess, answer);
                buckets[pattern]++;
            }

            long score = 0;

            foreach (var bucket in buckets)
                score += bucket * bucket;

            return score;
        }
        private static int GetPattern (string guess, string answer)
        {
            Span<byte> pattern = stackalloc byte[guess.Length];
            byte forbiddenIndexs = 0;

            // greens
            for (int i = 0; i < answer.Length; i++)
            {
                if (guess[i] == answer[i])
                {
                    pattern[i] = 2;
                    forbiddenIndexs += (byte) (1 << i);
                }
            }
            // yellows TODO
            for (int i = 0; i < answer.Length; i++)
            {
                if (pattern[i] == 0) // not green
                {
                    for (int j = 0; j < answer.Length; j++)
                    {
                        if ((forbiddenIndexs & (1 << j)) != 0) continue;
                        if (guess[i] != answer[j]) continue;

                        pattern[i] = 1;
                        forbiddenIndexs += (byte) (1 << j);
                        break;
                    }
                }
            }

            int value = 0;
            foreach (byte digit in pattern)
                value = value * 3 + digit;
            return value;
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

                    if (!yellowRepeats.TryGetValue(cellChar, out int value))
                        yellowRepeats.Add(cellChar, 1);
                    else
                        yellowRepeats[cellChar] = ++value;
                }
                else if (cell.State == 2) // green
                {
                    if (word[i] != cellChar)
                        return false;
                    forbiddenIndexs += (byte) (1 << i);
                }
                else // grey
                {
                    for (int j = 0; j < filter.Length; j++)
                    {
                        if (word[j] == cellChar)
                            return false;
                    }
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

        private static bool IsEnglishLetter (char c)
        {
            c = char.ToUpper(c);
            return c >= 'A' && c <= 'Z';
        }
    }
}
