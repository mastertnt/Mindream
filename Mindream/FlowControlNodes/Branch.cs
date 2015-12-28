namespace Mindream.FlowControlNodes
{
    /// <summary>
    ///     The Branch node serves as a simple way to create decision-based flow from a single true/false condition.
    ///     Once executed, the Branch node looks at the incoming value of the attached Boolean, and outputs an execution pulse
    ///     down the appropriate output.
    /// </summary>
    [GenerateComponentDescriptor]
    public class Branch : AComponent
    {
        #region Inputs

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Branch" /> is condition.
        /// </summary>
        /// <value>
        ///     <c>true</c> if condition; otherwise, <c>false</c>.
        /// </value>
        //[Input]
        public bool Condition { get; set; }

        #endregion // Inputs

        #region Fields

        #endregion // Fields.

        #region Events

        /// <summary>
        ///     This event is raised when a false is computed.
        /// </summary>
        public MethodEnd False;

        /// <summary>
        ///     This event is raised when a true is computed.
        /// </summary>
        public MethodEnd True;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        public override void StartProtected()
        {
            this.Stop();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        public override void StopProtected()
        {
            if (Condition)
            {
                if (this.True != null)
                {
                    this.True();
                }
            }
            else
            {
                if (this.False != null)
                {
                    this.False();
                }
            }
        }

        #endregion // Methods
    }
}