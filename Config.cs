using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Better_207
{
    public class Config : IConfig
    {
        [Description("↓Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("↓Chance of being damaged after drink SCP-207 (in %)")]
        public ushort DommageChance { get; set; } = 100;

        [Description("↓Damage received")]
        public int DommageAmount { get; set; } = 20;

        [Description("↓Duration of SCP-207 effect, if 0 no limit")]
        public int EffectDuration { get; set; } = 20;

        [Description("↓takes damage from the effect of 207 (if he drank a coke)")]
        public bool NoDammage { get; set; } = true;
    }
}
