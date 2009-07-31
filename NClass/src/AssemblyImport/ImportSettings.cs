using System;
using System.Collections.Generic;
using System.Text;
using NClass.Core;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;

namespace NClass.AssemblyImport
{
  /// <summary>
  /// This is needed to serialize an ArrayList of ImportSettings.
  /// </summary>
  [XmlInclude(typeof(ImportSettings))]
  public class TemplateList : ArrayList
  {
  }

  /// <summary>
  /// A collection of possible modifiers.
  /// </summary>
  public enum Modifiers
  {
    /// <summary>
    /// Means "all modifiers"
    /// </summary>
    All,
    /// <summary>
    /// Modifier private
    /// </summary>
    Private,
    /// <summary>
    /// Modifier public
    /// </summary>
    Public,
    /// <summary>
    /// Modifier protected
    /// </summary>
    Protected,
    /// <summary>
    /// Modifier internal
    /// </summary>
    Internal,
    /// <summary>
    /// Modifier protected internal
    /// </summary>
    ProtectedInternal,
    /// <summary>
    /// static elements
    /// </summary>
    Static,
    /// <summary>
    /// elements which are not static
    /// </summary>
    Instance,
  }

  /// <summary>
  /// A collection of possible elemnts
  /// </summary>
  public enum Elements
  {
    /// <summary>
    /// Means "all ellements"
    /// </summary>
    Elements,
    /// <summary>
    /// Type class
    /// </summary>
    Class,
    /// <summary>
    /// Type struct
    /// </summary>
    Struct,
    /// <summary>
    /// Type interface
    /// </summary>
    Interface,
    /// <summary>
    /// Type enum
    /// </summary>
    Enum,
    /// <summary>
    /// Type delegate
    /// </summary>
    Delegate,
    /// <summary>
    /// Member field
    /// </summary>
    Field,
    /// <summary>
    /// Member property
    /// </summary>
    Property,
    /// <summary>
    /// Member method
    /// </summary>
    Method,
    /// <summary>
    /// Member constructor
    /// </summary>
    Constructor,
    /// <summary>
    /// Member event
    /// </summary>
    Event,
  }

  /// <summary>
  /// A combination of a modifier and an element.
  /// </summary>
  public struct ModifierElement
  {
    #region === Construction

    /// <summary>
    /// Creates a new ModifierElement with the given values.
    /// </summary>
    /// <param name="theModifier">The Modifier for this rule.</param>
    /// <param name="theElement">The element for this rule</param>
    public ModifierElement(Modifiers theModifier, Elements theElement)
    {
      xElement = theElement;
      xModifier = theModifier;
    }

    #endregion

    #region === Fields

    /// <summary>
    /// The modifier for this rule.
    /// </summary>
    private Modifiers xModifier;
    /// <summary>
    /// The element for this rule.
    /// </summary>
    private Elements xElement;

    #endregion

    #region === Properties

    /// <summary>
    /// Gets or sets the modifier for this rule.
    /// </summary>
    public Modifiers Modifier
    {
      get { return xModifier; }
      set { xModifier = value; }
    }

    /// <summary>
    /// Gets or sets the element for this rule.
    /// </summary>
    public Elements Element
    {
      get { return xElement; }
      set { xElement = value; }
    }

    #endregion
  }

  /// <summary>
  /// A set of rules of things which should not be imported.
  /// </summary>
  public class ImportSettings
  {
    #region === Fields

    /// <summary>
    /// The list of import exception rules.
    /// </summary>
    private List<ModifierElement> axRules = new List<ModifierElement>();
    /// <summary>
    /// The name of this settings.
    /// </summary>
    private string stName;

    #endregion

    #region === Properties

    /// <summary>
    /// Gets or sets the name of this Settings
    /// </summary>
    public string Name
    {
      get { return stName; }
      set { stName = value; }
    }

    /// <summary>
    /// Gets the list of import exception rules.
    /// </summary>
    public List<ModifierElement> Rules
    {
      get { return axRules; }
    }

    #endregion

    #region === Methods

    /// <summary>
    /// Checks if a given combination of elemts and modifiers should be imported.
    /// </summary>
    /// <param name="Element">The element which is tested.</param>
    /// <param name="Instance">true if the element is an instance element.</param>
    /// <param name="Static">true if the element is a static element.</param>
    /// <param name="Internal">true if the element is an internal element.</param>
    /// <param name="Private">true if the element is a private element.</param>
    /// <param name="Protected">true if the element is a protected element.</param>
    /// <param name="ProtectedInternal">true if the element is a protected internal element.</param>
    /// <param name="Public">true if the element is a public element.</param>
    /// <returns>true, if the element should be imported, false otherwise.</returns>
    private bool CheckImport(Elements Element, bool Instance, bool Static, bool Internal, bool Private, bool Protected, bool ProtectedInternal, bool Public)
    {
      foreach(ModifierElement xRule in axRules)
      {
        if(xRule.Element == Elements.Elements || xRule.Element == Element)
        {
          //This rule restricts the import of this kind of ellement.
          if(xRule.Modifier == Modifiers.All ||
             (xRule.Modifier == Modifiers.Instance) && Instance ||
             (xRule.Modifier == Modifiers.Internal) && Internal ||
             (xRule.Modifier == Modifiers.Private) && Private ||
             (xRule.Modifier == Modifiers.Protected) && Protected ||
             (xRule.Modifier == Modifiers.ProtectedInternal) && ProtectedInternal ||
             (xRule.Modifier == Modifiers.Public) && Public ||
             (xRule.Modifier == Modifiers.Static) && Static)
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Checks, if the Type <paramref name="theType"/> should be imported.
    /// </summary>
    /// <param name="Element">The element which is tested.</param>
    /// <param name="theType">The type to test.</param>
    /// <returns>true, if the type should be imported, false otherwise.</returns>
    private bool CheckImportType(Elements Element, Type theType)
    {
      if(theType.IsNested)
      {
        return CheckImport(Element, false, false, theType.IsNestedAssembly, theType.IsNestedPrivate, theType.IsNestedFamily, theType.IsNestedFamORAssem, theType.IsNestedPublic);
      }
      return CheckImport(Element, false, false, theType.IsNotPublic, false, false, false, theType.IsPublic);
    }

    /// <summary>
    /// Checks, if the method <paramref name="theMethod"/> should be imported.
    /// </summary>
    /// <param name="Element">The element which is tested.</param>
    /// <param name="theMethod">The method to test.</param>
    /// <returns>true, if the method should be imported, false otherwise.</returns>
    private bool CheckImportMethod(Elements Element, MethodBase theMethod)
    {
      return CheckImport(Element, !theMethod.IsStatic, theMethod.IsStatic, theMethod.IsAssembly, theMethod.IsPrivate, theMethod.IsFamily, theMethod.IsFamilyOrAssembly, theMethod.IsPublic);
    }

    #region --- CheckImport<Element>()

    /// <summary>
    /// Checks if the class <paramref name="theClass"/> should be imported.
    /// </summary>
    /// <param name="theClass">The class which is checked.</param>
    /// <returns>true, if the class should be imported, false otherwise.</returns>
    public bool CheckImportClass(Type theClass)
    {
      return CheckImportType(Elements.Class, theClass);
    }

    /// <summary>
    /// Checks if the structure <paramref name="theStruct"/> should be imported.
    /// </summary>
    /// <param name="theClass">The structure which is checked.</param>
    /// <returns>true, if the structure should be imported, false otherwise.</returns>
    public bool CheckImportStruct(Type theStruct)
    {
      return CheckImportType(Elements.Struct, theStruct);
    }

    /// <summary>
    /// Checks if the interface <paramref name="theInterface"/> should be imported.
    /// </summary>
    /// <param name="theClass">The interface which is checked.</param>
    /// <returns>true, if the interface should be imported, false otherwise.</returns>
    public bool CheckImportInterface(Type theInterface)
    {
      return CheckImportType(Elements.Interface, theInterface);
    }

    /// <summary>
    /// Checks if the enumeration <paramref name="theEnum"/> should be imported.
    /// </summary>
    /// <param name="theClass">The enumeration which is checked.</param>
    /// <returns>true, if the enumeration should be imported, false otherwise.</returns>
    public bool CheckImportEnum(Type theEnum)
    {
      return CheckImportType(Elements.Enum, theEnum);
    }

    /// <summary>
    /// Checks if the delegate <paramref name="theDelegate"/> should be imported.
    /// </summary>
    /// <param name="theClass">The delegate which is checked.</param>
    /// <returns>true, if the delegate should be imported, false otherwise.</returns>
    public bool CheckImportDelegate(Type theDelegate)
    {
      return CheckImportType(Elements.Delegate, theDelegate);
    }

    #endregion

    #region --- CheckImport<Member>()

    /// <summary>
    /// Checks if the field <paramref name="theField"/> should be imported.
    /// </summary>
    /// <param name="theClass">The field which is checked.</param>
    /// <returns>true, if the field should be imported, false otherwise.</returns>
    public bool CheckImportField(FieldInfo theField)
    {
      return CheckImport(Elements.Field, !theField.IsStatic, theField.IsStatic, theField.IsAssembly, theField.IsPrivate, theField.IsFamily, theField.IsFamilyOrAssembly, theField.IsPublic);
    }

    /// <summary>
    /// Checks if the constructor <paramref name="theConstructor"/> should be imported.
    /// </summary>
    /// <param name="theClass">The constructor which is checked.</param>
    /// <returns>true, if the constructor should be imported, false otherwise.</returns>
    public bool CheckImportConstructor(ConstructorInfo theConstructor)
    {
      return CheckImportMethod(Elements.Constructor, theConstructor);
    }

    /// <summary>
    /// Checks if the property <paramref name="theProperty"/> should be imported.
    /// </summary>
    /// <param name="theClass">The property which is checked.</param>
    /// <returns>true, if the property should be imported, false otherwise.</returns>
    public bool CheckImportProperty(MethodInfo theProperty)
    {
      return CheckImportMethod(Elements.Property, theProperty);
    }

    /// <summary>
    /// Checks if the method <paramref name="theMethod"/> should be imported.
    /// </summary>
    /// <param name="theClass">The method which is checked.</param>
    /// <returns>true, if the method should be imported, false otherwise.</returns>
    public bool CheckImportMethod(MethodInfo theMethod)
    {
      return CheckImportMethod(Elements.Method, theMethod);
    }

    /// <summary>
    /// Checks if the event <paramref name="theEvent"/> should be imported.
    /// </summary>
    /// <param name="theClass">The event which is checked.</param>
    /// <returns>true, if the event should be imported, false otherwise.</returns>
    public bool CheckImportEvent(MethodInfo theEvent)
    {
      return CheckImportMethod(Elements.Event, theEvent);
    }

    #endregion

    /// <summary>
    /// Returns the name of the ImportSettings.
    /// </summary>
    /// <returns>The name of the ImportSettings.</returns>
    public override string ToString()
    {
      return stName;
    }

    #endregion
  }
}
