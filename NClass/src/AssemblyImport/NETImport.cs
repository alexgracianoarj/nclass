using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using NClass.Core;
using NClass.DiagramEditor.ClassDiagram;
using NClass.DiagramEditor.ClassDiagram.Connections;
using NClass.DiagramEditor.ClassDiagram.Shapes;

namespace NClass.AssemblyImport
{
  /// <summary>
  /// TODO: 
  /// - Relationships between entities
  ///   - Done: Generalization and Realization (DONE)
  ///   - Nested types (DONE)
  /// - Classes / Interfaces / Structs: 
  ///   - Fields:
  ///     - const (not available in NClass.Core yet)
  ///   - Methods:
  ///     - out / ref parameters (DONE)
  ///     - params (DONE)
  ///     - operators (DONE)
  ///     - conversion operators (DONE)
  ///     - constructor (DONE)
  ///     - destructor (Not sure how to get this. A destructor becomes a
  ///       normal method called Finalize in the assembly)
  ///   - Properties:
  ///     - this (DONE)
  ///     - (access modifier)
  ///   - events (DONE)
  ///   - all the overide, new stuff (DONE)
  /// - Delegates (DONE)
  /// </summary>
  /// <remarks>
  /// This is a brief description of the import progress.
  /// 
  /// The import is done in two passes:
  /// <ol>
  ///   <li>Reflect all types of the assembly</li>
  ///   <li>Create all dependencies between them</li>
  /// </ol>
  /// In the first pass all types of the assembly are taken into NClass and
  /// an internal dictionaries. While doing that, additionaly dictionaries
  /// get set up which hold some relationships.
  /// 
  /// In the second pass all the relationships in the dictionaries get added.
  /// </remarks>
  public class NETImport
  {
    #region === Construction

    /// <summary>
    /// Creates a new instance of NETImort.
    /// </summary>
    /// <param name="Destination">An instance of a Project to which all entities
    ///                           are added.</param>
    public NETImport(Diagram Destination, ImportSettings ImportSettings)
    {
      xProject = Destination;
      xImportSettings = ImportSettings;

      dstOperatorMethodsMap.Add("op_UnaryPlus", "operator +");
      dstOperatorMethodsMap.Add("op_UnaryNegation", "operator -");
      dstOperatorMethodsMap.Add("op_LogicalNot", "operator !");
      dstOperatorMethodsMap.Add("op_OnesComplement", "operator ~");
      dstOperatorMethodsMap.Add("op_Increment", "operator ++");
      dstOperatorMethodsMap.Add("op_Decrement", "operator --");
      dstOperatorMethodsMap.Add("op_True", "operator true");
      dstOperatorMethodsMap.Add("op_False", "operator false");
      dstOperatorMethodsMap.Add("op_Addition", "operator +");
      dstOperatorMethodsMap.Add("op_Subtraction", "operator -");
      dstOperatorMethodsMap.Add("op_Multiply", "operator *");
      dstOperatorMethodsMap.Add("op_Division", "operator /");
      dstOperatorMethodsMap.Add("op_Modulus", "operator %");
      dstOperatorMethodsMap.Add("op_BitwiseAnd", "operator &");
      dstOperatorMethodsMap.Add("op_BitwiseOr", "operator |");
      dstOperatorMethodsMap.Add("op_ExclusiveOr", "operator ^");
      dstOperatorMethodsMap.Add("op_LeftShift", "operator <<");
      dstOperatorMethodsMap.Add("op_RightShift", "operator >>");
      dstOperatorMethodsMap.Add("op_Equality", "operator ==");
      dstOperatorMethodsMap.Add("op_Inequality", "operator !=");
      dstOperatorMethodsMap.Add("op_LessThan", "operator <");
      dstOperatorMethodsMap.Add("op_GreaterThan", "operator >");
      dstOperatorMethodsMap.Add("op_LessThanOrEqual", "operator <=");
      dstOperatorMethodsMap.Add("op_GreaterThanOrEqual", "operator >=");
    }

    #endregion

    #region === Constants

    /// <summary>
    /// Bindingflags which reflect every member.
    /// </summary>
    private const BindingFlags xStandardBindingFlags = BindingFlags.NonPublic | 
                                                       BindingFlags.Public |
                                                       BindingFlags.Static |
                                                       BindingFlags.Instance;

    #endregion

    #region === Fields

    /// <summary>
    /// The imported entities get added to this project.
    /// </summary>
    private Diagram xProject;
    /// <summary>
    /// These settings describe which entities or elements should be imported.
    /// </summary>
    private ImportSettings xImportSettings = new ImportSettings();
    /// <summary>
    /// Mapping from operator-method to operator.
    /// </summary>
    private Dictionary<string, string> dstOperatorMethodsMap = new Dictionary<string, string>();

    /// <summary>
    /// Takes mappings from the fullname of a type to the generated NClass-
    /// entity.
    /// </summary>
    private Dictionary<string, IEntity> entities;
    /// <summary>
    /// Takes mappings from the fullname of a type to the reflected System.Type.
    /// </summary>
    private Dictionary<string, Type> types;
    /// <summary>
    /// Takes mappings from the fullname of the nesting type (key) to the
    /// fullname of the nested type (value).
    /// </summary>
    private Dictionary<string, string> nestings;
    /// <summary>
    /// Takes mappings from the fullname of the child-class (key) to a list 
    /// of the fullnames of the base-class or base interfaces (value).
    /// </summary>
    private Dictionary<string, List<string>> generalizations;
    /// <summary>
    /// Takes mappings from the fullname of the implementing type (key) to
    /// a list of fullnames of the implemented interfaces (value).
    /// </summary>
    private Dictionary<string, List<string>> implementations;

    #endregion

    #region === Properties

    /// <summary>
    /// Gets an instance of NETImportSettings.
    /// </summary>
    public ImportSettings ImportSettings
    {
      get { return xImportSettings; }
    }

    #endregion

    #region === Methods

    #region +++ Pass 1 - Types

    /// <summary>
    /// Imports the assembly mentioned in <paramref name="FileName"/>.
    /// </summary>
    /// <param name="FileName">The name of the assemblyfile to import.</param>
    public void ImportAssembly(string FileName)
    {
      if(string.IsNullOrEmpty(FileName))
      {
        MessageBox.Show("No assembly file to import.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      try
      {
         Assembly xNewAssembly = Assembly.LoadFrom(FileName);

        //Not needed until loading the Assembly into a new AppDomain doesn't work...
//         FileInfo fileInfo = new FileInfo(FileName);
// 
//         AppDomainSetup setup = new AppDomainSetup();
//         setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;//fileInfo.DirectoryName;
//         setup.PrivateBinPath = fileInfo.DirectoryName;//AppDomain.CurrentDomain.BaseDirectory;
//         setup.ApplicationName = "AssemblyLoader";
//         setup.ShadowCopyFiles = "true";
//         AppDomain xNewAppDomain = AppDomain.CreateDomain("TempDomain", null, setup);
// //        AssemblyLoader.AssemblyLoader xAssemblyLoader = (AssemblyLoader.AssemblyLoader)xNewAppDomain.CreateInstanceAndUnwrap("AssemblyLoader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "AssemblyLoader.AssemblyLoader");
//         AssemblyLoader.AssemblyLoader xAssemblyLoader = (AssemblyLoader.AssemblyLoader)xNewAppDomain.CreateInstanceAndUnwrap("AssemblyLoader", "AssemblyLoader.AssemblyLoader");
// 
//         Assembly xNewAssembly = xAssemblyLoader.Load(FileName);
// 
//         AppDomain.Unload(xNewAppDomain);




        //Prepare dictionaries
        entities = new Dictionary<string, IEntity>();
        types = new Dictionary<string, Type>();
        nestings = new Dictionary<string, string>();
        generalizations = new Dictionary<string, List<string>>();
        implementations = new Dictionary<string, List<string>>();

        xProject.RedrawSuspended = true;
        Type[] axTypes = xNewAssembly.GetTypes();
        ReflectTypes(axTypes);
		ArrangeTypes();

        CreateNestingRelationships();
        CreateGeneralizationRelationships();
        CreateRealizationRelationship();
      }
      catch(ReflectionTypeLoadException)
      {
          MessageBox.Show("Could not open assembly due to missing referenced assemblys", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch(BadImageFormatException)
      {
          MessageBox.Show("Could not load assembly since it seems to be not an assembly...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
          xProject.RedrawSuspended = false;
      }
//       catch(Exception ex)
//       {
//         MessageBox.Show("Error while importing!\n\n" + ex.ToString() + "\n\nNote 1: Managed C++ assemblies are not supported (yet).\nNote 2: Assemblys which are created with a fuscator might also fail to import.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//       }
    }

	private void ArrangeTypes()
	{
		const int Margin = Connection.Spacing * 2;
		const int DiagramPadding = Shape.SelectionMargin;

		int shapeCount = xProject.ShapeCount;
		int columns = (int) Math.Ceiling(Math.Sqrt(shapeCount * 2));
		int shapeIndex = 0;
		int top = Shape.SelectionMargin;
		int maxHeight = 0;

		foreach (Shape shape in xProject.Shapes)
		{
			int column = shapeIndex % columns;

			shape.Location = new Point(
				(TypeShape.DefaultWidth + Margin) * column + DiagramPadding, top);

			maxHeight = Math.Max(maxHeight, shape.Height);
			if (column == columns - 1)
			{
				top += maxHeight + Margin;
				maxHeight = 0;
			}
			shapeIndex++;
		}
	}

    /// <summary>
    /// Reflect all given types and create NClass-entities.
    /// </summary>
    /// <param name="Types">The types to reflect.</param>
    private void ReflectTypes(Type[] Types)
    {
      foreach(Type xType in Types)
      {
        //There are some compiler generated nested classes which should not
        //be imported. All these magic classes have the CompilerGeneratedAttribute.
        if(IsTypeCompilerGenerated(xType))
        {
          continue;
        }
        if(xType.IsClass)
        {
          //Could be a delegate
          if(xType.BaseType == typeof(MulticastDelegate))
          {
            ReflectDelegate(xType);
          }
          else
          {
            ReflectClass(xType);
          }
        }
        if(xType.IsInterface)
        {
          ReflectInterface(xType);
        }
        if(xType.IsEnum)
        {
          ReflectEnum(xType);
        }
        if(xType.IsValueType && !xType.IsEnum)
        {
          ReflectStruct(xType);
        }
      }
    }

    #endregion

    #region +++ Pass 2 - Relationships

    /// <summary>
    /// Creates all nested relationships. Uses the inforamtion stored in the nestings
    /// dictionary which is filled in the first pass.
    /// </summary>
    private void CreateNestingRelationships()
    {
      foreach(string stType in nestings.Keys)
      {
        if(entities.ContainsKey(nestings[stType]))
        {
          xProject.AddNesting((CompositeType) entities[nestings[stType]],
            (TypeBase) entities[stType]);
          //Repair access modifier (might not be set correctly at the first
          //phase)
          ((TypeBase)entities[stType]).AccessModifier = GetTypeAccessModifier(types[stType]);
        }
      }
    }

    /// <summary>
    /// Creates all generalization relationships. Uses information stored in the
    /// generalizations dictionary which is filled in the first pass.
    /// </summary>
    private void CreateGeneralizationRelationships()
    {
      foreach(string stChildTypeName in generalizations.Keys)
      {
        if(generalizations.ContainsKey(stChildTypeName))
        {
          foreach(string stBaseType in generalizations[stChildTypeName])
          {
            if(!string.IsNullOrEmpty(stBaseType) && entities.ContainsKey(stBaseType))
            {
              if(!IsInterfaceAlreadyImplemented(stChildTypeName, stBaseType, 0))
              {
                xProject.AddGeneralization((CompositeType) entities[stChildTypeName],
                  (CompositeType) entities[stBaseType]);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// Creates all realization relationships. Uses information stored in the 
    /// implementations dictionary which is filled in the first pass.
    /// </summary>
    private void CreateRealizationRelationship()
    {
      foreach(string stType in implementations.Keys)
      {
        if(entities.ContainsKey(stType))
        {
          foreach(string stInterface in implementations[stType])
          {
            if(entities.ContainsKey(stInterface))
            {
              if(!IsInterfaceAlreadyImplemented(stInterface, stType, 0))
              {
                xProject.AddRealization((TypeBase) entities[stType],
                  (InterfaceType) entities[stInterface]);
              }
            }
          }
        }
      }
    }

    #endregion

    #region +++ Reflect entities

    /// <summary>
    /// Reflects the class <paramref name="xType"/>.
    /// </summary>
    /// <param name="xType">A type with informations about the class which gets reflected.</param>
    private void ReflectClass(Type xType)
    {
      if(!xImportSettings.CheckImportClass(xType))
      {
        return;
      }
      ClassType xNewClass = xProject.AddClass();
      ReflectFields(xType, (CompositeType)xNewClass);
    }

    /// <summary>
    /// Reflects the delegate <paramref name="xType"/>.
    /// </summary>
    /// <param name="xType">A type with informations about the delegate which gets reflected.</param>
    private void ReflectDelegate(Type xType)
    {
      if(!xImportSettings.CheckImportDelegate(xType))
      {
        return;
      }
      DelegateType xNewDelegate = xProject.AddDelegate();
      MethodInfo xMethodInfo = xType.GetMethod("Invoke");
      xNewDelegate.ReturnType = xMethodInfo.ReturnType.Name;
      foreach(ParameterInfo xParameter in xMethodInfo.GetParameters())
      {
        xNewDelegate.AddParameter(xParameter.ParameterType.Name + " " + xParameter.Name);
      }
      ReflectTypeBase(xType, xNewDelegate);
    }

    /// <summary>
    /// Reflects the interface <paramref name="xType"/>.
    /// </summary>
    /// <param name="xType">A type with informations about the interface which gets reflected.</param>
    private void ReflectInterface(Type xType)
    {
      if(!xImportSettings.CheckImportInterface(xType))
      {
        return;
      }
      InterfaceType xNewInterface = xProject.AddInterface();
      ReflectOperations(xType, (CompositeType)xNewInterface);
    }

    /// <summary>
    /// Reflects the struct <paramref name="xType"/>.
    /// </summary>
    /// <param name="xType">A type with informations about the struct which gets reflected.</param>
    private void ReflectStruct(Type xType)
    {
      if(!xImportSettings.CheckImportStruct(xType))
      {
        return;
      }
      StructureType xNewStruct = xProject.AddStructure();
      ReflectFields(xType, (CompositeType)xNewStruct);
    }

    /// <summary>
    /// Reflects the enum <paramref name="xType"/>.
    /// </summary>
    /// <param name="xType">A type with informations about the enum which gets reflected.</param>
    private void ReflectEnum(Type xType)
    {
      if(!xImportSettings.CheckImportEnum(xType))
      {
        return;
      }
      EnumType xNewEnum = xProject.AddEnum();
      FieldInfo[] axFields = xType.GetFields(xStandardBindingFlags);
      foreach(FieldInfo xField in axFields)
      {
        //Sort this special field out
        if(xField.Name == "value__")
        {
          continue;
        }
        xNewEnum.AddValue(xField.Name);
      }
      ReflectTypeBase(xType, (TypeBase)xNewEnum);
    }

    #endregion

    #region +++ Reflect member and types

    /// <summary>
    /// Reflects all fields within the type <paramref name="xType"/>. Reflected
    /// fields are added to <paramref name="xFieldContainer"/>.
    /// </summary>
    /// <param name="xType">The fields are taken from this type.</param>
    /// <param name="xFieldContainer">Reflected fields are added to this FieldContainer.</param>
    private void ReflectFields(Type xType, CompositeType xFieldContainer)
    {
      #region --- Events

      EventInfo[] axEvents = xType.GetEvents(xStandardBindingFlags);
      List<string> astEvents = new List<string>();
      foreach(EventInfo xEvent in axEvents)
      {
        //Don't display derived events
        if(xEvent.DeclaringType != xType)
        {
          continue;
        }
        //The access modifier isn't stored at the event. So we have to take
        //that from the corresponding add_XXX (or perhaps remove_XXX) method.
        MethodInfo xAddMethodInfo = xType.GetMethod("add_" + xEvent.Name, xStandardBindingFlags);
        if(!xImportSettings.CheckImportEvent(xAddMethodInfo))
        {
          continue;
        }
        Event xNewEvent = xFieldContainer.AddEvent();
        xNewEvent.Name = xEvent.Name;
        xNewEvent.AccessModifier = GetMethodAccessModifier(xAddMethodInfo);
        xNewEvent.IsStatic = xAddMethodInfo.IsStatic;
        xNewEvent.Type = GetTypeName(xEvent.EventHandlerType);
        astEvents.Add(xEvent.Name);
      }

      #endregion

      #region --- Fields

      FieldInfo[] axFields = xType.GetFields(xStandardBindingFlags);
      foreach(FieldInfo xField in axFields)
      {
        if(!xImportSettings.CheckImportField(xField))
        {
          continue;
        }
        //Don't import fields belonging to events
        if(astEvents.Contains(xField.Name))
      	{
          continue;
      	}
        //Don't display derived fields
        if(xField.DeclaringType != xType)
        {
          continue;
        }

				//Don't add compiler generated fields (thaks to Luca)
				if(xField.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length > 0)
				{
					continue;
				}

        Field xNewField = xFieldContainer.AddField();
        xNewField.Name = xField.Name;
        xNewField.AccessModifier = GetFieldAccessModifier(xField);
        xNewField.IsReadonly = xField.IsInitOnly;
        xNewField.IsStatic = xField.IsStatic;
        if(xField.IsLiteral)
        {
          xNewField.InitialValue = xField.GetValue(null).ToString();
          xNewField.IsStatic = false;
          xNewField.IsConstant = true;
        }
        xNewField.Type = GetTypeName(xField.FieldType);
      }

      #endregion

      ReflectOperations(xType, (CompositeType)xFieldContainer);
    }

    /// <summary>
    /// Reflects all operations within the type <paramref name="xType"/>. Reflected
    /// operations are added to <paramref name="xOpContainer"/>.
    /// </summary>
    /// <param name="xType">The operations are taken from this type.</param>
    /// <param name="xOpContainer">Reflected operations are added to this OperationContainer.</param>
    private void ReflectOperations(Type xType, CompositeType xOpContainer)
    {
      #region --- Properties

      PropertyInfo[] axProperties = xType.GetProperties(xStandardBindingFlags);
      List<string> astPropertyNames = new List<string>();
      foreach(PropertyInfo xProperty in axProperties)
      {
        //Don't display derived Methods
        if(xProperty.DeclaringType != xType)
        {
          continue;
        }

        astPropertyNames.Add(xProperty.Name);
        StringBuilder stDeclaration = new StringBuilder();
        //The access modifier for a property isn't stored at the property.
        //We have to use the access modifier from the corresponding get_XXX /
        //set_XXX Method.
        //Well - that's not the whole truth... It's possible to create a 
        //public property with a private set method. But there could be also
        //a private property with a public get method. There's no way to get
        //this information back from the assembly.
        //Solution (for now): Take the first found access modifier as the 
        // property access modifier.
        ParameterInfo[] axIndexParameter = xProperty.GetIndexParameters();
        //get_XXX and set_XXX might be overloaded, so we have to specify the
        //index parameters as well.
        Type[] axGetIndexParamTypes = new Type[axIndexParameter.Length];
        Type[] axSetIndexParamTypes = new Type[axIndexParameter.Length +1];
        for(int i = 0; i < axIndexParameter.Length; i++)
			  {
          axGetIndexParamTypes[i] = axIndexParameter[i].ParameterType;
          axSetIndexParamTypes[i] = axIndexParameter[i].ParameterType;
        }
        axSetIndexParamTypes[axSetIndexParamTypes.Length - 1] = xProperty.PropertyType;
        string stGetMethodName = xProperty.Name.Insert(xProperty.Name.LastIndexOf('.') + 1, "get_");
        string stSetMethodName = xProperty.Name.Insert(xProperty.Name.LastIndexOf('.') + 1, "set_");
        MethodInfo xGetMethod = xType.GetMethod(stGetMethodName, xStandardBindingFlags, null, axGetIndexParamTypes, null);//, xStandardBindingFlags);
        MethodInfo xSetMethod = xType.GetMethod(stSetMethodName, xStandardBindingFlags, null, axSetIndexParamTypes, null);//, xStandardBindingFlags);

        //If one of the access methods (get_XXX or set_XXX) should be importet
        //import the property.
        if(!((xProperty.CanRead && xImportSettings.CheckImportProperty(xGetMethod)) ||
             (xProperty.CanWrite && xImportSettings.CheckImportProperty(xSetMethod))))
        {
          continue;
        }
        if(!(xOpContainer is InterfaceType))
        {
          if(xProperty.CanRead)
          {
            stDeclaration.Append(GetMethodAccessModifierString(xGetMethod));
            stDeclaration.AppendFormat(" {0}", GetOperationModifierString(xGetMethod));
            ChangeOperationModifierIfOverwritten(xType, xGetMethod, stDeclaration);
          }
          else
          {
            stDeclaration.Append(GetMethodAccessModifierString(xSetMethod));
            stDeclaration.AppendFormat(" {0}", GetOperationModifierString(xSetMethod));
            ChangeOperationModifierIfOverwritten(xType, xSetMethod, stDeclaration);
          }
        }
        stDeclaration.AppendFormat(" {0}", GetTypeName(xProperty.PropertyType));
        //Is this an Item-property (public int this[int i])
        if(axIndexParameter.Length > 0)
        {
          stDeclaration.Append(" this[");
          foreach(ParameterInfo xParameter in axIndexParameter)
          {
            stDeclaration.AppendFormat("{0} ", GetTypeName(xParameter.ParameterType));
            stDeclaration.AppendFormat("{0}, ", xParameter.Name);
          }
          //Get rid of the last ", "
          stDeclaration.Length -= 2;
          stDeclaration.Append("]");
        }
        else
        {
          stDeclaration.AppendFormat(" {0}", xProperty.Name);
        }
        stDeclaration.AppendFormat(" {0}", "{");
        if(xProperty.CanRead)
        {
          stDeclaration.AppendFormat(" {0}", "get;");
          if(xProperty.CanWrite)
          {
            if(!stDeclaration.ToString().StartsWith(GetMethodAccessModifierString(xSetMethod)) && !(xOpContainer is InterfaceType))
            {
              //! set has a different access modifier
              stDeclaration.AppendFormat(" {0}", GetMethodAccessModifierString(xSetMethod));
            }
            stDeclaration.AppendFormat(" {0}", "set;");
          }
        }
        else
        {
          //! Only set, so don't care about access modifier
          stDeclaration.AppendFormat(" {0}", "set;");
        }
        stDeclaration.AppendFormat(" {0}", "}");

        Property xNewProperty = xOpContainer.AddProperty();
        xNewProperty.InitFromString(stDeclaration.ToString());
      }

      #endregion

      #region --- Methods

      MethodInfo[] axMethods = xType.GetMethods(xStandardBindingFlags);
      foreach(MethodInfo xMethod in axMethods)
      {
        //Don't display derived Methods
        if(xMethod.DeclaringType != xType)
        {
          continue;
        }

        if(!xImportSettings.CheckImportMethod(xMethod))
        {
          continue;
        }

        //There are sometimes some magic methods like '<.ctor>b__0'. Those
        //methods are generated by the compiler and shouldn't be importet.
        //Those methods have an attribute CompilerGeneratedAttribute.
        if(xMethod.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length > 0)
        {
          continue;
        }

        //We store the method name here so it is much easier to take care about operators
        string stMethodName = xMethod.Name;
        bool boAddReturnType = true;
        if(xMethod.IsSpecialName)
        {
          //SpecialName means that this method is an automaticaly generated
          //method. This can be get_XXX and set_XXX for properties or
          //add_XXX and remove_XXX for events. There are also special name
          //methods starting with op_ for operators.
          if(!xMethod.Name.StartsWith("op_"))
          {
            continue;
          }
          //!xMethod.Name starts witch 'op_' and so it is an operator. We
          //have to get the 'real' method name here.
          if(dstOperatorMethodsMap.ContainsKey(stMethodName))
          {
            stMethodName = dstOperatorMethodsMap[stMethodName];
          }

          if(stMethodName == "op_Implicit")
          {
            stMethodName = "implicit operator " + GetTypeName(xMethod.ReturnType);
            boAddReturnType = false;
          }
          else if(stMethodName == "op_Explicit")
          {
            stMethodName = "explicit operator " + GetTypeName(xMethod.ReturnType);
            boAddReturnType = false;
          }
        }

        Method xNewMethod = (Method)xOpContainer.AddMethod();
        StringBuilder stDeclaration = new StringBuilder();

        if(!(xOpContainer is InterfaceType))
        {
          stDeclaration.AppendFormat("{0} ", GetMethodAccessModifierString(xMethod));
          stDeclaration.AppendFormat("{0} ", GetOperationModifierString(xMethod));
        }

        ChangeOperationModifierIfOverwritten(xType, xMethod, stDeclaration);

        if(boAddReturnType)
        {
          stDeclaration.AppendFormat("{0} ", GetTypeName(xMethod.ReturnType));
        }
        stDeclaration.AppendFormat("{0}", stMethodName);

        stDeclaration.AppendFormat("{0}", GetParameterDeclarationString(xMethod));

        xNewMethod.InitFromString(stDeclaration.ToString());
      }

      #endregion

      #region --- Constructors

      if(xOpContainer.SupportsConstuctors)
      {
        ConstructorInfo[] axConstructors = xType.GetConstructors(xStandardBindingFlags);
        foreach(ConstructorInfo xConstructor in axConstructors)
        {
          if(!xImportSettings.CheckImportConstructor(xConstructor))
          {
            continue;
          }
          Method xNewConstructor = xOpContainer.AddConstructor();
          StringBuilder stDeclaration = new StringBuilder();
          stDeclaration.AppendFormat("{0} ", GetMethodAccessModifierString(xConstructor));
          stDeclaration.AppendFormat("{0}", xConstructor.IsStatic ? "static " : "");
          stDeclaration.Append(xOpContainer.Name); //xOpContainer.Name since the name of the class/struct/... isn't set yet

          stDeclaration.AppendFormat("{0}", GetParameterDeclarationString(xConstructor));
          xNewConstructor.InitFromString(stDeclaration.ToString());
        }
      }

      #endregion

      ReflectTypeBase(xType, (TypeBase)xOpContainer);
    }

    /// <summary>
    /// Reflect the basic type <paramref name="xType"/>. All information is
    /// stored in <paramref name="xTypeBase"/>. Also sets up the dictionaries
    /// used for the relationships.
    /// </summary>
    /// <param name="xType">The information is taken from <paramref name="xType"/>.</param>
    /// <param name="xTypeBase">All information is stored in this TypeBase.</param>
    private void ReflectTypeBase(Type xType, TypeBase xTypeBase)
    {
      xTypeBase.Name = GetTypeName(xType);
      //Might set the wrong access modifier for nested classes. Will be
      //corrected when adding the nesting relationships.
      xTypeBase.AccessModifier = GetTypeAccessModifier(xType);

      //Fill up the dictionaries
      entities.Add(xType.FullName, xTypeBase);
      types.Add(xType.FullName, xType);
      if(xType.IsNested)
      {
        //Add an entry to the nesting dictionary
        nestings.Add(xType.FullName, xType.DeclaringType.FullName);
      }
      Type[] axImplementedInterfaces = xType.GetInterfaces();
      List<string> astImplementedInterfaceNames = new List<string>();
      foreach(Type xImplementedType in axImplementedInterfaces)
      {
        //An generic interface is implemented by xType
        if(xImplementedType.FullName == null)
        {
          astImplementedInterfaceNames.Add(GetTypeName(xImplementedType));
        }
        else
        {
          astImplementedInterfaceNames.Add(xImplementedType.FullName);
        }
      }
      if(xType.IsInterface)
      {
        //In NClass, interfaces are derived from (mayby more than one) interface
        generalizations.Add(xType.FullName, astImplementedInterfaceNames);
      }
      else
      {
        implementations.Add(xType.FullName, astImplementedInterfaceNames);
      }
      //Interfaces don't have a base type
      if(xType.BaseType != null)
      {
        List<string> astBaseType = new List<string>();
        // lytico: generic types have FullName == null
        if(xType.BaseType.IsGenericType)
        {
          astBaseType.Add(xType.BaseType.Namespace + '.' + xType.BaseType.Name);
        }
        else
        {
          astBaseType.Add(xType.BaseType.FullName);
        }
        generalizations.Add(xType.FullName, astBaseType);
      }
    }

    #endregion

    #region === Help methods

    #region +++ bool Is...

    /// <summary>
    /// Tests recursiv if the <paramref name="xType"/> or its declaring type
    /// has the CompilerGeneratedAttribute.
    /// </summary>
    /// <param name="xType">The type to test</param>
    /// <returns>True, if <paramref name="xType"/> has the CompilerGeneratedAttribute.</returns>
    private bool IsTypeCompilerGenerated(Type xType)
    {
      if(xType == null)
      {
        return false;
      }
      if(xType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length > 0)
      {
        return true;
      }
      return IsTypeCompilerGenerated(xType.DeclaringType);
    }

    /// <summary>
    /// Checks if interface <paramref name="stInterfaceName"/> is already
    /// implemented by <paramref name="stType"/> or it's basetypes.
    /// </summary>
    /// <param name="stInterfaceName">The name of the interface which is checked to be already implemented.</param>
    /// <param name="stType">The type which is checked if it (or it's base type) implements <paramref name="stInterfaceName"/>.</param>
    /// <param name="level">The actual recursion level. Must be 0 (zero) at the first call.</param>
    /// <returns>True, if <paramref name="stType"/> or it's base type implements <paramref name="stInterfaceName"/>, otherwise false.</returns>
    private bool IsInterfaceAlreadyImplemented(string stInterfaceName, string stType, int level)
    {
      //Don't check this at the first run. This would be allways true.
      if(((generalizations.ContainsKey(stType) && generalizations[stType].Contains(stInterfaceName)) ||
          (implementations.ContainsKey(stType) && implementations[stType].Contains(stInterfaceName))) &&
          level > 0)
      {
        return true;
      }
      if(generalizations.ContainsKey(stType))
      {
        foreach(string stBaseType in generalizations[stType])
        {
          if(IsInterfaceAlreadyImplemented(stInterfaceName, stBaseType, level + 1))
          {
            return true;
          }
        }
      }
      if(implementations.ContainsKey(stType))
      {
        foreach(string stBaseInterface in implementations[stType])
        {
          if(IsInterfaceAlreadyImplemented(stInterfaceName, stBaseInterface, +1))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Determines if the Method <paramref name="xMethod"/> is already
    /// defined within <paramref name="xType"/> or above.
    /// </summary>
    /// <param name="xType">The type which coud define <paramref name="xMethod"/> already</param>
    /// <param name="xMethod">The method wich should be checked</param>
    /// <returns>True, if <paramref name="xMethod"/> is defined in <paramref name="xType"/> or above.</returns>
    private bool IsMethodOverwritten(Type xType, MethodInfo xMethod)
    {
      if(xType == null)
      {
        return false;
      }
      ParameterInfo[] axParameters = xMethod.GetParameters();
      Type[] axParameterTypes = new Type[axParameters.Length];
      for(int i = 0; i < axParameters.Length; i++)
      {
        axParameterTypes[i] = axParameters[i].ParameterType;
      }
      if(xType.GetMethod(xMethod.Name, axParameterTypes) != null)
      {
        return true;
      }
      return IsMethodOverwritten(xType.BaseType, xMethod);
    }

    #endregion

    #region +++ string Get...

    /// <summary>
    /// Returns a string describing the parameters of the method <typeparamref name="xMethod"/>.
    /// The string cantains the opening and closing parenthesis. It is formated
    /// like C# code.
    /// </summary>
    /// <param name="xMethod">The parameters string is generated for this method. </param>
    /// <returns>The parameter string.</returns>
    private string GetParameterDeclarationString(MethodBase xMethod)
    {
      ParameterInfo[] axParameters = xMethod.GetParameters();
      StringBuilder stDeclaration = new StringBuilder("(");
      foreach(ParameterInfo xParameter in axParameters)
      {
        if(xParameter.ParameterType.Name.EndsWith("&"))
        {
          //This is a out or ref-Parameter
          if(xParameter.IsOut)
          {
            stDeclaration.AppendFormat("out {0} ", GetTypeName(xParameter.ParameterType.GetElementType()));
          }
          else
          {
            stDeclaration.AppendFormat("ref {0} ", GetTypeName(xParameter.ParameterType.GetElementType()));
          }
        }
        else
        {
          if(xParameter.GetCustomAttributes(typeof(ParamArrayAttribute), true).Length > 0)
          {
            //This is a 'params'
            stDeclaration.Append("params ");
          }
          stDeclaration.AppendFormat("{0} ", GetTypeName(xParameter.ParameterType));
        }
        stDeclaration.AppendFormat("{0}, ", xParameter.Name);
      }
      if(axParameters.Length > 0)
      {
        stDeclaration.Length -= 2;
      }
      stDeclaration.Append(")");

      return stDeclaration.ToString();
    }

    /// <summary>
    /// Returns a string containing the name of the type <paramref name="xType"/>
    /// in C# syntax. Is especially responsible to solve problems with generic
    /// types.
    /// </summary>
    /// <param name="xType">The type name is returned for this Type.</param>
    /// <returns>The name of <paramref name="xType"/> as a string.</returns>
    private string GetTypeName(Type xType)
    {
      StringBuilder stTypeName = new StringBuilder(xType.Name);
      Type b = xType.GetElementType();
      if(xType.IsArray)
      {
        stTypeName = new StringBuilder(GetTypeName(xType.GetElementType()) + xType.Name.Substring(xType.GetElementType().Name.Length));
      }
      else if(xType.IsGenericType)
      {
        if(stTypeName.ToString().LastIndexOf('`') > 0)
        {
          //Generics get names like "List`1"
          stTypeName.Remove(stTypeName.ToString().LastIndexOf('`'), stTypeName.Length - stTypeName.ToString().LastIndexOf('`'));
        }
        Type[] axGenericArguments = xType.GetGenericArguments();
        stTypeName.Append("<");
        foreach(Type xGenericArgument in axGenericArguments)
        {
          stTypeName.AppendFormat("{0}, ", GetTypeName(xGenericArgument));
        }
        //Get rid of ", " one time
        stTypeName.Length -= 2;
        stTypeName.Append(">");
      }

      return stTypeName.ToString();
    }

    #endregion

    /// <summary>
    /// If the method <paramref name="xMethod"/> is overwritten in type
    /// <paramref name="xType"/> the operation modifiers are changed to 
    /// reflect this.
    /// </summary>
    /// <param name="xType">The type the method is declared in.</param>
    /// <param name="xMethod">The method to check.</param>
    /// <param name="stDeclaration">The declaration string which holds the
    /// declaration if the method is not overwriting another.</param>
    private void ChangeOperationModifierIfOverwritten(Type xType, MethodInfo xMethod, StringBuilder stDeclaration)
    {
      if(IsMethodOverwritten(xType.BaseType, xMethod))
      {
        if(xMethod.IsVirtual && (xMethod.Attributes & MethodAttributes.VtableLayoutMask) != MethodAttributes.VtableLayoutMask)
        {
          //override
          if(!xType.IsValueType)
          {
            //stDeclaration contains already "virtual " - which is not allowed
            //in combinition with override, so get rid of it.
            // lytico: IsAbstract added; a abstract method is virtual
            if(xMethod.IsAbstract)
            {
              stDeclaration.Replace("abstract", "");
            }
            else
            {
              stDeclaration.Replace("virtual", "");
            }
          }
          if(xMethod.IsFinal)
          {
            stDeclaration.Append(" sealed ");
          }
          stDeclaration.Append(" override ");
        }
        //It's not possible to distinguish between virtual and virtual new
        //in the assembly, because virtual methods get implicitly virtual new.
        else if(xMethod.IsVirtual)
        {
          //virtual new
          stDeclaration.Append(" new ");
        }
        else
        {
          stDeclaration.Append(" new ");
        }
      }
    }

    #region +++ Get modifiers

    /// <summary>
    /// Returns the operation modifier for <paramref name="xMethod"/>.
    /// </summary>
    /// <param name="xMethod">The operation modifiers is returned for this MethodBase.</param>
    /// <returns>The OperationModifier of <paramref name="xMethod"/>.</returns>
    private OperationModifier GetOperationModifier(MethodBase xMethod)
    {
      if(xMethod.DeclaringType.IsValueType)
      {
        return OperationModifier.None;
      }
      if(xMethod.IsAbstract)
      {
        return OperationModifier.Abstract;
      }
      // lytico: possible value is: IsFinal AND IsVirtual
      else if(xMethod.IsFinal && xMethod.IsVirtual)
      {
        return OperationModifier.None;
      }
      else if(xMethod.IsFinal)
      {
        return OperationModifier.Sealed;
      }
      else if(xMethod.IsVirtual)
      {
        return OperationModifier.Virtual;
      }
      return OperationModifier.None;
    }

    /// <summary>
    /// Returns the access modifier for the type <paramref name="xType"/>.
    /// </summary>
    /// <param name="xType">The access modifier is returned for this Type.</param>
    /// <returns>The AccessModifier of <paramref name="xType"/>.</returns>
    private AccessModifier GetTypeAccessModifier(Type xType)
    {
      if(xType.IsNested)
      {
        if(xType.IsNestedPublic)
        {
          return AccessModifier.Public;
        }
        if(xType.IsNestedPrivate)
        {
          return AccessModifier.Private;
        }
        if(xType.IsNestedAssembly)
        {
          return AccessModifier.Internal;
        }
        if(xType.IsNestedFamily)
        {
          return AccessModifier.Protected;
        }
        if(xType.IsNestedFamORAssem)
        {
          return AccessModifier.ProtectedInternal;
        }
        return AccessModifier.Default;
      }
      if(xType.IsPublic)
      {
        return AccessModifier.Public;
      }
      if(xType.IsNotPublic)
      {
        return AccessModifier.Internal;
      }
      if(!xType.IsVisible)
      {
        return AccessModifier.Internal;
      }
      return AccessModifier.Default;
    }

    /// <summary>
    /// Returns the access modifier for the field <paramref name="xField"/>.
    /// </summary>
    /// <param name="xField">The access modifier is returned for this FieldInfo.</param>
    /// <returns>The AccessModifier of <paramref name="xField"/>.</returns>
    private AccessModifier GetFieldAccessModifier(FieldInfo xField)
    {
      if(xField.IsPublic)
      {
        return AccessModifier.Public;
      }
      if(xField.IsPrivate)
      {
        return AccessModifier.Private;
      }
      if(xField.IsAssembly)
      {
        return AccessModifier.Internal;
      }
      if(xField.IsFamily)
      {
        return AccessModifier.Protected;
      }
      if(xField.IsFamilyOrAssembly)
      {
        return AccessModifier.ProtectedInternal;
      }
      return AccessModifier.Default;
    }

    /// <summary>
    /// Returns the access modifier for the method <paramref name="xMethodBase"/>.
    /// </summary>
    /// <param name="xField">The access modifier is returned for this MethodBase.</param>
    /// <returns>The AccessModifier of <paramref name="xMethodBase"/>.</returns>
    private AccessModifier GetMethodAccessModifier(MethodBase xMethodBase)
    {
      if(xMethodBase.Name.Contains(".") && !xMethodBase.IsConstructor)
      {
        //explicit interface implementation
        return AccessModifier.Default;
      }
      if(xMethodBase.IsPublic)
      {
        return AccessModifier.Public;
      }
      if(xMethodBase.IsPrivate)
      {
        return AccessModifier.Private;
      }
      if(xMethodBase.IsAssembly)
      {
        return AccessModifier.Internal;
      }
      if(xMethodBase.IsFamily)
      {
        return AccessModifier.Protected;
      }
      if(xMethodBase.IsFamilyOrAssembly)
      {
        return AccessModifier.ProtectedInternal;
      }
      return AccessModifier.Default;
    }

    /// <summary>
    /// Returns the operation modifier of <paramref name="xMethod"/> as a string
    /// in C# syntax.
    /// </summary>
    /// <param name="xMethod">The operation modifier is returned for this MethodBase.</param>
    /// <returns>The operation modifier of <paramref name="xMethod"/> as a string.</returns>
    private string GetOperationModifierString(MethodBase xMethod)
    {
      return xProject.Language.GetOperationModifierString(GetOperationModifier(xMethod), true);
    }

    /// <summary>
    /// Returns the access modifier of <paramref name="xMethodBase"/> as a string
    /// in C# syntax.
    /// </summary>
    /// <param name="xMethodBase">The access modifier is returned for this MethodBase.</param>
    /// <returns>The access modifier of <paramref name="xMethodBase"/> as a string.</returns>
    private string GetMethodAccessModifierString(MethodBase xMethodBase)
    {
      if(xMethodBase == null)
      {
        return "private";
      }
      return xProject.Language.GetAccessString(GetMethodAccessModifier(xMethodBase), true);
    }

    #endregion

    #endregion

    #endregion
  }
}
