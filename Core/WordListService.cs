using System.Net.Http.Json;

namespace WordleSweat.Core
{
    public class WordListService (HttpClient http)
    {
        public List<string> Words { get; private set; } = [];

        public async Task LoadAsync ()
        {
            Words = await http.GetFromJsonAsync<List<string>>("data/wordle_words.json")
                     ?? [];
        }
    }
}
