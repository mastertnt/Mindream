using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace Mindream
{
    /// <summary>
    /// Helper class to emit types
    /// </summary>
    public static class EmitHelper
    {
        /// <summary>
        /// Creates the module.
        /// </summary>
        /// <param name="pAssemblyName">Name of the p assembly.</param>
        /// <returns></returns>
        public static ModuleBuilder CreateModule(string pAssemblyName)
        {
            AssemblyName lAssemblyName = new AssemblyName(pAssemblyName);
            AssemblyBuilder lDynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(lAssemblyName, AssemblyBuilderAccess.Run);
            return lDynamicAssembly.DefineDynamicModule(lAssemblyName.Name);
        }

        /// <summary>
        /// Creates the type with default constructor.
        /// </summary>
        /// <param name="pModule">The p module.</param>
        /// <param name="pTypeName">Name of the p type.</param>
        /// <param name="pSuperClass">The p super class.</param>
        /// <returns></returns>
        public static TypeBuilder CreateTypeWithDefaultConstructor(ModuleBuilder pModule, string pTypeName, Type pSuperClass)
        {
            TypeBuilder lDynamicType = pModule.DefineType(pTypeName, TypeAttributes.Public, pSuperClass);
            // Add default constructor
            lDynamicType.DefineDefaultConstructor(MethodAttributes.Public);

            return lDynamicType;
        }

        /// <summary>
        /// Creates the type of the enum.
        /// </summary>
        /// <param name="pModule">The module.</param>
        /// <param name="pEnumName">Name of the enum.</param>
        /// <param name="pPossibleValues">The possible values.</param>
        /// <returns></returns>
        public static Type CreateEnumType(ModuleBuilder pModule, string pEnumName, List<string> pPossibleValues)
        {
            EnumBuilder lEnumBuilder = pModule.DefineEnum(pEnumName, TypeAttributes.Public, typeof(int));

            int lValueIndex = 0;
            foreach (string lValue in pPossibleValues)
            {
                lEnumBuilder.DefineLiteral(lValue, lValueIndex++);
            }

            return lEnumBuilder.CreateType();

        }

        /// <summary>
        /// Creates the property.
        /// </summary>
        /// <param name="pTypeToAddProperty">Type in which we will add a property.</param>
        /// <param name="pPropertyName">Name of the p property.</param>
        /// <param name="pPopertyType">Type of the p poperty.</param>
        /// <param name="pAddSetter">if set to <c>true</c> [add setter].</param>
        /// <returns></returns>
        public static PropertyBuilder CreateProperty(TypeBuilder pTypeToAddProperty, string pPropertyName, Type pPopertyType, bool pAddSetter)
        {
            FieldBuilder lField = pTypeToAddProperty.DefineField("m" + pPropertyName, pPopertyType, FieldAttributes.Private);

            PropertyBuilder lPropertyBuilder = pTypeToAddProperty.DefineProperty(pPropertyName, PropertyAttributes.HasDefault, pPopertyType, null);

            // The property set and property get methods require a special
            // set of attributes.
            MethodAttributes getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // Define the "get" accessor method for CustomerName.
            MethodBuilder lGetMethodBuilder =
                pTypeToAddProperty.DefineMethod("get_" + pPropertyName,
                                           getSetAttr,
                                           pPopertyType,
                                           Type.EmptyTypes);

            ILGenerator custNameGetIL = lGetMethodBuilder.GetILGenerator();

            custNameGetIL.Emit(OpCodes.Ldarg_0);
            custNameGetIL.Emit(OpCodes.Ldfld, lField);
            custNameGetIL.Emit(OpCodes.Ret);

            lPropertyBuilder.SetGetMethod(lGetMethodBuilder);

            if (pAddSetter)
            {
                // Define the "set" accessor method for CustomerName.
                MethodBuilder lSetMethodBuilder =
                    pTypeToAddProperty.DefineMethod("set" + pPropertyName,
                                               getSetAttr,
                                               null,
                                               new Type[] { pPopertyType });

                ILGenerator custNameSetIL = lSetMethodBuilder.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, lField);
                if (pTypeToAddProperty.BaseType != null)
                {
                    MethodInfo lNotifyPropertyChangedMethod = pTypeToAddProperty.BaseType.GetMethod("NotifyPropertyChanged", new Type[1] { typeof(string) });
                    if (lNotifyPropertyChangedMethod != null)
                    {
                        custNameSetIL.Emit(OpCodes.Ldarg_0);
                        custNameSetIL.Emit(OpCodes.Ldstr, pPropertyName);
                        custNameSetIL.Emit(OpCodes.Callvirt, lNotifyPropertyChangedMethod);
                    }
                }
                custNameSetIL.Emit(OpCodes.Ret);

                // Last, we must map the two methods created above to our PropertyBuilder to 
                // their corresponding behaviors, "get" and "set" respectively. 
                lPropertyBuilder.SetSetMethod(lSetMethodBuilder);
            }

            return lPropertyBuilder;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProperty"></param>
        /// <param name="pAttributeType"></param>
        /// <param name="pAttributeParameters"></param>
        public static void AddAttributeToProperty(PropertyBuilder pProperty, Type pAttributeType, object[] pAttributeParameters)
        {
            Type[] lAttributeConstructorParams = new Type[pAttributeParameters.Length];
            for (int lParameterIndex = 0; lParameterIndex < pAttributeParameters.Length; lParameterIndex++)
            {
                lAttributeConstructorParams[lParameterIndex] = pAttributeParameters[lParameterIndex].GetType();
            }

            ConstructorInfo lAttributeConstructorInfo = pAttributeType.GetConstructor(lAttributeConstructorParams);
            CustomAttributeBuilder lAttributeBuilder = new CustomAttributeBuilder(lAttributeConstructorInfo, pAttributeParameters);
            pProperty.SetCustomAttribute(lAttributeBuilder);
        }

        /// <summary>
        /// Adds an attribute 
        /// </summary>
        /// <param name="pType">The type.</param>
        /// <param name="pAttributeType">The attribute type.</param>
        /// <param name="pAttributeParameters">The attribute parameters</param>
        public static void AddAttributeToType(TypeBuilder pType, Type pAttributeType, object[] pAttributeParameters)
        {
            Type[] lAttributeConstructorParams = new Type[pAttributeParameters.Length];
            for (int lParameterIndex = 0; lParameterIndex < pAttributeParameters.Length; lParameterIndex++)
            {
                lAttributeConstructorParams[lParameterIndex] = pAttributeParameters[lParameterIndex].GetType();
            }

            ConstructorInfo lAttributeConstructorInfo = pAttributeType.GetConstructor(lAttributeConstructorParams);
            CustomAttributeBuilder lAttributeBuilder = new CustomAttributeBuilder(lAttributeConstructorInfo, pAttributeParameters);
            pType.SetCustomAttribute(lAttributeBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pTypeToAddProperty"></param>
        /// <param name="pEventName"></param>
        /// <param name="pEventType"></param>
        /// <returns></returns>
        public static EventBuilder CreateEvent(TypeBuilder pTypeToAddProperty, string pEventName, Type pEventType)
        {

            FieldBuilder lFieldBuilder = pTypeToAddProperty.DefineField("m" + pEventName, pEventType, FieldAttributes.Private);

            EventBuilder lEventBuilder = pTypeToAddProperty.DefineEvent(pEventName, EventAttributes.None, pEventType);

            MethodBuilder lAddMethod = pTypeToAddProperty.DefineMethod("add_{EventName}",
                            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                            CallingConventions.Standard | CallingConventions.HasThis,
                            typeof(void),
                            new[] { pEventType });
            ILGenerator lGenerator = lAddMethod.GetILGenerator();
            MethodInfo lCombineMethod = typeof(Delegate).GetMethod("Combine", new[] { typeof(Delegate), typeof(Delegate) });
            lGenerator.Emit(OpCodes.Ldarg_0);
            lGenerator.Emit(OpCodes.Ldarg_0);
            lGenerator.Emit(OpCodes.Ldfld, lFieldBuilder);
            lGenerator.Emit(OpCodes.Ldarg_1);
            lGenerator.Emit(OpCodes.Call, lCombineMethod);
            lGenerator.Emit(OpCodes.Castclass, pEventType);
            lGenerator.Emit(OpCodes.Stfld, lFieldBuilder);
            lGenerator.Emit(OpCodes.Ret);
            lEventBuilder.SetAddOnMethod(lAddMethod);


            MethodBuilder lRemoveMethodBuilder = pTypeToAddProperty.DefineMethod("remove_{EventName}",
                                            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
                                            CallingConventions.Standard | CallingConventions.HasThis,
                                            typeof(void),
                                            new[] { pEventType });
            MethodInfo lRemoveMethod = typeof(Delegate).GetMethod("Remove", new[] { typeof(Delegate), typeof(Delegate) });
            lGenerator = lRemoveMethodBuilder.GetILGenerator();
            lGenerator.Emit(OpCodes.Ldarg_0);
            lGenerator.Emit(OpCodes.Ldarg_0);
            lGenerator.Emit(OpCodes.Ldfld, lFieldBuilder);
            lGenerator.Emit(OpCodes.Ldarg_1);
            lGenerator.Emit(OpCodes.Call, lRemoveMethod);
            lGenerator.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            lGenerator.Emit(OpCodes.Stfld, lFieldBuilder);
            lGenerator.Emit(OpCodes.Ret);
            lEventBuilder.SetRemoveOnMethod(lRemoveMethodBuilder);

            return lEventBuilder;
        }

        /// <summary>
        /// Emits the type.
        /// </summary>
        /// <param name="pType">Type of the p.</param>
        /// <returns></returns>
        public static Type EmitType(TypeBuilder pType)
        {
            Type lNewType = pType.CreateType();
            return lNewType;
        }

    }
}
