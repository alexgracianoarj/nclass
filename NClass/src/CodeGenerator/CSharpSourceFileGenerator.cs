// NClass - Free class diagram editor
// Copyright (C) 2006-2009 Balazs Tihanyi
// 
// This program is free software; you can redistribute it and/or modify it under 
// the terms of the GNU General Public License as published by the Free Software 
// Foundation; either version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// this program; if not, write to the Free Software Foundation, Inc., 
// 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using NClass.Core;
using NClass.CSharp;

using System.Linq;

namespace NClass.CodeGenerator
{
	internal sealed class CSharpSourceFileGenerator : SourceFileGenerator
	{
		/// <exception cref="NullReferenceException">
		/// <paramref name="type"/> is null.
		/// </exception>
		public CSharpSourceFileGenerator(TypeBase type, string rootNamespace, Model model)
			: base(type, rootNamespace, model)
		{
		}

		protected override string Extension
		{
			get { return ".cs"; }
		}

		protected override void WriteFileContent()
		{
            WriteHeader();
			WriteUsings();
			OpenNamespace();
			WriteType(Type);
			CloseNamespace();
		}

        private void WriteCompositeId()
        {
            if(Type is ClassType)
            {
                List<string> entities = new List<string>();
                foreach (IEntity entity in Model.Entities)
                {
                    entities.Add(entity.Name);
                }

                ClassType _class = (ClassType)Type;

                List<string> properties = new List<string>();
                foreach (Operation operation in _class.Operations)
                {
                    if (operation is Property)
                        properties.Add(operation.Name);
                }

                List<Property> compositeId = new List<Property>();

                if (entities.Contains(_class.Operations.ToList()[0].Type))
                {
                    for (int index = 0; index <= (_class.Operations.Count() - 1); index++)
                    {
                        if (_class.Operations.ToList()[index] is Property)
                        {
                            Property property = (Property)_class.Operations.ToList()[index];

                            if (entities.Contains(property.Type))
                            {
                                compositeId.Add(property);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                if (compositeId.Count > 1)
                {
                    WriteEquals(compositeId);
                    WriteGetHashCode(compositeId);
                }
            }
        }

        private void WriteEquals(List<Property> compositeId)
        {
            AddBlankLine();
            WriteLine("// Needs this for composite id.");
            WriteLine("public override bool Equals(object obj)");
            WriteLine("{");
            IndentLevel++;
            WriteLine("if (obj == null) return false;");
            WriteLine(string.Format("var t = obj as {0};", Type.Name));
            WriteLine("if (t == null) return false;");
            Write("return ");
            foreach(var id in compositeId)
            {
                Write(string.Format("{0} == t.{0}", id.Name), false);
                if(id != compositeId.Last())
                    Write(" && ", false);
                else
                    Write(";", false);
            }
            WriteLine("", false);
            IndentLevel--;
            WriteLine("}");
        }

        private void WriteGetHashCode(List<Property> compositeId)
        {
            AddBlankLine();
            WriteLine("// Needs this for composite id.");
            WriteLine("public override int GetHashCode()");
            WriteLine("{");
            IndentLevel++;
            Write("return (");
            foreach (var id in compositeId)
            {
                Write(id.Name, false);
                if (id != compositeId.Last())
                    Write(" + \"|\" + ", false);
                else
                    Write(").GetHashCode();", false);
            }
            WriteLine("", false);
            IndentLevel--;
            WriteLine("}");
        }

		private void WriteUsings()
		{
			StringCollection importList = Settings.Default.CSharpImportList;
			foreach (string usingElement in importList)
				WriteLine("using " + usingElement + ";");

			if (importList.Count > 0)
				AddBlankLine();
		}

		private void OpenNamespace()
		{
			WriteLine("namespace " + RootNamespace);
			WriteLine("{");
			IndentLevel++;
		}

		private void CloseNamespace()
		{
			IndentLevel--;
			WriteLine("}");
		}

		private void WriteType(TypeBase type)
		{
            WriteXmlComments(type);

			if (type is CompositeType)
				WriteCompositeType((CompositeType) type);
			else if (type is EnumType)
				WriteEnum((EnumType) type);
			else if (type is DelegateType)
				WriteDelegate((DelegateType) type);
		}

		private void WriteCompositeType(CompositeType type)
		{
			// Writing type declaration
			WriteLine(type.GetDeclaration());
			WriteLine("{");
			IndentLevel++;

			if (type is ClassType)
			{
				foreach (TypeBase nestedType in ((ClassType) type).NestedChilds)
				{
					WriteType(nestedType);
					AddBlankLine();
				}
			}

			if (type.SupportsFields)
			{
				foreach (Field field in type.Fields)
					WriteField(field);
			}

			bool needBlankLine = (type.FieldCount > 0 && type.OperationCount > 0);

			foreach (Operation operation in type.Operations)
			{
				if (needBlankLine)
					AddBlankLine();
				needBlankLine = true;

				WriteOperation(operation);
			}

            if(Settings.Default.GenerateNHibernateMapping)
                WriteCompositeId();

			// Writing closing bracket of the type block
			IndentLevel--;
			WriteLine("}");
		}

		private void WriteEnum(EnumType _enum)
		{
			// Writing type declaration
			WriteLine(_enum.GetDeclaration());
			WriteLine("{");
			IndentLevel++;

			int valuesRemained = _enum.ValueCount;
			foreach (EnumValue value in _enum.Values)
			{
                WriteXmlComments(value);

				if (--valuesRemained > 0)
					WriteLine(value.GetDeclaration() + ",");
				else
					WriteLine(value.GetDeclaration());
			}

			// Writing closing bracket of the type block
			IndentLevel--;
			WriteLine("}");
		}

		private void WriteDelegate(DelegateType _delegate)
		{
			WriteLine(_delegate.GetDeclaration());
		}

		private void WriteField(Field field)
		{
            WriteXmlComments(field);

			WriteLine(field.GetDeclaration());
		}

		private void WriteOperation(Operation operation)
		{
            WriteXmlComments(operation);

            if (operation is Property)
            {
                if (Settings.Default.UseAutomaticProperties)
                {
                    WriteLine(string.Format("{0} {{ get; set; }}", operation.GetDeclaration()));
                }
                else
                {
                    WriteLine(operation.GetDeclaration());
                    WriteProperty((Property)operation);
                }
            }
            else
            {
                WriteLine(operation.GetDeclaration());

                if (operation.HasBody)
                {
                    if (operation is Event)
                    {
                        WriteLine("{");
                        IndentLevel++;
                        WriteLine("add {  }");
                        WriteLine("remove {  }");
                        IndentLevel--;
                        WriteLine("}");
                    }
                    else
                    {
                        WriteLine("{");
                        IndentLevel++;
                        WriteNotImplementedString();
                        IndentLevel--;
                        WriteLine("}");
                    }
                }
            }
		}

		private void WriteProperty(Property property)
		{
			WriteLine("{");
			IndentLevel++;

			if (!property.IsWriteonly)
			{
				if (property.HasImplementation)
				{
					WriteLine("get");
					WriteLine("{");
					IndentLevel++;
					WriteNotImplementedString();
					IndentLevel--;
					WriteLine("}");
				}
				else
				{
					WriteLine("get;");
				}
			}
			if (!property.IsReadonly)
			{
				if (property.HasImplementation)
				{
					WriteLine("set");
					WriteLine("{");
					IndentLevel++;
					WriteNotImplementedString();
					IndentLevel--;
					WriteLine("}");
				}
				else
				{
					WriteLine("set;");
				}
			}

			IndentLevel--;
			WriteLine("}");
		}

		private void WriteNotImplementedString()
		{
			if (Settings.Default.UseNotImplementedExceptions)
			{
				if (Settings.Default.CSharpImportList.Contains("System"))
					WriteLine("throw new NotImplementedException();");
				else
					WriteLine("throw new System.NotImplementedException();");
			}
			else
			{
				AddBlankLine(true);
			}
		}
	}
}