using Mutagen.Bethesda.Plugins;
using System.Collections.Generic;

namespace MetricUnits.Util
{
    public class Settings
    {
        public List<ModKey> keys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Ordinator - Perks of Skyrim.esp")
        };


        public bool Whitelisted(ModKey modkey)
        {
            return keys.Contains(modkey);
        }
    }
}
