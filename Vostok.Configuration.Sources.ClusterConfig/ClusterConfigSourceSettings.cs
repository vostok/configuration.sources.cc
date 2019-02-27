using System;
using JetBrains.Annotations;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig
{
    [PublicAPI]
    public class ClusterConfigSourceSettings
    {
        public ClusterConfigSourceSettings([NotNull] IClusterConfigClient client, [NotNull] string prefix)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

        [NotNull]
        public IClusterConfigClient Client { get; }

        [NotNull]
        public string Prefix { get; }

        public bool SplitMultiLevelKeys { get; set; } = true;
        
        public Func<string, ISettingsNode> ParseSettings { get; set; }
    }
}