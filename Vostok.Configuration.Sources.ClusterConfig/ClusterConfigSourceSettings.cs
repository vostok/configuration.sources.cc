using System;
using JetBrains.Annotations;
using Vostok.ClusterConfig.Client.Abstractions;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig
{
    /// <summary>
    /// Represents configuration options for <see cref="ClusterConfigSource"/>.
    /// </summary>
    [PublicAPI]
    public class ClusterConfigSourceSettings
    {
        public ClusterConfigSourceSettings([NotNull] IClusterConfigClient client, [NotNull] string prefix)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }

        /// <summary>
        /// <para>An instance of <see cref="IClusterConfigClient"/> used to obtain settings.</para>
        /// <para>This configuration parameter is mandatory.</para>
        /// </summary>
        [NotNull]
        public IClusterConfigClient Client { get; }

        /// <summary>
        /// <para>Prefix passed to <see cref="IClusterConfigClient"/>'s <see cref="IClusterConfigClient.Observe"/> method.</para>
        /// <para>This configuration parameter is mandatory. May be empty though.</para>
        /// </summary>
        [NotNull]
        public string Prefix { get; }

        /// <summary>
        /// If set to <c>true</c>, <see cref="ClusterConfigSource"/> will split keys containing dots (such as <c>a.b.c</c>) into a hierarchy of <see cref="ObjectNode"/>s.
        /// </summary>
        public bool SplitMultiLevelKeys { get; set; } = true;
        
        /// <summary>
        /// If provided with a non-null value, <see cref="ClusterConfigSource"/> will use this delegate to parse values of all <see cref="ValueNode"/>s in observed tree, replacing them with parsed nodes.
        /// </summary>
        [CanBeNull]
        public Func<string, ISettingsNode> ValuesParser { get; set; }
    }
}