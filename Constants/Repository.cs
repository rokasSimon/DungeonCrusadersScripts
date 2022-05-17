using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Constants
{
    public static class Repository
    {
        private static readonly string PlayerDataPath = Application.persistentDataPath + "/PlayerData.json";
        private static readonly string PlayerOptionsPath = Application.persistentDataPath + "/PlayerOptions.json";

        public static void SavePlayerData(Player player)
        {
            var json = JsonConvert.SerializeObject(player, Formatting.Indented);
            File.WriteAllText(PlayerDataPath, json);
        }

        public static Player LoadPlayerData()
        {
            if (!PlayerDataExists())
                return null;

            var json = File.ReadAllText(PlayerDataPath);
            var player = JsonConvert.DeserializeObject<Player>(json);


            return player;
        }

        public static bool PlayerDataExists()
        {
            return File.Exists(PlayerDataPath);
        }

        public static bool PlayerOptionsExists()
        {
            return File.Exists(PlayerOptionsPath);
        }
    }

}