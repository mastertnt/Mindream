﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Mindream
{
    /// <summary>
    /// This class represents a static method descritor.
    /// For example, double Math.Sin(double)
    /// </summary>
    public class StaticMethodComponentDescriptor : IComponentDescriptor
    {
        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>
        /// The inputs.
        /// </value>
        public List<ParameterInfo> Inputs
        {
            get
            {
                return this.Method.GetParameters().ToList();
            }
        }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        public List<ParameterInfo> Outputs
        {
            get
            {
                List<ParameterInfo> lOutputs = this.Method.GetParameters().Where(pParemeter => pParemeter.IsOut).ToList();
                lOutputs.Add(this.Method.ReturnParameter);
                return lOutputs;
            }
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public MethodInfo Method
        {
            get; private set;
        }

        #endregion // Events.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticMethodComponent"/> class.
        /// </summary>
        public StaticMethodComponentDescriptor(MethodInfo pMethod)
        {
            this.Method = pMethod;
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <returns>
        /// The created instance of the component.
        /// </returns>
        public IComponent Create()
        {
            return new StaticMethodComponent(this);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder lBuilder = new StringBuilder();
            lBuilder.AppendLine(this.Name);
            lBuilder.AppendLine("in:");
            foreach (var lInput in this.Inputs)
            {
                lBuilder.AppendLine("(" +lInput.ParameterType + ") " + lInput.Name);
            }
            lBuilder.AppendLine("out:");
            foreach (var lOuput in this.Outputs)
            {
                lBuilder.AppendLine("(" + lOuput.ParameterType + ") " + lOuput.Name);
            }
            return lBuilder.ToString();
        }

        #endregion // Methods.
    }
}
