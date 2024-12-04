using DnDBeyondConnector.Models;

namespace DnDBeyondConnector.Repository
{
    public class DnDBeyondMonsterSearchRepo
    {
        private HttpClient _httpClient;

        public DnDBeyondMonsterSearchRepo(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public List<MonsterStatModel> SearchForMonster(string monsterName)
        {
            Uri monsterUri = new Uri($"https://monster-service.dndbeyond.com/v1/Monster?search={monsterName}&sources=1&skip=0&take=10");
            var response = _httpClient.GetAsync(monsterUri).Result;
            if (response.IsSuccessStatusCode)
            {
                var json =  response.Content.ReadAsStringAsync().Result;
                var result = MonsterStatModelFactory.FromJson(json);
                return result;
            }
            else
            {
                return new List<MonsterStatModel>(); 
            }
        }
    }
}
