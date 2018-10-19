using System;
using System.Collections.Generic;
using NClass.Core;

using Liquid.NET;
using Liquid.NET.Utils;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

namespace NClass.CodeGenerator
{
    internal sealed class CSharpTemplateSourceFileGenerator 
        : SourceFileGenerator
    {
        public CSharpTemplateSourceFileGenerator
            (TypeBase type, string rootNamespace, Model model)
            : base(type, rootNamespace, model)
        {}

        public CSharpTemplateSourceFileGenerator
            (string rootNamespace, Model model)
            : base(rootNamespace, model)
        { }

        const int DefaultBuilderCapacity = 10240; // 10 KB

        protected override string Extension
        {
            get { return ""; }
        }

        /// <exception cref="FileGenerationException">
        /// An error has occured while generating the source file.
        /// </exception>
        public List<string> GenerateFiles(string directory, bool perEntity)
        {
            List<string> files = new List<string>();

            var tmpltsSettings = TemplatesSettings.Load();

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            foreach (var group in tmpltsSettings.Groups.Where(x => x.Enabled))
            {
                foreach (var tmpltSettings in group.Templates.Where(x => x.Enabled && x.PerEntity == perEntity))
                {
                    try
                    {
                        string fileName = null;

                        if (tmpltSettings.PerEntity)
                        {
                            var entityMeta = GenerateEntityMeta();
                            fileName = TemplateRender("entity", entityMeta, tmpltSettings.FileExt);
                        }
                        else
                        {
                            var modelMeta = GenerateModelMeta();                            
                            fileName = TemplateRender("model", modelMeta, tmpltSettings.FileExt);
                        }

                        fileName = Regex.Replace(fileName, @"\<(?<type>.+)\>", @"[${type}]");
                        string path = Path.Combine(directory, fileName);

                        if (WriteFileContent(tmpltSettings))
                        {
                            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Unicode))
                            {
                                writer.Write(CodeBuilder.ToString());
                            }

                            files.Add(fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new FileGenerationException(directory, ex);
                    }
                }
            }

            return files;
        }

        /// <exception cref="IOException">
        /// An I/O error occurs.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The <see cref="TextWriter"/> is closed.
        /// </exception>
        private bool WriteFileContent(TemplateSettings tmpltSettings)
        {
            if (CodeBuilder == null)
                CodeBuilder = new StringBuilder(DefaultBuilderCapacity);
            else
                CodeBuilder.Length = 0;

            if (tmpltSettings.PerEntity)
            {
                var entityMeta = GenerateEntityMeta();
                string code = TemplateRender("entity", entityMeta, tmpltSettings.Code);
                CodeBuilder.Append(code);
            }
            else
            {
                var modelMeta = GenerateModelMeta();
                string code = TemplateRender("model", modelMeta, tmpltSettings.Code);
                CodeBuilder.Append(code);
            }

            if (CodeBuilder.Length == 0)
                return false;

            return true;
        }

        protected override void WriteFileContent()
        {
        }

        private string TemplateRender(string context, object constant, string template)
        {
            string result = "";
            string liquidErrors = "";

            ITemplateContext ctx = new TemplateContext()
                .WithAllFilters()
                .DefineLocalVariable(context, constant.ToLiquid());

            var parsingResult = LiquidTemplate.Create(template);

            if (parsingResult.ParsingErrors.Count() > 0)
            {
                liquidErrors += string.Join(", ", parsingResult.ParsingErrors.Select(x => x.ToString()));
                result += liquidErrors;
            }

            var renderingResult = parsingResult.LiquidTemplate.Render(ctx);

            if (renderingResult.RenderingErrors.Count() > 0)
            {
                liquidErrors = string.Join(", ", renderingResult.RenderingErrors.Select(x => x.ToString()));
                result += liquidErrors;
            }

            result += renderingResult.Result;

            return result;
        }

        private ModelMeta GenerateModelMeta()
        {
            var modelMeta = new ModelMeta();
            modelMeta.ProjectName = Model.Project.Name;
            modelMeta.Name = Model.Name;

            foreach (var entity in Model.Entities)
            {
                if (entity is EnumType)
                {
                    var entityType = entity as EnumType;
                    var entityMeta = new EnumMeta();

                    entityMeta.EntityType = Enum.GetName(typeof(EntityType), entity.EntityType);
                    entityMeta.Access = Enum.GetName(typeof(AccessModifier), entityType.Access);
                    entityMeta.Name = entityType.Name;
                    entityMeta.ValuesCount = entityType.ValueCount;

                    foreach (var value in entityType.Values)
                    {
                        entityMeta.Values.Add(Enum.GetName(typeof(EnumValue), value));
                    }

                    modelMeta.Entities.Add(entityMeta);
                }
                else
                {
                    var entityType = entity as CompositeType;
                    var entityMeta = new CompositeMeta();

                    entityMeta.EntityType = Enum.GetName(typeof(EntityType), entity.EntityType);
                    entityMeta.Access = Enum.GetName(typeof(AccessModifier), entityType.Access);
                    entityMeta.Name = entityType.Name;
                    entityMeta.NHMTableName = entityType.NHMTableName;
                    entityMeta.IdentityGenerator = EnumExtensions.GetDescription((IdentityGeneratorType)Enum.Parse(typeof(IdentityGeneratorType), entityType.IdGenerator));

                    if (entityType.SupportsFields)
                    {
                        entityMeta.FieldsCount = entityType.FieldCount;

                        foreach (var field in entityType.Fields)
                        {
                            var fieldMeta = new FieldMeta();
                            fieldMeta.MemberType = Enum.GetName(typeof(MemberType), field.MemberType);
                            fieldMeta.Access = Enum.GetName(typeof(AccessModifier), field.Access);
                            fieldMeta.Name = field.Name;
                            fieldMeta.Type = field.Type;

                            entityMeta.Fields.Add(fieldMeta);
                        }
                    }

                    if (entityType.SupportsOperations)
                    {
                        entityMeta.OperationsCount = entityType.OperationCount;

                        foreach (var operation in entityType.Operations)
                        {
                            var operationMeta = new OperationMeta();
                            operationMeta.MemberType = Enum.GetName(typeof(MemberType), operation.MemberType);
                            operationMeta.Access = Enum.GetName(typeof(AccessModifier), operation.Access);
                            operationMeta.Name = operation.Name;
                            operationMeta.Type = operation.Type;
                            operationMeta.NHMColumnName = operation.NHMColumnName;
                            operationMeta.Identity = operation.IsIdentity;
                            operationMeta.ManyToOne = operation.ManyToOne;
                            operationMeta.Unique = operation.IsUnique;
                            operationMeta.NotNull = operation.IsNotNull;

                            entityMeta.Operations.Add(operationMeta);
                        }
                    }

                    modelMeta.Entities.Add(entityMeta);
                }
            }

            return modelMeta;
        }

        private EntityMeta GenerateEntityMeta()
        {
            if (Type is EnumType)
            {
                var entityType = Type as EnumType;
                var entityMeta = new EnumMeta();

                entityMeta.ProjectName = Model.Project.Name;
                entityMeta.ModelName = Model.Name;

                entityMeta.EntityType = Enum.GetName(typeof(EntityType), entityType.EntityType);
                entityMeta.Access = Enum.GetName(typeof(AccessModifier), entityType.Access);
                entityMeta.Name = entityType.Name;
                entityMeta.ValuesCount = entityType.ValueCount;

                foreach (var value in entityType.Values)
                {
                    entityMeta.Values.Add(Enum.GetName(typeof(EnumValue), value));
                }

                return entityMeta;
            }
            else
            {
                var entityType = Type as CompositeType;
                var entityMeta = new CompositeMeta();

                entityMeta.ProjectName = Model.Project.Name;
                entityMeta.ModelName = Model.Name;

                entityMeta.EntityType = Enum.GetName(typeof(EntityType), entityType.EntityType);
                entityMeta.Access = Enum.GetName(typeof(AccessModifier), entityType.Access);
                entityMeta.Name = entityType.Name;
                entityMeta.NHMTableName = entityType.NHMTableName;
                entityMeta.IdentityGenerator = EnumExtensions.GetDescription((IdentityGeneratorType)Enum.Parse(typeof(IdentityGeneratorType), entityType.IdGenerator));

                if (entityType.SupportsFields)
                {
                    entityMeta.FieldsCount = entityType.FieldCount;

                    foreach (var field in entityType.Fields)
                    {
                        var fieldMeta = new FieldMeta();
                        fieldMeta.MemberType = Enum.GetName(typeof(MemberType), field.MemberType);
                        fieldMeta.Access = Enum.GetName(typeof(AccessModifier), field.Access);
                        fieldMeta.Name = field.Name;
                        fieldMeta.Type = field.Type;

                        entityMeta.Fields.Add(fieldMeta);
                    }
                }

                if (entityType.SupportsOperations)
                {
                    entityMeta.OperationsCount = entityType.OperationCount;

                    foreach (var operation in entityType.Operations)
                    {
                        var operationMeta = new OperationMeta();
                        operationMeta.MemberType = Enum.GetName(typeof(MemberType), operation.MemberType);
                        operationMeta.Access = Enum.GetName(typeof(AccessModifier), operation.Access);
                        operationMeta.Name = operation.Name;
                        operationMeta.Type = operation.Type;
                        operationMeta.NHMColumnName = operation.NHMColumnName;
                        operationMeta.Identity = operation.IsIdentity;
                        operationMeta.ManyToOne = operation.ManyToOne;
                        operationMeta.Unique = operation.IsUnique;
                        operationMeta.NotNull = operation.IsNotNull;

                        entityMeta.Operations.Add(operationMeta);
                    }
                }

                return entityMeta;
            }
        }
    }

    public class ModelMeta
    {
        public string ProjectName { get; set; }
        public string Name { get; set; }
        public string AssemblyName
        {
            get
            {
                if (string.Equals(ProjectName, Name, StringComparison.OrdinalIgnoreCase))
                    return ProjectName;
                else
                    return ProjectName + "." + Name;
            }
        }
        public string RootNamespace
        {
            get
            {
                if (string.Equals(ProjectName, Name, StringComparison.OrdinalIgnoreCase))
                    return Name;
                else
                    return ProjectName + "." + Name;
            }
        }
        public List<EntityMeta> Entities { get; set; }

        public ModelMeta()
        {
            Entities = new List<EntityMeta>();
        }
    }

    public class EntityMeta
    {
        public string ProjectName { get; set; }
        public string ModelName { get; set; }
        public string AssemblyName
        {
            get
            {
                if (string.Equals(ProjectName, ModelName, StringComparison.OrdinalIgnoreCase))
                    return ProjectName;
                else
                    return ProjectName + "." + ModelName;
            }
        }
        public string RootNamespace
        {
            get
            {
                if (string.Equals(ProjectName, ModelName, StringComparison.OrdinalIgnoreCase))
                    return ModelName;
                else
                    return ProjectName + "." + ModelName;
            }
        }
        public string EntityType { get; set; }
        public string Access { get; set; }
        public string Name { get; set; }

        public EntityMeta()
        {
        }
    }

    public class CompositeMeta : EntityMeta
    {
        public string NHMTableName { get; set; }
        public string IdentityGenerator { get; set; }
        public int FieldsCount { get; set; }
        public List<FieldMeta> Fields { get; set; }
        public int OperationsCount { get; set; }
        public List<OperationMeta> Operations { get; set; }

        public CompositeMeta()
        {
            Fields = new List<FieldMeta>();
            Operations = new List<OperationMeta>();
        }
    }

    public class EnumMeta : EntityMeta
    {
        public int ValuesCount { get; set; }
        public List<string> Values { get; set; }

        public EnumMeta()
        {
            Values = new List<string>();
        }
    }

    public class MemberMeta
    {
        public string MemberType { get; set; }
        public string Access { get; set; }
        public string Name { get; set; }
    }

    public class OperationMeta : MemberMeta
    {
        public string Type { get; set; }
        public string NHMColumnName { get; set; }
        public bool Identity { get; set; }
        public string ManyToOne { get; set; }
        public bool Unique { get; set; }
        public bool NotNull { get; set; }

        public OperationMeta()
        {
        }
    }

    public class FieldMeta : MemberMeta
    {
        public string Type { get; set; }
        
        public FieldMeta()
        {
        }
    }
}
