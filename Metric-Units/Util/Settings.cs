using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;

namespace MetricUnits.Util
{
    public class Settings
    {
        [MaintainOrder]
        [SettingName("Plugin Whitelist"), Tooltip("Only plugins on this list will be checked for imperial units to convert.")]
        public List<ModKey> keys = new List<ModKey>()
        {
            ModKey.FromNameAndExtension("Ordinator - Perks of Skyrim.esp")
        };
        [SettingName("Decimal Precision"), Tooltip("This is the number of digits after the decimal to keep when rounding the resulting value in meters.")]
        public uint round_to_decimal = 1;

        public bool Whitelisted(ModKey modkey)
        {
            return keys.Contains(modkey);
        }

        public bool Whitelisted(IModContext record)
        {
            if (Whitelisted(record.ModKey))
                return true;
            if (record.Parent != null)
                return Whitelisted(record.Parent);
            return false;
        }
    }
}
