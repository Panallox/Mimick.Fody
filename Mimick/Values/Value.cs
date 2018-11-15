using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Values
{
    /// <summary>
    /// A class containing information on a potential value which has been parsed from text. A value contains constant
    /// and variable values, which can be read and assigned for evaluation.
    /// </summary>
    public sealed class Value
    {
        private Node root;
        private List<Variable> variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="Value"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public Value(string expression)
        {
            root = new ValueParser(expression).Parse();
            variables = new List<Variable>();
            GetVariables(root.Value as IEnumerable<Node>);
        }

        #region Properties

        /// <summary>
        /// Gets whether the value is simple.
        /// </summary>
        public bool IsSimple => (root.Value as IEnumerable<Node>).Count() == 1;

        /// <summary>
        /// Gets whether the value contains any variables.
        /// </summary>
        public bool IsVariable => variables.Count > 0;

        /// <summary>
        /// Gets an immutable list of the variables required in the value.
        /// </summary>
        public IReadOnlyList<Variable> Variables => variables;

        #endregion

        /// <summary>
        /// Evaluates the value expression by processing any operators, constants and variables.
        /// </summary>
        /// <returns>The resulting value of the expression.</returns>
        public object Evaluate()
        {
            foreach (var variable in variables)
            {
                if (variable.Value == null)
                    throw new ArgumentException($"A variable '{variable.Expression}' cannot be resolved");
            }

            var typeCode = GetTypeCode(root.Value as IEnumerable<Node>);
            var result = Evaluate(root.Value as IEnumerable<Node>, typeCode);

            if (result is Constant constant)
                result = constant.Value;

            return result;
        }
        
        /// <summary>
        /// Evaluates the provided collection of nodes into a resulting value.
        /// </summary>
        /// <param name="nodes">The collection of nodes.</param>
        /// <param name="type">The expected value type.</param>
        /// <returns>The result of the nodes.</returns>
        private object Evaluate(IEnumerable<Node> nodes, TypeCode type)
        {
            foreach (var node in nodes)
            {
                if (node.Type == NodeType.Group)
                {
                    node.Value = Evaluate(node.Value as IEnumerable<Node>, type);
                    node.Type = NodeType.Constant;
                }

                if (node.Type == NodeType.Variable)
                {
                    node.Value = new Constant((node.Value as Variable).Value);
                    node.Type = NodeType.Constant;
                }
            }

            Evaluate(nodes, Operator.Modulus, type);
            Evaluate(nodes, Operator.Divide, type);
            Evaluate(nodes, Operator.Multiply, type);
            Evaluate(nodes, Operator.Subtract, type);
            Evaluate(nodes, Operator.Add, type);

            var result = nodes.First();

            if (result.Type != NodeType.Constant && result.Type != NodeType.Variable)
                throw new ArgumentException("Unable to evaluate an expression");

            return result.Value;
        }

        /// <summary>
        /// Evaluates the provided collection of nodes with the provided operator into a resulting value.
        /// </summary>
        /// <param name="nodes">The collection of nodes.</param>
        /// <param name="oper">The operator to process.</param>
        /// <param name="type">The expected value type.</param>
        /// <returns>The result of the nodes.</returns>
        private void Evaluate(IEnumerable<Node> nodes, Operator oper, TypeCode type)
        {
            var array = nodes.ToArray();

            for (int i = 1, count = array.Length; i < count; i += 2)
            {
                var node = array[i];

                if (node.Type != NodeType.Symbol || (Operator)node.Value != oper)
                    continue;

                var prev = array[i - 1];
                var next = array[i + 1];

                if (prev == null || next == null)
                    throw new ArgumentException("Expected constants at either side of operator");

                var a = EvaluateNode(prev, type);
                var b = EvaluateNode(next, type);
                var highest = (TypeCode)Math.Max((int)GetTypeCode(a), (int)GetTypeCode(b));

                if (highest == TypeCode.String && oper != Operator.Add)
                    throw new ArgumentException($"Unsupported operator {oper} for string values");

                var result = EvaluateOperation(a, b, oper, highest);
                
                prev.IsResolved = true;
                prev.Next = next;
                prev.Type = NodeType.Constant;
                prev.Value = new Constant(result);

                next.IsResolved = true;
                next.Previous = prev;
                next.Type = NodeType.Constant;
                next.Value = new Constant(result);
                
                for (prev = prev.Previous; prev != null && prev.IsResolved; prev = prev.Previous)
                    prev.Value = new Constant(result);

                for (next = next.Next; next != null && next.IsResolved; next = next.Next)
                    next.Value = new Constant(result);
            }
        }

        /// <summary>
        /// Evaluates a single node by consuming the value of the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="type">The expected value type.</param>
        /// <returns>The node value.</returns>
        private object EvaluateNode(Node node, TypeCode type)
        {
            if (node.Type == NodeType.Constant)
                return (node.Value as Constant).Value;

            if (node.Type == NodeType.Variable)
                return (node.Value as Variable).Value;

            if (node.Type == NodeType.Group)
                return Evaluate(node.Value as IEnumerable<Node>, type);

            throw new ArgumentException("Expected constant or group for evaluation");
        }

        /// <summary>
        /// Evaluates an operation by processing two values using the provided type as the highest precedence.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <param name="oper">The operator.</param>
        /// <param name="type">The value type.</param>
        /// <returns>The resulting value.</returns>
        private object EvaluateOperation(object left, object right, Operator oper, TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                    var sa = Convert.ToInt64(left);
                    var sb = Convert.ToInt64(right);
                    var sresult = default(long);
                    switch (oper)
                    {
                        case Operator.Add: sresult = sa + sb; break;
                        case Operator.Divide: sresult = sa / sb; break;
                        case Operator.Modulus: sresult = sa % sb; break;
                        case Operator.Multiply: sresult = sa * sb; break;
                        case Operator.Subtract: sresult = sa - sb; break;
                    }
                    return NumberHelper.Shrink(sresult);

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    var ua = Convert.ToUInt64(left);
                    var ub = Convert.ToUInt64(right);
                    var uresult = default(ulong);
                    switch (oper)
                    {
                        case Operator.Add: uresult = ua + ub; break;
                        case Operator.Divide: uresult = ua / ub; break;
                        case Operator.Modulus: uresult = ua % ub; break;
                        case Operator.Multiply: uresult = ua * ub; break;
                        case Operator.Subtract: uresult = ua - ub; break;
                    }
                    return NumberHelper.Shrink(uresult);

                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    var fa = Convert.ToDouble(left);
                    var fb = Convert.ToDouble(right);
                    var fresult = default(double);
                    switch (oper)
                    {
                        case Operator.Add: fresult = fa + fb; break;
                        case Operator.Divide: fresult = fa / fb; break;
                        case Operator.Modulus: fresult = fa % fb; break;
                        case Operator.Multiply: fresult = fa * fb; break;
                        case Operator.Subtract: fresult = fa - fb; break;
                    }
                    return NumberHelper.Shrink(fresult);

                case TypeCode.String:
                    var ta = Convert.ToString(left);
                    var tb = Convert.ToString(right);
                    switch (oper)
                    {
                        case Operator.Add: return ta + tb;
                        default:
                            throw new ArgumentException("Unsupported operator for string values");
                    }
            }

            throw new ArgumentException("Unsupport or unexpected operator evaluation");
        }

        /// <summary>
        /// Gets the highest type-code for the provided collection of nodes.
        /// </summary>
        /// <param name="nodes">The collection of child nodes.</param>
        /// <returns>The type-code with the highest precedence.</returns>
        private TypeCode GetTypeCode(IEnumerable<Node> nodes)
        {
            var value = (int)TypeCode.Empty;

            foreach (var node in nodes)
            {
                if (node.Type == NodeType.Constant)
                    value = Math.Max(value, (int)(node.Value as Constant).Type);

                if (node.Type == NodeType.Variable)
                    value = Math.Max(value, (int)Type.GetTypeCode((node.Value as Variable).Value.GetType()));

                if (node.Type == NodeType.Group)
                    value = Math.Max(value, (int)GetTypeCode(node.Value as IEnumerable<Node>));
            }

            var code = (TypeCode)value;

            switch (code)
            {
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.Object:
                    throw new ArgumentException("An unsupported or undetermined type code has been matched");
            }

            return code;
        }

        /// <summary>
        /// Gets the type-code for the provided value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The type-code.</returns>
        private TypeCode GetTypeCode(object value) => Type.GetTypeCode(value.GetType());

        /// <summary>
        /// Gets all variables from the root node of the value and populates the internal variable list.
        /// </summary>
        /// <param name="nodes">The collection of child nodes.</param>
        private void GetVariables(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Type == NodeType.Variable)
                    variables.Add(node.Value as Variable);

                if (node.Type == NodeType.Group)
                    GetVariables(node.Value as IEnumerable<Node>);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => ToString(root.Value as IEnumerable<Node>);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        private string ToString(IEnumerable<Node> nodes)
        {
            var builder = new StringBuilder();

            foreach (var node in nodes)
            {
                switch (node.Type)
                {
                    case NodeType.Constant:
                        builder.Append((node.Value as Constant).Value);
                        break;
                    case NodeType.Group:
                        builder.Append('(').Append(ToString(node.Value as IEnumerable<Node>)).Append(')');
                        break;
                    case NodeType.Symbol:
                        switch ((Operator)node.Value)
                        {
                            case Operator.Add:
                                builder.Append(" + ");
                                break;
                            case Operator.Divide:
                                builder.Append(" / ");
                                break;
                            case Operator.Modulus:
                                builder.Append(" % ");
                                break;
                            case Operator.Multiply:
                                builder.Append(" * ");
                                break;
                            case Operator.Subtract:
                                builder.Append(" - ");
                                break;
                        }
                        break;
                    case NodeType.Variable:
                        builder.Append((node.Value as Variable).Value);
                        break;
                }
            }

            return builder.ToString();
        }
    }
}
