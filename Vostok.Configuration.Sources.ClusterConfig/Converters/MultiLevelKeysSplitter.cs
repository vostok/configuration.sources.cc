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

        // TODO(krait): Optimize!
        public ISettingsNode Convert(ISettingsNode node)
        {
            if (!EnumerateAllValueNodeNames(node).Any(name => name.Contains(Separator)))
                return node;

            return ConvertInternal(node);
        }

        private static ISettingsNode ConvertInternal(ISettingsNode node)
        {
            if (node == null)
                return null;

            var keys = node.Name == null
                ? new string[] { null }
                : node.Name.Replace(" ", "").Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

            if (keys.Length == 0)
                keys = new[] { "" };

            switch (node)
            {
                case ValueNode valueNode:
                    return TreeFactory.CreateTreeByMultiLevelKey(keys[0], keys.Skip(1).ToArray(), valueNode.Value);
                case ArrayNode arrayNode:
                    return TreeFactory.CreateTreeByMultiLevelKey(keys[0], keys.Skip(1), new ArrayNode(keys.Last(), arrayNode.Children.Select(ConvertInternal).ToArray()));
                case ObjectNode objectNode:
                    return TreeFactory.CreateTreeByMultiLevelKey(keys[0], keys.Skip(1), new ObjectNode(keys.Last(), objectNode.Children.Select(ConvertInternal)));
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