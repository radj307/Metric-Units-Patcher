using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System.Collections.Generic;

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
        public uint max_decimal = 1;

        [SettingName("Use \"Meter\" instead of \"Metre\""), Tooltip("When checked, the American spelling of Meter is used.")]
        public bool use_american_spelling = true;

        [SettingName("Allow Centimeters"), Tooltip("When checked, values smaller than 1 meter will be converted to centimeters instead. If unchecked, all values will be in meters, and any values less than 1 will ignore the \"Truncate Decimals After\" setting.")]
        public bool allow_centimeters = true;

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
