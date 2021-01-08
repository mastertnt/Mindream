using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;
using Mindream.Attributes;

namespace Mindream.Components
{
    /// <summary>
    /// This class defines an emitable component.
    /// </summary>
    public class EmitableComponent
    {
        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the category of the template.
        /// </summary>
        public string Category
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets or sets the initialization script.
        ///// </summary>
        //public string InitializationScript
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the update script.
        ///// </summary>
        //public string UpdateScript
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// Gets or sets the termination script.
        ///// </summary>
        //public string TerminationScript
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets all the input parameters.
        /// </summary>
        public Dictionary<string, string> Inputs
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EmitableComponent()
        {
            this.Inputs = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes the type from a file.
        /// </summary>
        /// <param name="pFilename">The filename.</param>
        /// <returns>True if initialization succeed, false otherwise.</returns>
        public bool Initialize(string pFilename)
        {
            XElement lScriptRoot = XElement.Load(pFilename);
            if (lScriptRoot.Name == "BehaviorDescriptor")
            {
                XElement lNameElt = lScriptRoot.Element("Name");
                if (lNameElt != null)
                {
                    this.Name = lNameElt.Value;
                    XElement lCategoryElt = lScriptRoot.Element("Category");
                    this.Category = lCategoryElt == null ? "Others" : lCategoryElt.Value;
                    if (lScriptRoot.Element("Inputs") != null)
                    {
                        foreach (var lInput in lScriptRoot.Element("Inputs").Elements("Input"))
                        {
                            XElement lInputNameElt = lInput.Element("Name");
                            XElement lInputTypeElt = lInput.Element("Type");
                            if (lInputNameElt != null && lInputTypeElt != null)
                            {
                                this.Inputs.Add(lInputNameElt.Value, lInputTypeElt.Value);
                            }
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Builds a type from the template.
        /// </summary>
        /// <param name="pModuleBuilder">The module builder.</param>
        /// <returns>The built type.</returns>
        public Type BuildType(ModuleBuilder pModuleBuilder)
        {
            TypeBuilder lScriptClass = EmitHelper.CreateTypeWithDefaultConstructor(pModuleBuilder, this.Name, typeof(EmitComponent));
            EmitHelper.AddAttributeToType(lScriptClass, typeof(FunctionComponentAttribute), new object[] { this.Category });
            foreach (var lInput in this.Inputs)
            {
                PropertyBuilder lProperty = null;
                Type lInputType = Type.GetType(lInput.Value);
                lProperty = EmitHelper.CreateProperty(lScriptClass, lInput.Key, lInputType, true);
                EmitHelper.AddAttributeToProperty(lProperty, typeof(InOutAttribute), new object[] { });
            }
            return EmitHelper.EmitType(lScriptClass);
        }
    }
}
