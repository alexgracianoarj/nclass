using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace NClass.CodeGenerator
{
    [Serializable]
    public class GroupTemplates
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<TemplateSettings> Templates { get; set; }

        public GroupTemplates()
        {
            Templates = new List<TemplateSettings>();
        }
    }

    [Serializable]
    public class TemplateSettings
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool PerEntity { get; set; }
        public string FileExt { get; set; }
        public string Language { get; set; }
        public string Code { get; set; }
    }

    [Serializable]
    public class TemplatesSettings
    {
        [NonSerialized]
        private static string pathFile = Application.StartupPath + @"\Templates\templates.bin";
        public List<GroupTemplates> Groups { get; set; }

        public TemplatesSettings()
        {
            Groups = new List<GroupTemplates>();
        }

        public void Save()
        {
            using (var fileStream = new FileStream(pathFile, FileMode.Create))
            {
                new BinaryFormatter().Serialize(fileStream, this);
            }
        }

        public static TemplatesSettings Load()
        {
            TemplatesSettings templates = new TemplatesSettings();

            if (File.Exists(pathFile))
            {
                using (var fileStream = new FileStream(pathFile, FileMode.Open))
                {
                    templates = (TemplatesSettings)new BinaryFormatter().Deserialize(fileStream);
                }
            }

            return templates;
        }

        public int SaveGroup(GroupTemplates groupTemplates)
        {
            var group = Groups.SingleOrDefault(x => x.Id.Equals(groupTemplates.Id));

            if (group == null)
            {
                Groups.Add(groupTemplates);
            }
            else
            {
                group.Name = groupTemplates.Name;
                group.Enabled = groupTemplates.Enabled;
            }

            Save();

            return Groups.FindIndex(x => x.Id.Equals(groupTemplates.Id));
        }

        public int SaveTemplate(GroupTemplates groupTemplates, TemplateSettings template)
        {
            var tmplt = groupTemplates.Templates.SingleOrDefault(x => x.Id.Equals(template.Id));

            if (tmplt == null)
            {
                groupTemplates.Templates.Add(template);
            }
            else
            {
                tmplt.Name = template.Name;
                tmplt.Enabled = template.Enabled;
                tmplt.PerEntity = template.PerEntity;
                tmplt.FileExt = template.FileExt;
                tmplt.Code = template.Code;
                tmplt.Language = template.Language;
            }

            Save();

            return groupTemplates.Templates.FindIndex(x => x.Id.Equals(template.Id));
        }

        public void DeleteGroup(GroupTemplates groupTemplates)
        {
            var group = Groups.SingleOrDefault(x => x.Id.Equals(groupTemplates.Id));

            if (group != null)
            {
                Groups.Remove(group);
            }

            Save();
        }

        public void DeleteTemplate(GroupTemplates groupTemplates, TemplateSettings template)
        {
            var group = Groups.SingleOrDefault(x => x.Id.Equals(groupTemplates.Id));
            var tmplt = group.Templates.SingleOrDefault(x => x.Id.Equals(template.Id));

            if (tmplt != null)
            {
                group.Templates.Remove(tmplt);
            }

            Save();
        }
    }
}
