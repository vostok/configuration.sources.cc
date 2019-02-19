using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.SettingsNodeConverters
{
    internal interface ISettingsNodeConverter
    {
        ISettingsNode Convert(ISettingsNode settings);
    }
}