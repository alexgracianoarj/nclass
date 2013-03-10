using System;
using System.Collections.Generic;
using System.Linq;
using NReflect;
using NReflect.Filter;
using NReflect.NRAttributes;
using NReflect.NREntities;
using NReflect.NRMembers;
using NReflect.NRParameters;

namespace NClass.AssemblyImport
{
  /// <summary>
  /// This import filter will filter out all entities and members NClass doesn't
  /// understand.
  /// </summary>
  [Serializable]
  public class NClassImportFilter : IFilter
  {
    // ========================================================================
    // Fields

    #region === Fields

    /// <summary>
    /// The filter to delegate filter calls to.
    /// </summary>
    private readonly IFilter filter;

    #endregion

    // ========================================================================
    // Con- / Destruction

    #region === Con- / Destruction

    /// <summary>
    /// Initializes a new instance of <see cref="NClassImportFilter"/>.
    /// </summary>
    /// <param name="filter">The filter to delegate filter calls to.</param>
    public NClassImportFilter(IFilter filter)
    {
      this.filter = filter;
      UnsafeTypesPresent = false;
    }

    #endregion

    // ========================================================================
    // Properties

    #region === Properties

    /// <summary>
    /// Gets a value indicating if unsafe types where filtered out.
    /// </summary>
    public bool UnsafeTypesPresent { get; private set; }
    /// <summary>
    /// Gets a value indicating if there are type usages of types which are nested
    /// within generac types. These type usages are filtered out.
    /// </summary>
    public bool GenericNestingPresent { get; private set; }
    /// <summary>
    /// Gets a value indicating if there are nullable type parameters. These type parameters are filtered out.
    /// </summary>
    public bool NullableAsTypeParamPresent { get; private set; }
    /// <summary>
    /// Gets a value indicating if there are extension methods. These type parameters are filtered out.
    /// </summary>
    public bool ExtensionMethodsPresent { get; private set; }
    /// <summary>
    /// Gets a value indicating if there are type usages using a deep generic nesting. These type parameters are filtered out.
    /// </summary>
    public bool DeepGenericNestingPresent { get; private set; }

    #endregion

    // ========================================================================
    // Operators and Type Conversions

    #region === Operators and Type Conversions


    #endregion

    // ========================================================================
    // Methods

    #region === Methods

    /// <summary>
    /// Determines if a class will be reflected.
    /// </summary>
    /// <param name="nrClass">The class to test.</param>
    /// <returns>
    /// <c>True</c> if the class should be reflected.
    /// </returns>
    public bool Reflect(NRClass nrClass)
    {
      return filter.Reflect(nrClass);
    }

    /// <summary>
    /// Determines if an interface will be reflected.
    /// </summary>
    /// <param name="nrInterface">The interface to test.</param>
    /// <returns>
    /// <c>True</c> if the interface should be reflected.
    /// </returns>
    public bool Reflect(NRInterface nrInterface)
    {
      return filter.Reflect(nrInterface);
    }

    /// <summary>
    /// Determines if a struct will be reflected.
    /// </summary>
    /// <param name="nrStruct">The struct to test.</param>
    /// <returns>
    /// <c>True</c> if the struct should be reflected.
    /// </returns>
    public bool Reflect(NRStruct nrStruct)
    {
      return filter.Reflect(nrStruct);
    }

    /// <summary>
    /// Determines if a delegate will be reflected.
    /// </summary>
    /// <param name="nrDelegate">The delegate to test.</param>
    /// <returns>
    /// <c>True</c> if the delegate should be reflected.
    /// </returns>
    public bool Reflect(NRDelegate nrDelegate)
    {
      return filter.Reflect(nrDelegate) && CanImportParameters(nrDelegate.Parameters) && CanImportTypeUsage(nrDelegate.ReturnType);
    }

    /// <summary>
    /// Determines if a enum will be reflected.
    /// </summary>
    /// <param name="nrEnum">The enum to test.</param>
    /// <returns>
    /// <c>True</c> if the enum should be reflected.
    /// </returns>
    public bool Reflect(NREnum nrEnum)
    {
      return filter.Reflect(nrEnum);
    }

    /// <summary>
    /// Determines if a enum value will be reflected.
    /// </summary>
    /// <param name="nrEnumValue">The enum value to test.</param>
    /// <returns>
    /// <c>True</c> if the enum value should be reflected.
    /// </returns>
    public bool Reflect(NREnumValue nrEnumValue)
    {
      return filter.Reflect(nrEnumValue);
    }

    /// <summary>
    /// Determines if a method will be reflected.
    /// </summary>
    /// <param name="nrConstructor">The method to test.</param>
    /// <returns>
    /// <c>True</c> if the method should be reflected.
    /// </returns>
    public bool Reflect(NRConstructor nrConstructor)
    {
      return filter.Reflect(nrConstructor) && CanImportParameters(nrConstructor.Parameters);
    }

    /// <summary>
    /// Determines if a method will be reflected.
    /// </summary>
    /// <param name="nrMethod">The method to test.</param>
    /// <returns>
    /// <c>True</c> if the method should be reflected.
    /// </returns>
    public bool Reflect(NRMethod nrMethod)
    {
      return filter.Reflect(nrMethod) && CanImportParameters(nrMethod.Parameters) && CanImportTypeUsage(nrMethod.Type) && !IsExtensionMethod(nrMethod);
    }

    /// <summary>
    /// Determines if an operator will be reflected.
    /// </summary>
    /// <param name="nrOperator">The operator to test.</param>
    /// <returns>
    /// <c>True</c> if the operator should be reflected.
    /// </returns>
    public bool Reflect(NROperator nrOperator)
    {
      return filter.Reflect(nrOperator) && CanImportParameters(nrOperator.Parameters) && CanImportTypeUsage(nrOperator.Type);
    }

    /// <summary>
    /// Determines if an event will be reflected.
    /// </summary>
    /// <param name="nrEvent">The event to test.</param>
    /// <returns>
    /// <c>True</c> if the event should be reflected.
    /// </returns>
    public bool Reflect(NREvent nrEvent)
    {
      return filter.Reflect(nrEvent) && CanImportTypeUsage(nrEvent.Type);
    }

    /// <summary>
    /// Determines if a field will be reflected.
    /// </summary>
    /// <param name="nrField">The field to test.</param>
    /// <returns>
    /// <c>True</c> if the field should be reflected.
    /// </returns>
    public bool Reflect(NRField nrField)
    {
      return filter.Reflect(nrField) && CanImportTypeUsage(nrField.Type);
    }

    /// <summary>
    /// Determines if a property will be reflected.
    /// </summary>
    /// <param name="nrProperty">The property to test.</param>
    /// <returns>
    /// <c>True</c> if the property should be reflected.
    /// </returns>
    public bool Reflect(NRProperty nrProperty)
    {
      return filter.Reflect(nrProperty) && CanImportTypeUsage(nrProperty.Type);
    }

    /// <summary>
    /// Determines if an attribute will be reflected.
    /// </summary>
    /// <param name="nrAttribute">The attribute to test.</param>
    /// <returns>
    /// <c>False</c> since NClass don't know attributes.
    /// </returns>
    public bool Reflect(NRAttribute nrAttribute)
    {
        return false;
    }

    /// <summary>
    /// Determines if a module will be reflected.
    /// </summary>
    /// <param name="nrModule">The module to test.</param>
    /// <returns>
    /// <c>False</c> since NClass don't know modules.
    /// </returns>
    public bool Reflect(NRModule nrModule)
    {
        return false;
    }

    /// <summary>
    /// Determines if NClass can handle a specific type usage.
    /// </summary>
    /// <param name="nrTypeUsage">The type usage to check.</param>
    /// <returns><c>True</c> if the type usage can be imported to NClass.</returns>
    private bool CanImportTypeUsage(NRTypeUsage nrTypeUsage)
    {
      return !IsUnsafePointer(nrTypeUsage) && !IsGenericNested(nrTypeUsage) && !IsNullableTypeParameter(nrTypeUsage) && !IsDeepGenericNesting(nrTypeUsage);
    }

    /// <summary>
    /// Determines if NClass can handle all given parameters.
    /// </summary>
    /// <param name="parameters">The parameters to check.</param>
    /// <returns><c>True</c> if the parameters can be imported to NClass.</returns>
    private bool CanImportParameters(IEnumerable<NRParameter> parameters)
    {
      return !parameters.Select(p => p.Type).Any(type => IsUnsafePointer(type) || IsGenericNested(type) || IsNullableTypeParameter(type) || IsDeepGenericNesting(type));
    }

    /// <summary>
    /// Returns <c>true</c> if the given type represents an unsafe pointer.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the given type represents an unsafe pointer.</returns>
    private bool IsUnsafePointer(NRTypeUsage type)
    {
      if(type != null && type.FullName  != null && type.FullName.Contains("*"))
      {
        UnsafeTypesPresent = true;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns <c>true</c> if the given type usage is nested within a generic type.
    /// </summary>
    /// <param name="nrTypeUsage">The type usage to check.</param>
    /// <returns><c>true</c> if the given type usage is nested within a generic type.</returns>
    private bool IsGenericNested(NRTypeUsage nrTypeUsage)
    {
      if(nrTypeUsage == null)
      {
        return false;
      }
      NRTypeUsage declaringType = nrTypeUsage.DeclaringType;
      while(declaringType != null)
      {
        if(declaringType.IsGeneric)
        {
          GenericNestingPresent = true;
          return true;
        }
        declaringType = declaringType.DeclaringType;
      }
      return false;
    }

    /// <summary>
    /// Returns <c>true</c> if the given type usage contains a nullable generic parameter.
    /// </summary>
    /// <param name="nrTypeUsage">The type usage to check.</param>
    /// <returns><c>true</c> if the given type usage contains a nullable generic parameter.</returns>
    private bool IsNullableTypeParameter(NRTypeUsage nrTypeUsage)
    {
      foreach(NRTypeUsage genericParameter in nrTypeUsage.GenericParameters)
      {
        if(genericParameter.IsNullable)
        {
          NullableAsTypeParamPresent = true;
          return true;
        }
        if(IsNullableTypeParameter(genericParameter))
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Returns <c>true</c> if the given type usage uses deep generic nesting.
    /// </summary>
    /// <param name="nrTypeUsage">The type usage to check.</param>
    /// <param name="level">The current level of nesting.</param>
    /// <returns><c>true</c> if the given type usage uses deep generic nesting.</returns>
    private bool IsDeepGenericNesting(NRTypeUsage nrTypeUsage, int level = 0)
    {
      if(nrTypeUsage.GenericParameters.Any(genericParameter => IsDeepGenericNesting(genericParameter, level + 1)))
      {
        return true;
      }
      if(level > 2)
      {
        DeepGenericNestingPresent = true;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns <c>true</c> if the given method is an extension method.
    /// </summary>
    /// <param name="method">The method to check.</param>
    /// <returns><c>true</c> if the given method is an extension method.</returns>
    private bool IsExtensionMethod(NRMethod method)
    {
      if(method.IsExtensionMethod)
      {
        ExtensionMethodsPresent = true;
        return true;
      }
      return false;
    }

    #endregion

    // ========================================================================
    // Events

    #region === Events


    #endregion
  }
}