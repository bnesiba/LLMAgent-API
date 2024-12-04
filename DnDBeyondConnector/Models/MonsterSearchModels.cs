using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DnDBeyondConnector.Models
{
    public class MonsterStatModel
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int ArmorClass { get; set; }
        public int AverageHitPoints { get; set; }
        public string HitPointDice { get; set; }
        public int PassivePerception { get; set; }
        public string Alignment { get; set; } // Alignment value (translated from alignmentId).
        public string Size { get; set; } // Size value (translated from sizeId).
        public string Type { get; set; } // Type value (translated from typeId).
        public string ChallengeRating { get; set; } // Challenge rating (translated from challengeRatingId).

        // Standard ability scores.
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public List<string> SpecialTraits { get; set; } // Special traits descriptions.
        public List<string> Actions { get; set; } // Actions descriptions.
        public List<string> Tags { get; set; } // Tags (if any).

    }


    public static class MonsterStatUtil
    {
        // Utility to map challengeRatingId to the challenge rating string.
        public static string TranslateChallengeRating(int challengeRatingId)
        {
            return challengeRatingId switch
            {
                1 => "none",
                2 => "1/8",
                3 => "1/4",
                4 => "1/2",
                _ => (challengeRatingId - 4).ToString() // Beyond CR 1/2, ID increases linearly.
            };
        }

        // Utility to map sizeId to the size string.
        public static string TranslateSize(int sizeId)
        {
            return sizeId switch
            {
                2 => "Tiny",
                3 => "Small",
                4 => "Medium",
                5 => "Large",
                6 => "Huge",
                7 => "Gargantuan",
                _ => "Unknown" // Fallback for invalid IDs.
            };
        }

        // Utility to map alignmentId to the alignment string.
        public static string TranslateAlignment(int alignmentId)
        {
            return alignmentId switch
            {
                1 => "Lawful Good",
                2 => "Neutral Good",
                3 => "Chaotic Good",
                4 => "Lawful Neutral",
                5 => "True Neutral",
                6 => "Chaotic Neutral",
                7 => "Lawful Evil",
                8 => "Neutral Evil",
                9 => "Chaotic Evil",
                10 => "Unaligned",
                11 => "Any",
                12 => "Any Evil",
                13 => "Any Good",
                14 => "Any Chaotic",
                15 => "Any Lawful",
                16 => "Any Non-Good",
                17 => "Any Non-Lawful",
                18 => "Unaligned or Chaotic Neutral",
                19 => "Unaligned or Neutral Good",
                20 => "Unaligned or Lawful Good",
                21 => "Unaligned or Chaotic Evil",
                22 => "Unaligned or Neutral Evil",
                23 => "Unaligned or Chaotic Good",
                24 => "Unaligned or True Neutral",
                25 => "Unaligned or Lawful Evil",
                26 => "Unaligned or Lawful Neutral",
                _ => "Unknown" // Fallback for invalid IDs.
            };
        }

        // Utility to map typeId to the type string.
        public static string TranslateType(int typeId)
        {
            return typeId switch
            {
                1 => "Aberration",
                2 => "Beast",
                3 => "Celestial",
                4 => "Construct",
                6 => "Dragon",
                7 => "Elemental",
                8 => "Fey",
                9 => "Fiend",
                10 => "Giant",
                11 => "Humanoid",
                13 => "Monstrosity",
                14 => "Ooze",
                15 => "Plant",
                16 => "Undead",
                _ => "Unknown" // Fallback for invalid IDs.
            };
        }
    }

    public static class MonsterStatModelFactory
    {
        public static List<MonsterStatModel> FromJson(string jsonData)
        {
            var monsterStatModels = new List<MonsterStatModel>();

            // Parse the JSON object.
            var dataBlob = JObject.Parse(jsonData);

            // Extract the "data" array containing monster details.
            var monsters = dataBlob["data"] as JArray;

            if (monsters == null) return monsterStatModels;

            // Process each monster in the array.
            foreach (var monster in monsters)
            {
                var model = new MonsterStatModel
                {
                    Id = monster["id"]?.Value<int>() ?? 0,
                    Name = monster["name"]?.Value<string>() ?? "Unknown",
                    ArmorClass = monster["armorClass"]?.Value<int>() ?? 0,
                    AverageHitPoints = monster["averageHitPoints"]?.Value<int>() ?? 0,
                    HitPointDice = monster["hitPointDice"]?["diceString"]?.Value<string>() ?? "0d0",
                    PassivePerception = monster["passivePerception"]?.Value<int>() ?? 0,
                    Alignment = MonsterStatUtil.TranslateAlignment(monster["alignmentId"]?.Value<int>() ?? 0),
                    Size = MonsterStatUtil.TranslateSize(monster["sizeId"]?.Value<int>() ?? 0),
                    Type = MonsterStatUtil.TranslateType(monster["typeId"]?.Value<int>() ?? 0),
                    ChallengeRating = MonsterStatUtil.TranslateChallengeRating(monster["challengeRatingId"]?.Value<int>() ?? 1),

                    // Parse standard stats.
                    Strength = ExtractStatValue(monster, 1), // Strength
                    Dexterity = ExtractStatValue(monster, 2), // Dexterity
                    Constitution = ExtractStatValue(monster, 3), // Constitution
                    Intelligence = ExtractStatValue(monster, 4), // Intelligence
                    Wisdom = ExtractStatValue(monster, 5), // Wisdom
                    Charisma = ExtractStatValue(monster, 6), // Charisma

                    SpecialTraits = ExtractHtmlDescriptions(monster["specialTraitsDescription"]),
                    Actions = ExtractHtmlDescriptions(monster["actionsDescription"]),
                    Tags = monster["tags"]?.ToObject<List<string>>() ?? new List<string>()
                };

                monsterStatModels.Add(model);
            }

            return monsterStatModels;
        }

        private static int ExtractStatValue(JToken monster, int statId)
        {
            var statsArray = monster["stats"] as JArray;

            // Find the stat matching the given statId and return its value.
            var stat = statsArray?.FirstOrDefault(s => s["statId"]?.Value<int>() == statId);
            return stat?["value"]?.Value<int>() ?? 0;
        }

        private static List<string> ExtractHtmlDescriptions(JToken descriptionToken)
        {
            if (descriptionToken == null || descriptionToken.Type == JTokenType.Null)
                return new List<string>();

            var rawHtml = descriptionToken.Value<string>() ?? string.Empty;
            var descriptions = rawHtml.Split(new[] { "</p>" }, StringSplitOptions.RemoveEmptyEntries);

            // Clean up the HTML tags.
            var cleanedDescriptions = new List<string>();
            foreach (var desc in descriptions)
            {
                var cleanText = System.Text.RegularExpressions.Regex.Replace(desc, "<.*?>", "").Trim();
                if (!string.IsNullOrEmpty(cleanText))
                    cleanedDescriptions.Add(cleanText);
            }

            return cleanedDescriptions;
        }
    }


}
