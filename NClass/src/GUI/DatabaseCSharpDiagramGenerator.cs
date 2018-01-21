using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using NClass.Core;
using NClass.CSharp;
using NClass.Translations;
using NClass.DiagramEditor.ClassDiagram;
using NClass.DiagramEditor.ClassDiagram.Shapes;
using NClass.DiagramEditor.ClassDiagram.Connections;

using NClass.CodeGenerator;

using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;

namespace NClass.GUI
{
    public class DatabaseCSharpDiagramGenerator
    {
        private ConnectionSettings connSettings;
        private DatabaseReader metadataReader;
        private IList<DatabaseTable> tables;
        private Project project;
        private Diagram diagram;
        private ITextFormatter textFormatter;

        public Project ProjectGenerated
        {
            get
            {
                return project;
            }
        }

        public DatabaseCSharpDiagramGenerator
            (ConnectionSettings connectionSettings)
        {
            this.connSettings = connectionSettings;

            metadataReader = new DatabaseReader(connSettings.ConnectionString, connSettings.ServerType);
            metadataReader.Owner = connSettings.Schema;
            textFormatter = new  PascalCaseTextFormatter();
            textFormatter.PrefixRemoval = connSettings.PrefixRemoval;
        }

        private void CreateAssociation(ClassType first, ClassType second)
        {
            AssociationRelationship associationRelationship = diagram.AddAssociation(first, second);
            associationRelationship.StartMultiplicity = "0..1";
            associationRelationship.EndMultiplicity = "*";
        }

        private ClassType CreateClass(DatabaseTable table)
        {
            var name = textFormatter.FormatText(table.Name);

            ClassType classType = ClassAlreadyExists(name);

            if (classType != null)
                return classType;

            classType = diagram.AddClass();
            classType.AccessModifier = AccessModifier.Public;
            classType.Modifier = ClassModifier.None;
            classType.Name = name;

            foreach (var column in table.Columns)
            {
                classType.AddProperty().InitFromString(CreateProperty(column, classType));
            }

            return classType;
        }

        private ClassType ClassAlreadyExists(string className)
        {
            return diagram.Entities.SingleOrDefault(x => x.Name == className) as ClassType;
        }

        private DatabaseTable GetTableByName(string tableName)
        {
            return tables.SingleOrDefault(x => x.Name == tableName);
        }

        private string CreateProperty(DatabaseColumn column, ClassType classType)
        {
            string property = null;
            
            if(column.IsForeignKey
                && !string.IsNullOrEmpty(column.ForeignKeyTableName))
            {
                property = string.Format(
                    "public virtual {0} {1} {{ get; set; }}",
                    textFormatter.FormatText(column.ForeignKeyTableName),
                    textFormatter.FormatText(column.Name));

                CreateAssociation(CreateClass(GetTableByName(column.ForeignKeyTableName)), classType);
            }
            else
            {
                property = string.Format(
                    "public virtual {0} {1} {{ get; set; }}",
                    column.DataType.NetDataTypeCSharpName,
                    textFormatter.FormatText(column.Name));
            }
            
            return property;
        }

        public void Generate()
        {
            project = new Project(Strings.Untitled);

            diagram = new Diagram(Strings.Untitled, CSharpLanguage.Instance);

            tables = metadataReader.AllTables();

            foreach (var table in tables)
            {
                CreateClass(table);
            }

            ArrangeTypes();

            project.Add(diagram);

            metadataReader.Dispose();
        }

        /// <summary>
        /// Creates a nice arrangement for each entity.
        /// </summary>
        private void ArrangeTypes()
        {
            const int Margin = Connection.Spacing * 2;
            const int DiagramPadding = Shape.SelectionMargin;

            int shapeCount = diagram.ShapeCount;
            int columns = (int)Math.Ceiling(Math.Sqrt(shapeCount * 2));
            int shapeIndex = 0;
            int top = Shape.SelectionMargin;
            int maxHeight = 0;

            foreach (Shape shape in diagram.Shapes)
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
    }
}
