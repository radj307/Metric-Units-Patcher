using Mutagen.Bethesda.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricUnits
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
