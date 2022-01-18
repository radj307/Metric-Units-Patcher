using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MetricUnits.Util
{
    public class Settings
    {
        [MaintainOrder]
        [SettingName("Plugin Whitelist"), Tooltip("Only plugins on this list will be checked for imperial units to convert.")]
        public List<ModKey> keys = new()
        {
            ModKey.FromNameAndExtension("Ordinator - Perks of Skyrim.esp")
        };

        [SettingName("Enable Plugin Whitelist"), Tooltip("When checked, only whitelisted plugins will be checked.")]
        public bool enable_whitelist = true;

        [SettingName("Truncate Decimals After"), Tooltip("Truncate digits that appear this many characters after the decimal point.")]
        public uint max_decimal = 0;

        [SettingName("Use \"Meter\" instead of \"Metre\""), Tooltip("When checked, the American spelling of Meter is used.")]
        public bool use_american_spelling = true;

        public bool Whitelisted(ModKey modkey)
        {
            return !enable_whitelist || keys.Contains(modkey);
        }

        public bool Whitelisted(IModContext record)
        {
            return Whitelisted(record.ModKey) || (record.Parent != null && Whitelisted(record.Parent));
        }
    }
}
