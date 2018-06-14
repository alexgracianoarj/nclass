using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace NClass.CodeGenerator
{
    public class TemplateSettings
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool PerEntity { get; set; }
        public string FileName { get; set; }
        public string Code { get; set; }
        public string Language { get; set; }
    }

    public class TemplatesSettings
    {
        public List<TemplateSettings> Templates { get; set; }

        public TemplatesSettings()
        {
            Templates = new List<TemplateSettings>();
        }

        public void Save()
        {
            var streamWriter = new StreamWriter(Application.StartupPath + @"\Templates\templates.xml", false, Encoding.Unicode);
            using (streamWriter)
            {
                var xmlSerializer = new XmlSerializer(typeof(TemplatesSettings));
                xmlSerializer.Serialize(streamWriter, this);
            }
        }

        public static TemplatesSettings Load()
        {
            TemplatesSettings templates = new TemplatesSettings();

            var xmlSerializer = new XmlSerializer(typeof(TemplatesSettings));
            var fi = new FileInfo(Application.StartupPath + @"\Templates\templates.xml");
            if (fi.Exists)
            {
                using (FileStream fileStream = fi.OpenRead())
                {
                    templates = (TemplatesSettings)xmlSerializer.Deserialize(fileStream);
                }
            }

            return templates;
        }

        public int SaveOrUpdateTemplate(TemplateSettings template)
        {
            var tmplt = Templates.SingleOrDefault(x => x.Id.Equals(template.Id));

            if (tmplt == null)
            {
                Templates.Add(template);
            }
            else
            {
                tmplt.Name = template.Name;
                tmplt.Enabled = template.Enabled;
                tmplt.PerEntity = template.PerEntity;
                tmplt.FileName = template.FileName;
                tmplt.Code = template.Code;
                tmplt.Language = template.Language;
            }

            Save();

            return Templates.FindIndex(x => x.Id.Equals(template.Id));
        }

        public void DeleteTemplate(TemplateSettings template)
        {
            var tmplt = Templates.SingleOrDefault(x => x.Id.Equals(template.Id));

            if (tmplt != null)
            {
                Templates.Remove(tmplt);
            }

            Save();
        }
    }
}
