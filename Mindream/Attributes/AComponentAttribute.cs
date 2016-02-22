using System;

namespace Mindream.Attributes
{
    public abstract class AComponentAttribute : Attribute
    {
        protected AComponentAttribute(string pCategory)
        {
            this.Category = pCategory;
        }

        public string Category
        {
            get;
            private set;
        }
    }
}