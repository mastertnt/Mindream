using System;

namespace Mindream
{
    /// <summary>
    /// This attribute can be used to expose a static method as a static method in the registry.
    /// This attribute will create a static method component descriptor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class StaticMethodComponentAttribute : Attribute
    {
    }
}
