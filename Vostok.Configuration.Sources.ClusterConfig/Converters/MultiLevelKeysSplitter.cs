using System;
using System.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.Converters
{
    internal class MultiLevelKeysSplitter : ISettingsNodeConverter
    {
        private const char Separator = '.';

        // TODO(krait): Optimize!
        public ISettingsNode Convert(ISettingsNode node)
        {
            if (node == null)
                return null;

            var keys = node.Name == null
                ? new string[]{null}
                : node.Name.Replace(" ", "").Split(new [] {Separator}, StringSplitOptions.RemoveEmptyEntries);
            
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