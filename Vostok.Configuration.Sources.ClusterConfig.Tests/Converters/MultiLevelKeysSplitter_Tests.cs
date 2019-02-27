using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Sources.ClusterConfig.Converters;
using Vostok.Configuration.Sources.ClusterConfig.Tests.Helpers;

namespace Vostok.Configuration.Sources.ClusterConfig.Tests.Converters
{
    [TestFixture]
    internal class MultiLevelKeysSplitter_Tests : TreeConstructionSet
    {
        private MultiLevelKeysSplitter converter;

        [SetUp]
        public void SetUp()
        {
            converter = new MultiLevelKeysSplitter();
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("key")]
        public void Should_leave_ValueNode_untouched_when_no_separators_in_name(string name)
        {
            var node = Value(name, "value");
            converter.Convert(node).Should().Be(node);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("key")]
        public void Should_leave_ArrayNode_untouched_when_no_separators_in_name(string name)
        {
            var node = Array(name, "value1", "value2");
            converter.Convert(node).Should().Be(node);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("key")]
        public void Should_leave_ObjectNode_untouched_when_no_separators_in_name(string name)
        {
            var node = Object(name, ("key1", "value1"), ("key2", "value2"));
            converter.Convert(node).Should().Be(node);
        }

        [Test]
        public void Should_create_tree_when_ValueNode_has_separators_in_name()
        {
            var node = new ValueNode("a.b.c", "value");
            converter.Convert(node).Should().Be(Object("a", Object("b", Value("c", "value"))));
        }

        [Test]
        public void Should_work_recursively()
        {
            var node = Object("a.b", Array("c.d", Value("e.f", "value")));
            converter.Convert(node)
                .Should()
                .Be(
                    Object(
                        "a",
                        Object(
                            "b",
                            Object(
                                "c",
                                Array(
                                    "d",
                                    Object(
                                        "e",
                                        Value("f", "value")))))));
        }
    }
}