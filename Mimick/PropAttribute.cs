using Mimick.Aspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PropAttribute : Attribute, IPropertyGetInterceptor, IPropertySetInterceptor
    {
        public void OnException(PropertyInterceptionArgs e, Exception ex)
        {
            throw ex;
        }

        public void OnExit(PropertyInterceptionArgs e)
        {
            
        }

        public void OnGet(PropertyInterceptionArgs e)
        {
            if (e.Value == null)
                e.Value = "Goodbye world";
        }

        public void OnSet(PropertyInterceptionArgs e)
        {
            Console.WriteLine($"The property is being updated to '{e.Value}'");
        }
    }
}
