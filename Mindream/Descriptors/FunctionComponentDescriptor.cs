using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mindream.Components;

namespace Mindream.Descriptors
{
    /// <summary>
    /// This class represents a function component descriptor.
    /// For example, a branch or a flip/flop is considered as a function descriptor.
    /// </summary>
    public class FunctionComponentDescriptor : ABaseComponentDescriptor
    {
        #region Fields

        /// <summary>
        /// This field stores the inputs.
        /// </summary>
        private readonly List<IComponentMemberInfo> mInputs = new List<IComponentMemberInfo>();

        /// <summary>
        /// This field stores the outputs.
        /// </summary>
        private readonly List<IComponentMemberInfo> mOutputs = new List<IComponentMemberInfo>();

        #endregion // Fields.

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name
        {
            get
            {
                return this.Type.Name;
            }
        }

        /// <summary>
        /// Gets the inputs.
        /// </summary>
        /// <value>
        /// The inputs.
        /// </value>
        public override List<IComponentMemberInfo> Inputs
        {
            get
            {
                return this.mInputs;
            }
        }

        /// <summary>
        /// Gets the outputs.
        /// </summary>
        /// <value>
        /// The outputs.
        /// </value>
        public override List<IComponentMemberInfo> Outputs
        {
            get
            {
                return this.mOutputs;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type
        {
            get; private set;
        }

        #endregion // Events.

        #region Constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionComponentDescriptor"/> class.
        /// </summary>
        /// <param name="pType">Type of the p.</param>
        public FunctionComponentDescriptor(Type pType)
            :base()
        {
            this.Type = pType;

            //// Look for inputs.
            //foreach (var lPropertyInfo in this.Type.GetProperties())
            //{
            //    if (lPropertyInfo.CanWrite)
            //    {
            //        this.Inputs.Add();
            //    }
            //}
        }

        #endregion // Constructors.

        #region Methods

        /// <summary>
        /// Creates an instance of a component.
        /// </summary>
        /// <returns>
        /// The created instance of the component.
        /// </returns>
        public override IComponent Create()
        {
            StaticMethodComponent lComponent = new StaticMethodComponent();
            lComponent.Initialize(this);
            return lComponent;
        }

        #endregion // Methods.
    }
}
