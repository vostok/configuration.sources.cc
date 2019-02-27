using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.SettingsTree;

namespace Vostok.Configuration.Sources.ClusterConfig.Converters
{
    internal class MultiLevelKeysSplitter : ISettingsNodeConverter
    {
        private const char Separator = '.';

        public ISettingsNode Convert(ISettingsNode node)
        {
            if (!EnumerateAllValueNodeNames(node).Any(name => name.Contains(Separator)))
                return node;

            return ConvertInternal(node);
        }

        private static ISettingsNode ConvertInternal(ISettingsNode node)
        {
            switch (node)
            {
                case ValueNode valueNode:
                    if (valueNode.Name == null || !valueNode.Name.Contains(Separator))
                        return valueNode;

                    var nameParts = valueNode.Name.Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
                    if (nameParts.Length == 1)
                        return valueNode;

                    return TreeFactory.CreateTreeByMultiLevelKey(nameParts[0], nameParts.Skip(1).ToArray(), valueNode.Value);

                case ArrayNode arrayNode:
                    return new ArrayNode(arrayNode.Name, arrayNode.Children.Select(ConvertInternal).ToArray());

                case ObjectNode objectNode:
                    return new ObjectNode(objectNode.Name, objectNode.Children.Select(ConvertInternal));

                default:
                    return node;
            }
        }

        private static IEnumerable<string> EnumerateAllValueNodeNames([CanBeNull] ISettingsNode startingNode)
        {
            var queue = new Queue<ISettingsNode>();

            queue.Enqueue(startingNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node == null)
                    continue;

                if (node is ValueNode && node.Name != null)
                    yield return node.Name;

                foreach (var child in node.Children)
                    queue.Enqueue(child);
            }
        }
    }
}