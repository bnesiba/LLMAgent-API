namespace LLMAgentApi.DataStorage.StatblockStorage
{
    public class StatblockStorage
    {
        private Dictionary<string, MonsterStatblock> statblockStorage = new Dictionary<string, MonsterStatblock>();

        public StatblockStorage() { }

        public string InsertStatblock(MonsterStatblock monsterStatblock)
        {
            var newId = Guid.NewGuid().ToString();
            statblockStorage.Add(newId, monsterStatblock);
            return newId;
        }

        public MonsterStatblock GetStatblock(string id)
        {
            if (statblockStorage.ContainsKey(id))
            {
                return statblockStorage[id];
            }
            return new MonsterStatblock();
            
        }
    }
}
