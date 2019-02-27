using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.Converters
{
    internal interface ISettingsNodeConverter
    {
        [CanBeNull]
        ISettingsNode Convert([CanBeNull] ISettingsNode settings);
    }
}