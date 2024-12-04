namespace LLMAgentApi.DataStorage.StatblockStorage
{
    public class MonsterStatblock
    {
        public string name { get; set; }
        public string size { get; set; }
        public string type { get; set; }
        public string alignment { get; set; }
        public string armorClass { get; set; }
        public string hitPoints { get; set; }
        public string speed { get; set; }
        public string challengeRating { get; set; }
        public StatblockAttributes attributes { get; set; }
        public List<StatblockAction> actions { get; set; }

    }

    public class StatblockAttributes
    {
        public AttributeValues STR { get; set; }
        public AttributeValues DEX { get; set; }
        public AttributeValues CON { get; set; }
        public AttributeValues INT { get; set; }
        public AttributeValues WIS { get; set; }
        public AttributeValues CHA { get; set; }

    }

    public class AttributeValues
    {
        public string value { get; set; }
        public string modifier { get; set; }
    }

    public class StatblockAction
    {
        public string name { get; set; }
        public string description { get; set; }
    }
}
