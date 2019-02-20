using System;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig
{
    [PublicAPI]
    public class ClusterConfigSourceSettings
    {
        public ClusterConfigSourceSettings(string prefix)
        {
            Prefix = prefix;
        }

        [NotNull]
        public string Prefix { get; }

        public bool SplitMultiLevelKeys { get; set; } = true;
        
        public Func<string, ISettingsNode> ParseSettings { get; set; }
    }
}