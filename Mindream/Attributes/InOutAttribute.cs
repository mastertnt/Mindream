namespace Mindream.Attributes
{
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