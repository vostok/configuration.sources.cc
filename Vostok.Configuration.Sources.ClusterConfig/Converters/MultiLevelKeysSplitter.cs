using System;
using System.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.SettingsNodeConverters
{
    internal class MultiLevelKeysSplitter : ISettingsNodeConverter
    {
        private readonly string[] separators;

        public MultiLevelKeysSplitter(params string[] separators)
        {
            this.separators = separators;
        }

        public ISettingsNode Convert(ISettingsNode node)
        {
            var keys = node.Name == null
                ? new string[]{null}
                : node.Name.Replace(" ", "").Split(separators, StringSplitOptions.RemoveEmptyEntries);
            
            if (keys.Length == 0)
                keys = new []{""};
            
            switch (node)
            {
                case ValueNode valueNode:
                    return TreeFactory.CreateTreeByMultiLevelKey(keys[0], keys.Skip(1).ToArray(), valueNode.Value);
                case ArrayNode arrayNode:
                    return TreeFactory.CreateTreeByMultiLevelKey(keys[0], keys.Skip(1), new ArrayNode(keys.Last(), arrayNode.Children.Select(Convert).ToArray()));
                case ObjectNode objectNode:
                    return TreeFactory.CreateTreeByMultiLevelKey(keys[0], keys.Skip(1), new ObjectNode(keys.Last(), objectNode.Children.Select(Convert)));
                default:
                    return node;
            }
        }
    }
}