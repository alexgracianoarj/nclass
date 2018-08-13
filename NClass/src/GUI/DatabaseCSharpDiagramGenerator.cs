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
        private IList<DatabaseView> views;
        private Project project;
        private Diagram diagram;
        private ITextFormatter textFormatter;

        public virtual bool ConvertToPascalCase { get; set; }

        public IList<DatabaseTable> Tables
        {
            get
            {
                return tables;
            }

            set
            {
                tables = value;
            }
        }

        public IList<DatabaseView> Views
        {
            get
            {
                return views;
            }

            set
            {
                views = value;
            }
        }

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

            tables = metadataReader.AllTables();
            views = metadataReader.AllViews();
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
            
            if(CodeGenerator.Settings.Default.GenerateNHibernateMapping)
            {
                if (CodeGenerator.Settings.Default.UseUnderscoreAndLowercaseInDB)
                    classType.NHMTableName = new LowercaseAndUnderscoreTextFormatter().FormatText(table.Name);
                else
                    classType.NHMTableName = table.Name;

                classType.IdGenerator = Enum.GetName(typeof(CodeGenerator.IdGeneratorType), CodeGenerator.Settings.Default.DefaultIdGenerator);
            }

            foreach (var column in table.Columns)
            {
                Property property = classType.AddProperty();
                property.InitFromString(CreateProperty(column, classType));

                if (CodeGenerator.Settings.Default.GenerateNHibernateMapping)
                {
                    if(CodeGenerator.Settings.Default.UseUnderscoreAndLowercaseInDB)
                        property.NHMColumnName = new LowercaseAndUnderscoreTextFormatter().FormatText(column.Name);
                    else
                        property.NHMColumnName = column.Name;

                    property.IsPrimaryKey = column.IsPrimaryKey;
                    property.IsUnique = column.IsUniqueKey;
                    property.IsNotNull = !column.Nullable;
                }
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

            if (column.IsForeignKey
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
            if (ConvertToPascalCase)
                textFormatter = new PascalCaseTextFormatter();
            else
                textFormatter = new UnformattedTextFormatter();

            textFormatter.PrefixRemoval = connSettings.PrefixRemoval;

            project = new Project(Strings.Untitled);

            diagram = new Diagram(Strings.Untitled, CSharpLanguage.Instance);

            foreach (var table in tables)
            {
                CreateClass(table);
            }

            foreach (var view in views)
            {
                CreateClass(view);
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
