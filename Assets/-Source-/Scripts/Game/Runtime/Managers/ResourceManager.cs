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

        public delegate void NeutroniumChanged(int currentAmount);
        public ReceivedNeutronium onNeutroniumChanged;

        public void AddNeutronium(int amount)
        {
            storedNeutronium += amount;
            onReceivedNeutronium?.Invoke(storedNeutronium);
            onNeutroniumChanged?.Invoke(storedNeutronium);
        }

        public void RemoveNeutronium(int amount)
        {
            storedNeutronium -= amount;
            onNeutroniumChanged?.Invoke(storedNeutronium);
        }
    }
}
