using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Fody.Weavers;
using Mono.Cecil;

namespace Mimick.Fody
{
    /// <summary>
    /// An emitter class containing methods for emitting against an event.
    /// </summary>
    public class EventEmitter
    {
        private MethodEmitter add;
        private MethodEmitter remove;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventEmitter"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="evt">The evt.</param>
        public EventEmitter(TypeEmitter parent, EventReference evt)
        {
            Parent = parent;
            Target = evt as EventDefinition ?? evt.Resolve();

            if (Target.AddMethod != null)
                add = new MethodEmitter(parent, Target.AddMethod);
            else
            {
                var existing = parent.Target.GetMethod($"add_{Target.Name}", parent.Context.Module.TypeSystem.Void, new[] { evt.EventType }, new GenericParameter[0]);
                if (existing != null)
                {
                    Target.AddMethod = existing.Resolve();
                    add = new MethodEmitter(parent, Target.AddMethod);
                }
            }

            if (Target.RemoveMethod != null)
                remove = new MethodEmitter(parent, Target.RemoveMethod);
            else
            {
                var existing = parent.Target.GetMethod($"remove_{Target.Name}", parent.Context.Module.TypeSystem.Void, new[] { evt.EventType }, new GenericParameter[0]);
                if (existing != null)
                {
                    Target.RemoveMethod = existing.Resolve();
                    remove = new MethodEmitter(parent, Target.RemoveMethod);
                }
            }
        }

        #region Properties

        /// <summary>
        /// Gets whether the event add method exists.
        /// </summary>
        public bool HasAdd => add != null;

        /// <summary>
        /// Gets whether the event remove method exists.
        /// </summary>
        public bool HasRemove => remove != null;
        
        /// <summary>
        /// Gets the parent emitter.
        /// </summary>
        public TypeEmitter Parent { get; }

        /// <summary>
        /// Gets the event.
        /// </summary>
        public EventDefinition Target { get; }

        #endregion

        /// <summary>
        /// Gets or creates the <c>add</c> method emitter.
        /// </summary>
        /// <returns></returns>
        public MethodEmitter GetAdd()
        {
            if (add != null)
                return add;
            
            var attributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final;
            var method = new MethodDefinition($"add_{Target.Name}", attributes, Parent.Context.Module.TypeSystem.Void);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Target.EventType.Import()));
            Target.AddMethod = method;
            Parent.Target.Methods.Add(method);
            Parent.Context.AddCompilerGenerated(method);

            return add = new MethodEmitter(Parent, method);
        }

        /// <summary>
        /// Gets or creates the <c>remove</c> method emitter.
        /// </summary>
        /// <returns></returns>
        public MethodEmitter GetRemove()
        {
            if (remove != null)
                return remove;

            var attributes = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final;
            var method = new MethodDefinition($"remove_{Target.Name}", attributes, Parent.Context.Module.TypeSystem.Void);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Target.EventType.Import()));
            Target.RemoveMethod = method;
            Parent.Target.Methods.Add(method);
            Parent.Context.AddCompilerGenerated(method);

            return remove = new MethodEmitter(Parent, method);
        }
    }
}
