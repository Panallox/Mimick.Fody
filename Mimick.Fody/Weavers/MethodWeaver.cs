using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody.Weavers
{
    /// <summary>
    /// A class containing methods for weaving against a method body.
    /// </summary>
    public class MethodWeaver
    {
        private CodeWeaver code;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodWeaver"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="method">The method.</param>
        public MethodWeaver(TypeWeaver parent, MethodReference method)
        {
            Parent = parent;
            Target = method as MethodDefinition ?? method.Resolve();
        }

        #region Properties

        /// <summary>
        /// Gets the method body.
        /// </summary>
        public MethodBody Body => Target.Body;

        /// <summary>
        /// Gets the parent type weaver.
        /// </summary>
        public TypeWeaver Parent
        {
            get;
        }

        /// <summary>
        /// Gets the resolved method definition.
        /// </summary>
        public MethodDefinition Target
        {
            get;
        }

        #endregion

        /// <summary>
        /// Create a new variable definition within the method body of the provided type.
        /// </summary>
        /// <param name="type">The variable type.</param>
        /// <param name="name">The variable name.</param>
        /// <returns>A <see cref="Variable"/> value.</returns>
        public Variable CreateVariable(TypeReference type, string name = null)
        {
            var variable = new VariableDefinition(type);
            Body.Variables.Add(variable);

            if (name != null)
            {
                var debug = Target.DebugInformation;

                if (debug.Scope == null)
                    debug.Scope = new ScopeDebugInformation(Body.Instructions.First(), Body.Instructions.Last());

                debug.Scope.Variables.Add(new VariableDebugInformation(variable, name));
            }

            return new Variable(variable);
        }

        /// <summary>
        /// Gets a code weaver which can be used to weave the method body instructions.
        /// </summary>
        /// <returns>A <see cref="CodeWeaver"/> value.</returns>
        public CodeWeaver GetWeaver() => code ?? (code = new CodeWeaver(this));
    }
}
