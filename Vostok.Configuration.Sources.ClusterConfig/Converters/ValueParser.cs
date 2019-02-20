using System;
using System.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.Converters
{
    internal class ValueParser : ISettingsNodeConverter
    {
        private readonly Func<string, ISettingsNode> parseSettings;

        public ValueParser(Func<string, ISettingsNode> parseSettings)
        {
            this.parseSettings = parseSettings;
        }

        public ISettingsNode Convert(ISettingsNode node)
        {
            if (parseSettings == null)
                return node;

            if (!(node is ObjectNode objectNode) || node.Children.Count() != 1)
                throw new FormatException($"Provided settings node of type '{node.GetType()}' cannot be parsed.");

            return parseSettings(objectNode.Children.Single().Value);
        }
    }
}