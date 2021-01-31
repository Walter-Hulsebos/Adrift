using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGTK.Utilities.Singletons;

namespace Game
{
    public class ResourceManager : PersistentLazySingleton<ResourceManager>
    {
        public int storedNeutronium = 0;
        public delegate void ReceivedNeutronium(int currentAmount);
        public ReceivedNeutronium onReceivedNeutronium;

        public void AddNeutronium(int amount)
        {
            storedNeutronium += amount;
            onReceivedNeutronium(storedNeutronium);
        }

        public void RemoveNeutronium(int amount)
        {
            storedNeutronium -= amount;
        }
    }
}
