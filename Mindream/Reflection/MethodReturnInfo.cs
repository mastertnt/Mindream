namespace Mindream.Reflection
{
    /// <summary>
    ///     This class represents the end of a method (added by the user).
    ///     Sometimes, it is necessary to have different branchs of returns (e.g True or False)
    /// </summary>
    public class MethodReturnInfo : IComponentReturnInfo
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MethodReturnInfo" /> class.
        /// </summary>
        /// <param name="pName">Id of the return.</param>
        public MethodReturnInfo(string pName)
        {
            this.Name = pName;
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        #endregion // Properties.
    }
}