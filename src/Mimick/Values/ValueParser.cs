using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Values
{
    /// <summary>
    /// A class responsible for parsing the content of a value expression into nodes.
    /// </summary>
    internal sealed class ValueParser
    {
        private readonly char[] buf;
        private readonly int count;

        private List<Node> children;
        private Node current;
        private int index;
        private Node parent;
        private Node root;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueParser" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public ValueParser(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression", "Cannot parse a null expresson");
            
            if (expression.Trim().Length == 0)
                throw new ArgumentException("expression", "Cannot parse a blank expression");

            buf = expression.ToCharArray();
            children = new List<Node>();
            count = buf.Length;
            index = 0;
            parent = root = new Node { Type = NodeType.Group, Value = children };
        }

        /// <summary>
        /// Gets whether the provided character is a recognised symbol.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns><c>true</c> if the character is a symbol; otherwise, <c>false</c>.</returns>
        private bool IsSymbol(char c) => c == '+' || c == '-' || c == '/' || c == '*' || c == '%';

        /// <summary>
        /// Parse the content of the expression supplied into the constructor and retrieve the root value node.
        /// </summary>
        /// <returns>The <see cref="Node"/> value representing the expression.</returns>
        public Node Parse()
        {
            while (index < count)
            {
                var c = buf[index];

                if (char.IsWhiteSpace(c))
                {
                    index++;
                    continue;
                }

                if (c == '(')
                    ParseBeginGroup();
                else if (c == ')')
                    ParseEndGroup();
                else if (char.IsNumber(c))
                    ParseConstantNumber();
                else if (c == '-')
                {
                    if (index + 1 < count && char.IsNumber(buf[index + 1]))
                        ParseConstantNumber();
                    else
                        ParseSymbol();
                }
                else if (IsSymbol(c))
                    ParseSymbol();
                else if (c == '\'')
                    ParseString();
                else if (c == '{')
                    ParseVariable();
                else
                {
                    if (index == 0)
                        ParseFullString();
                    else
                        ThrowError("Unexpected character: expected a constant, symbol or group");
                }
            }

            if (!ReferenceEquals(root, parent))
                ThrowError("Unexpected end-of-expression: a group may not have been closed");

            if (current != null && current.Type == NodeType.Symbol)
                ThrowError("Unexpected end-of-expression: expected a constant or group");

            return root;
        }

        /// <summary>
        /// Parses the beginning of a group from the expression.
        /// </summary>
        private void ParseBeginGroup()
        {
            if (current != null && current.Type != NodeType.Symbol)
                ThrowError("Unexpected group: expected a symbol");

            var list = new List<Node>();
            var container = new Node
            {
                Parent = parent,
                Type = NodeType.Group,
                Value = list
            };
            
            children.Add(container);
            children = list;
            parent = container;
            current = null;

            index++;
        }

        /// <summary>
        /// Parses a constant number value.
        /// </summary>
        private void ParseConstantNumber()
        {
            var start = index;

            while (index < count)
            {
                var c = buf[index];

                if (char.IsWhiteSpace(c))
                    break;

                if ((c == '-' || IsSymbol(c)) && index != start)
                    break;

                if (c == ')')
                    break;

                if ((c != '.' && !char.IsNumber(c)) && !(c == '-' && index == start))
                    ThrowError("Unexpected character: expected a number or decimal point");

                index++;
            }

            if (start == index)
                ThrowError("Unexpected end-of-number: expected a number value");

            var text = new string(buf, start, index - start);

            if (!double.TryParse(text, out var value))
                ThrowError("Unexpected constant: expected a number value");

            var points = text.IndexOf('.') != -1;

            var constant = new Constant(value);
            var node = new Node
            {
                Parent = parent,
                Type = NodeType.Constant,
            };

            node.Value = new Constant(NumberHelper.Shrink(value));
            
            children.Add(node);
            current = node;
        }

        /// <summary>
        /// Parses the end of a group from the expression.
        /// </summary>
        private void ParseEndGroup()
        {
            if (current == null || ReferenceEquals(parent, root))
                ThrowError("Unexpected end of group: expected an expression");

            if (current.Type == NodeType.Symbol)
                ThrowError("Unexpected end of group: expected a constant or variable");

            parent = parent.Parent;
            children = parent.Value as List<Node>;
            current = children.Last();

            index++;
        }

        /// <summary>
        /// Parses the whole expression as a constant string value.
        /// </summary>
        private void ParseFullString()
        {
            var value = new string(buf);
            var constant = new Constant(value);
            var node = new Node
            {
                Parent = parent,
                Type = NodeType.Constant,
                Value = new Constant(value)
            };

            index = count;

            children.Add(node);
            current = node;
        }

        /// <summary>
        /// Parses a constant string value.
        /// </summary>
        private void ParseString()
        {
            var start = ++index;

            while (index < count)
            {
                var c = buf[index++];

                if (c == '\\' && index + 1 < count && buf[index + 1] == '\'')
                    index++;

                if (c == '\'')
                    break;
            }

            if (index == count && buf[index - 1] != '\'')
                ThrowError("Unexpected end-of-string: expected a closing quotation");
            
            var value = index == start + 1 ? string.Empty : new string(buf, start, index - start - 1);
            var constant = new Constant(value);
            var node = new Node
            {
                Parent = parent,
                Type = NodeType.Constant,
                Value = constant
            };

            children.Add(node);
            current = node;
        }

        /// <summary>
        /// Parses a symbol value.
        /// </summary>
        private void ParseSymbol()
        {
            var c = buf[index++];
            var op = Operator.Add;

            switch (c)
            {
                case '+': break;
                case '-': op = Operator.Subtract; break;
                case '/': op = Operator.Divide; break;
                case '*': op = Operator.Multiply; break;
                case '%': op = Operator.Modulus; break;
                default:
                    ThrowError("Unexpected symbol: expected a valid operator");
                    break;
            }

            var node = new Node
            {
                Parent = parent,
                Type = NodeType.Symbol,
                Value = op
            };

            children.Add(node);
            current = node;
        }

        /// <summary>
        /// Parses a variable value.
        /// </summary>
        private void ParseVariable()
        {
            var start = ++index;

            while (index < count)
            {
                var c = buf[index++];

                if (c == '\\' && index + 1 < count && ((c = buf[index + 1]) == '{' || (c = buf[index + 1]) == '}'))
                {
                    index++;
                    continue;
                }

                if (c == '}')
                    break;
            }

            var value = index == start + 1 ? string.Empty : new string(buf, start, index - start - 1);

            if (value.Trim().Length == 0)
                ThrowError($"Expected a variable name");

            var variable = new Variable(value);
            var node = new Node
            {
                Parent = parent,
                Type = NodeType.Variable,
                Value = variable
            };

            children.Add(node);
            current = node;
        }

        /// <summary>
        /// Throws a <see cref="ValueParseException"/> using the current configuration.
        /// </summary>
        /// <param name="message">The message.</param>
        private void ThrowError(string message) => throw new ValueParseException(buf, index - 1, message);
    }

    /// <summary>
    /// An exception class thrown when a value expression could not be processed.
    /// </summary>
    public class ValueParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueParseException" /> class.
        /// </summary>
        /// <param name="buf">The value content.</param>
        /// <param name="index">The index.</param>
        /// <param name="message">The message.</param>
        public ValueParseException(char[] buf, int index, string message) : base(message)
        {
            var begin = index < 5 ? index : 5;
            var end = index + 5 > buf.Length ? buf.Length - index : 5;

            Excerpt = new string(buf, index - begin, begin) + "'" + buf[index] + "'" + new string(buf, index + 1, end);
            Position = index;
        }

        #region Properties

        /// <summary>
        /// Gets the excerpt of the part of the expression which failed.
        /// </summary>
        public string Excerpt
        {
            get;
        }

        /// <summary>
        /// Gets the position within the expression.
        /// </summary>
        public int Position
        {
            get;
        }

        #endregion
    }
}
