﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream.Components
{
    /// <summary>
    /// This component can be used for special case like Singleton (need Instance property in static).
    /// </summary>
    /// <seealso cref="Mindream.Components.AFunctionComponent" />
    public abstract class ASingletonFunctionComponent : AFunctionComponent
    {
    }
}