using MetricUnits.Util;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System;
using System.Threading.Tasks;


namespace MetricUnits
{
    public class Program
    {
        private static Lazy<Settings> _lazySettings = null!;
        private static Settings Settings => _lazySettings.Value;

        public static async Task<int> Main(string[] args)
            => await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "Metric-Units-Patch.esp")
                .SetAutogeneratedSettings("Settings", "settings.json", out _lazySettings)
                .Run(args);

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            Console.WriteLine("Initialization Complete");

            if (Settings.keys.Count == 0)
            {
                Console.WriteLine($"[ERROR]:  The plugin list is empty! (See the settings tab to add plugins)");
                return;
            }

            // instantiate external conversion executable
            CreationKitUnitConverter ckconv = new();

            Console.WriteLine($"Finished initializing {ckconv.GetVersion(true)}");

            ulong count = 0;

            foreach (var msg in state.LoadOrder.PriorityOrder.Message().WinningContextOverrides())
            {
                if (!Settings.Whitelisted(msg) || msg.Record.EditorID == null)
                    continue;

                var copy = msg.Record.DeepCopy();

                var (str, changes) = Utilities.PatchString(copy.Description!.Lookup(Language.English) ?? "", ckconv, Settings, msg.Record.EditorID);

                if (changes == 0)
                    continue;

                copy.Description = str;

                state.PatchMod.Messages.Set(copy);

                ++count;
            }

            foreach (var mgef in state.LoadOrder.PriorityOrder.MagicEffect().WinningContextOverrides())
            {
                if (!Settings.Whitelisted(mgef) || mgef.Record.EditorID == null || mgef.Record.Description == null)
                    continue;

                var copy = mgef.Record.DeepCopy()!;

                var (str, changes) = Utilities.PatchString(copy.Description!.Lookup(Language.English) ?? "", ckconv, Settings, mgef.Record.EditorID);

                if (changes == 0)
                    continue;

                copy.Description = str;

                state.PatchMod.MagicEffects.Set(copy);

                ++count;
            }

            foreach(var perk in state.LoadOrder.PriorityOrder.Perk().WinningContextOverrides())
            {
                if (!Settings.Whitelisted(perk))
                    continue;

                var copy = perk.Record.DeepCopy();

                var (str, changes) = Utilities.PatchString(copy.Description!.Lookup(Language.English) ?? "", ckconv, Settings, perk.Record.EditorID);

                if (changes == 0)
                    continue;

                copy.Description = str;

                state.PatchMod.Perks.Set(copy);

                ++count;
            }

            Console.WriteLine($"Patcher complete. Modified {count} record{(count > 0 ? "s" : "")}.");
        }
    }
}
