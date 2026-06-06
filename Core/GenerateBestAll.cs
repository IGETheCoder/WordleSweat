namespace WordleSweat.Core
{
    public static class GenerateBestAll
    {
        public static event Action? OnProgress;

        public static async Task<string> GenerateAll (LetterCell[] inputs, WordListService wordService, GenResult[] results)
        {
            string errorCode = "";
            LetterCell[] internalInput = new LetterCell[inputs.Length];

            string wordLower = "";
            for (int j = 0; j < inputs.Length; j++)
            {
                LetterCell cell = inputs[j];
                if (cell.Value.Length == 0)
                {
                    errorCode = "Incomplete input";
                    return errorCode;
                }
                if (!GenerateBest.IsEnglishLetter(cell.Value[0]))
                {
                    errorCode = "Letters must be 'A-Z'";
                    return errorCode;
                }

                internalInput[j] = new LetterCell() { Value = cell.Value, State = 0 };
                wordLower += cell.Value.ToLower()[0];
            }
            if (!wordService.Words.Contains(wordLower))
            {
                errorCode = "Starting word is not valid";
                return errorCode;
            }

            int max = (int) Math.Pow(3, internalInput.Length);
            for (int i = 0; i < max; i++)
            {
                int temp = i;

                for (int d = internalInput.Length - 1; d >= 0; d--)
                {
                    internalInput[d].State = temp % 3;
                    temp /= 3;
                }

                List<string> answers = await Task.Run(() => GenerateBest.FilterWords(wordService.Words, internalInput));
                // format answers list
                var answersArray = new int[answers.Count][];
                for (int j = 0; j < answers.Count; j++)
                    answersArray[j] = GenerateBest.ConvertStringToIntArr(answers[j].ToUpper());

                (GenResult result, errorCode) = await GenerateBest.GenerateRaw(answersArray, wordService.Words, internalInput);
                if (errorCode != "No more moves left")
                {
                    results[i] = result;
                    OnProgress?.Invoke();
                }
                else
                    errorCode = "";
            }
            return errorCode;
        }
    }
}
