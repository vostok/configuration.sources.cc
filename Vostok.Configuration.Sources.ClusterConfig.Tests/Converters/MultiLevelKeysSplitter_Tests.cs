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
        private MultiLevelKeysSplitter splitter;

        [SetUp]
        public void SetUp()
        {
            splitter = new MultiLevelKeysSplitter();
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("key")]
        public void Should_leave_ValueNode_untouched_when_there_are_no_separators_in_name(string name)
        {
            var node = Value(name, "value");
            splitter.Convert(node).Should().BeSameAs(node);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("key")]
        public void Should_leave_ArrayNode_untouched_when_no_separators_in_name(string name)
        {
            var node = Array(name, "value1", "value2");
            splitter.Convert(node).Should().BeSameAs(node);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("key")]
        public void Should_leave_ObjectNode_untouched_when_no_separators_in_name(string name)
        {
            var node = Object(name, ("key1", "value1"), ("key2", "value2"));
            splitter.Convert(node).Should().BeSameAs(node);
        }

        [Test]
        public void Should_leave_an_arbitrary_hierarchy_of_nodes_untouched_if_there_are_no_separators_in_value_node_names()
        {
            var node = Object("zebra", 
                Object("master.settings.shared", ("k", "v")),
                Object("ts.settings.shared", ("k2", "v2")),
                Array("name.with.dots", Value("1"), Value("2")));

            splitter.Convert(node).Should().BeSameAs(node);
        }

        [Test]
        public void Should_create_tree_when_ValueNode_has_separators_in_name()
        {
            var node = new ValueNode("a.b.c", "value");
            splitter.Convert(node).Should().Be(Object("a", Object("b", Value("c", "value"))));
        }
        
        [Test]
        public void Should_work_recursively()
        {
            var node = Object("a.b", Array("c.d", Value("e.f", "value"), Value("e.g", "value2")));

            splitter.Convert(node)
                .Should()
                .Be(Object("a.b", Array("c.d", Object("e", Value("f", "value"), Value("g", "value2")))));
        }

        [Test]
        public void Should_preserve_ordering_for_arrays()
        {
            var node = Object("foo",
                Object("bar", Value("a.b", "1"), Value("a.c", "2"), Value("b.c", "3")),
                Array("baz", Object("obj", new ISettingsNode[] { }), Value("d.e", "4"), Value("d.f", "5"), Value("key", "6")));

            var expectedResult = Object("foo",
                Object("bar",
                    Object("a", Value("b", "1"), Value("c", "2")),
                    Object("b", Value("c", "3"))),
                Array("baz", 
                    Object("obj", new ISettingsNode[] { }), 
                    Object("d", 
                        Value("e", "4"), 
                        Value("f", "5")),
                    Value("key", "6")));

            splitter.Convert(node).Should().Be(expectedResult);
        }

        [Test]
        public void Should_not_merge_object_nodes_with_null_names()
        {
            var node = Array(
                Value("a.b", "1"),
                Object(null as string, Value("x1", "y1")),
                Object(null as string, Value("x2", "y2")));

            var expectedResult = Array(
                Object("a", Value("b", "1")),
                Object(null as string, Value("x1", "y1")),
                Object(null as string, Value("x2", "y2")));

            splitter.Convert(node).Should().Be(expectedResult);
        }
    }
}