﻿using Mimick.Aspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostConstructAttribute : Attribute, IInjectAfterInitializer
    {

    }
}
