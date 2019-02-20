using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.Converters
{
    internal interface ISettingsNodeConverter
    {
        ISettingsNode Convert(ISettingsNode settings);
    }
}