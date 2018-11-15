using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// Indicates that the associated member should introduce new members to the declaring type.
    /// </summary>
    public abstract class IntroducesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntroducesAttribute" /> class.
        /// </summary>
        public IntroducesAttribute()
        {

        }

        #region Properties

        /// <summary>
        /// Gets or sets the member associated with the attribute.
        /// </summary>
        public MemberInfo DeclaringMember
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the declaring type associated with the attribute.
        /// </summary>
        public Type DeclaringType
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Called when the attribute has been instantiated and the declaring members information has been populated.
        /// </summary>
        public virtual void OnInitialize()
        {

        }
    }
}
