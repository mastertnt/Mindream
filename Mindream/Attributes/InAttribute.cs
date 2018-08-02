namespace Mindream.Attributes
{
    /// <summary>
    /// Input attribute class definition.
    /// </summary>
    public class InAttribute : ParameterAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InAttribute" /> class.
        /// </summary>
        public InAttribute()
            : base(true, false)
        {
        }
    }
}