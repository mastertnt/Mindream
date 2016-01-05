﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream.Attributes
{
    public class InAttribute : ParameterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InAttribute"/> class.
        /// </summary>
        public InAttribute()
        :base(true, false)
        {
            
        }
    }
}
