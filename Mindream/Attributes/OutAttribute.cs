namespace Mindream.Attributes
{
    /// <summary>
    /// Output attribute class definition.
    /// </summary>
    public class OutAttribute : ParameterAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OutAttribute" /> class.
        /// </summary>
        public OutAttribute()
            : base(false, true)
        {
        }
    }
}