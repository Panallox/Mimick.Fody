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
    /// An emitter class containing methods for emitting against a method.
    /// </summary>
    public class MethodEmitter
    {
        private CodeEmitter code;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodEmitter"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="method">The method.</param>
        public MethodEmitter(TypeEmitter parent, MethodReference method)
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
        /// Gets whether the method is static.
        /// </summary>
        public bool IsStatic => Target.IsStatic;

        /// <summary>
        /// Gets the parent type emitter.
        /// </summary>
        public TypeEmitter Parent
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
        public Variable EmitLocal(TypeReference type, string name = null)
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
        /// Signs the method as an override of the provided method.
        /// </summary>
        /// <param name="method">The method.</param>
        public void EmitOverride(MethodReference method)
        {
            Target.Overrides.Add(method);
        }

        /// <summary>
        /// Gets a code emitter which can be used to weave the method body instructions.
        /// </summary>
        /// <returns>A <see cref="CodeEmitter"/> value.</returns>
        public CodeEmitter GetIL() => code ?? (code = new CodeEmitter(this));
    }
}
