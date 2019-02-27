using System;
using System.Linq;
using JetBrains.Annotations;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.ClusterConfig.Converters;
using Vostok.Configuration.Sources.Extensions.Observable;

namespace Vostok.Configuration.Sources.ClusterConfig
{
    /// <summary>
    /// A source that loads settings from ClusterConfig.
    /// </summary>
    [PublicAPI]
    public class ClusterConfigSource : IConfigurationSource
    {
        private static readonly string[] Separators = {"."};
        
        private readonly Func<string, ISettingsNode> parseSettings;
        private readonly IClusterConfigClient client;
        private readonly ISettingsNodeConverter[] converters;
        private readonly string prefix;

        public ClusterConfigSource(ClusterConfigSourceSettings settings)
            :this(
                settings.Client,
                settings.Prefix,
                new ISettingsNodeConverter[]
                {
                    settings.SplitMultiLevelKeys ? new MultiLevelKeysSplitter(Separators) : null,
                    settings.ValuesParser != null ? new ValueParser(settings.ValuesParser) : null
                }.Where(converter => converter != null).ToArray())
        {
        }
        
        internal ClusterConfigSource(IClusterConfigClient client, string prefix, params ISettingsNodeConverter[] converters)
        {
            this.client = client;
            this.prefix = prefix ?? "";
            this.converters = converters;
        }

        public IObservable<(ISettingsNode settings, Exception error)> Observe() =>
            client
                .Observe(prefix)
                .Select(settings => converters.Aggregate(settings, (s, converter) => converter.Convert(s)))
                .Select(settings => (settings, null as Exception));
    }
}