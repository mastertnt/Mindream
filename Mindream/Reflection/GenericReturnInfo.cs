using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindream.Reflection
{
    /// <summary>
    /// This class represents the end of a method.
    /// Sometimes, it is necessary to have different branchs of returns (e.g True or False)
    /// </summary>
    public class GenericReturnInfo : IComponentReturnInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        #endregion // Properties.

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericReturnInfo"/> class.
        /// </summary>
        /// <param name="pName">Id of the return.</param>
        public GenericReturnInfo(string pName)
        {
            this.Name = pName;
        }

        #endregion // Constructors.
    }
}
