using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [CompilationImplements(Interface = typeof(INotifyPropertyChanged))]
    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyChangedAttribute : Attribute, IPropertySetInterceptor, INotifyPropertyChanged, IInstanceAware
    {
        public object Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChanged(string property) => PropertyChanged?.Invoke(Instance, new PropertyChangedEventArgs(property));

        public void OnException(PropertyInterceptionArgs e, Exception ex) => throw ex;

        public void OnExit(PropertyInterceptionArgs e) => OnChanged(e.Property.Name);

        public void OnSet(PropertyInterceptionArgs e)
        {
            
        }
    }
}
