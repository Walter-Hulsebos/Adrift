using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonGames.Utilities.Singletons;

namespace Game
{
    public class ResourceManager : PersistentLazySingleton<ResourceManager>
    {
        public int storedNeutronium = 0;

        public void AddNeutronium(int amount)
        {
            storedNeutronium += amount;
        }

        public void RemoveNeutronium(int amount)
        {
            storedNeutronium -= amount;
        }
    }
}
