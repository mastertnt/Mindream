using System;
using System.ComponentModel;
using Mindream.Attributes;

namespace Mindream.Components.Variables
{
    /// <summary>
    ///     This component cane be used to get/set a variable.
    /// </summary>
    /// <typeparam name="TNativeType">The type of the native type.</typeparam>
    public abstract class AVariableComponent<TNativeType> : AMethodComponent
    {
        #region Fields

        /// <summary>
        /// The field stores the value.
        /// </summary>
        private TNativeType mValue;

        #endregion // Fields.

        #region Inputs

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [Out]
        [Browsable(false)]
        public virtual TNativeType Get
        {
            get
            {
                return this.mValue;
            }
            private set
            {
                this.mValue = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [In]
        public virtual TNativeType Set
        {
            get
            {
                return this.mValue;
            }
            set
            {
                if (value != null)
                {
                    this.mValue = value;
                    this.NotifyPropertyChanged("Get");
                }

            }
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <value>
        /// The type of the variable.
        /// </value>
        [Browsable(false)]
        public Type VariableType
        {
            get
            {
                return typeof(TNativeType);
            }
        }

        #endregion // Inputs

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.Stop();
        }

        #endregion // Methods
    }
}