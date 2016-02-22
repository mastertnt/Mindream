namespace Mindream.Attributes
{
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