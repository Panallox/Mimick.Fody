using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing methods for processing values.
    /// </summary>
    public static class ValueHelper
    {
        /// <summary>
        /// Parses the content of a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The expected type.</param>
        /// <param name="context">The context.</param>
        /// <returns>The resulting value.</returns>
        public static object Parse(string value, Type type, IFrameworkContext context)
        {
            if (value == null)
                return TypeHelper.Default(type);

            var buf = value.ToCharArray();

            if (buf.Length == 0)
                return type == typeof(string) ? "" : TypeHelper.Default(type);

            var list = new List<Node>();
            var root = new Node { Type = NodeType.Group, Value = list };

            var container = root;
            var current = (Node)null;

            for (int i = 0, count = buf.Length; i < count; i++)
            {
                var c = buf[i];

                if (char.IsWhiteSpace(c))
                    continue;
                
                if (c == '(')
                {
                    if (current != null && current.Type != NodeType.Symbol)
                        throw new ArgumentException($"Unexpected '(' where there should be content as position {i}");

                    var children = new List<Node>();
                    var child = new Node { Parent = container, Type = NodeType.Group, Value = children };

                    list.Add(child);
                    list = children;
                    container = child;
                    current = null;
                }
                else if (c == ')')
                {
                    if (container.Parent == null || current == null || current.Type == NodeType.Symbol)
                        throw new ArgumentException($"Unexpected ')' where there should be content at position {i}");

                    list = container.Parent.Value as List<Node>;
                    container = container.Parent;
                    current = list.Last();
                }
                else if (c == '\'')
                {
                    var str = "";
                    i++;

                    while (i < count)
                    {
                        c = buf[i++];

                        if (c == '\'')
                            break;

                        if (c == '\\' && i < count && buf[i] == '\'')
                        {
                            str += "'";
                            i++;
                            continue;
                        }

                        str += c;
                    }

                    var node = new Node { Previous = current, Type = NodeType.Value, Parent = container, TypeCode = TypeCode.String, Value = str };

                    if (current != null)
                        current.Next = node;

                    list.Add(node);
                    current = node;
                }
                else if (char.IsNumber(c) || (c == '-' && i + 1 < count && char.IsNumber(buf[i + 1])))
                {
                    var str = "" + c;
                    var prd = false;
                    i++;

                    while (i < count)
                    {
                        c = buf[i++];

                        if (char.IsWhiteSpace(c) || c == '+' || c == '-' || c == '*' || c == '/')
                            break;

                        if (!char.IsNumber(c) && c != '.')
                            throw new ArgumentException($"Unexpected '{c}' when there should be a number of period at position {i - 1}");

                        if (c == '.')
                        {
                            if (prd)
                                throw new ArgumentException($"Unexpected '.' when there should be a number at position {i - 1}");
                            prd = true;
                        }

                        str += c;
                    }

                    var val = (object)null;
                    var code = TypeCode.Empty;

                    if (prd)
                    {

                        if (!double.TryParse(str, out var num))
                            throw new ArgumentException($"Unrecognised number '{str}'");

                        val = num;
                        code = TypeCode.Double;
                    }
                    else
                    {
                        if (!long.TryParse(str, out var num))
                            throw new ArgumentException($"Unrecognised number '{str}'");

                        val = num;
                        code = TypeCode.Int64;
                    }

                    var node = new Node { Previous = current, Type = NodeType.Value, Parent = container, TypeCode = code, Value = val };

                    if (current != null)
                        current.Next = node;

                    list.Add(node);
                    current = node;
                }
                else if (c == '-' || c == '+' || c == '*' || c == '/')
                {
                    if (current == null || (current != null && current.Type == NodeType.Symbol))
                        throw new ArgumentException($"Unexpected '{c}' when there should be content at position {i}");
                }
            }

            return null;
        }

        /// <summary>
        /// A structure representing a node of a value.
        /// </summary>
        private class Node
        {
            public Node Previous;
            public Node Next;
            public Node Parent;
            public NodeType Type;
            public TypeCode TypeCode;
            public object Value;
        }

        /// <summary>
        /// Indicates the type of content contained within a node.
        /// </summary>
        private enum NodeType
        {
            /// <summary>
            /// The node is a group.
            /// </summary>
            Group,

            /// <summary>
            /// The node is a symbol.
            /// </summary>
            Symbol,

            /// <summary>
            /// The node is a value.
            /// </summary>
            Value,
        }
        
        private enum NodeSymbol
        {
            Add,
        }
    }
}
