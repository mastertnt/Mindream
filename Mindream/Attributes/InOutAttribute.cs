namespace Mindream.Attributes
{
    /// <summary>
    /// Varying attribute class definition.
    /// </summary>
    public class InOutAttribute : ParameterAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InOutAttribute" /> class.
        /// </summary>
        public InOutAttribute()
            : base(true, true)
        {
        }
    }
}