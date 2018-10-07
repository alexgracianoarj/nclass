using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NClass.Core;
using NClass.Translations;

using System.Xml;

namespace NClass.DiagramEditor.ClassDiagram.Editors
{
	public partial class ClassTypeEditor : CompositeTypeEditor
	{
		private bool isAdvancedOptionsExpanded = false;

		/// <summary>
		/// Creates an instance of the class type editor.
		/// </summary>
		public ClassTypeEditor()
			: base()
		{
			InitializeComponent();

			toolStripAdvancedOptions.Renderer = ToolStripSimplifiedRenderer.Default;

            cboIdentityGenerator.SelectedIndexChanged -= cboIdGenerator_SelectedIndexChanged;

            cboIdentityGenerator.DataSource = Enum.GetNames(typeof(CodeGenerator.IdentityGeneratorType));

            cboIdentityGenerator.SelectedIndexChanged += cboIdGenerator_SelectedIndexChanged;

			// Set this in the constructor instead of the designer so that the designer doesn't give an error about
			// using the parent's TextChanged method.
            this.txtStereotype.TextChanged += new EventHandler(base.textBox_TextChanged);
            this.txtNHMTableName.TextChanged += new EventHandler(base.textBox_TextChanged);
		}

		/// <summary>
		/// Sets up the editor.
		/// </summary>
		/// <param name="element">The diagram element which is being edited through this editor.</param>
		internal override void Init(DiagramElement element)
		{
			base.Init(element);

			CompositeType type = Shape.CompositeType;

			// If the user has entered a stereotype for this class, then expand the form to show the stereotype textbox.
			if (type.Stereotype != null || !string.IsNullOrEmpty(type.NHMTableName))
			{
				ExpandAdvancedOptions();
                RefreshIdentityGeneratorParameters();
			}
			else
			{
				CollapseAdvancedOptions();
			}

            Refresh();
		}

		/// <summary>
		/// Updates labels and other controls so that they contain localized text.
		/// </summary>
		protected override void UpdateTexts()
		{
			base.UpdateTexts();

			lblStereotype.Text = Strings.Stereotype;
		}

		/// <summary>
		/// Updates textboxes and other controls so that they contain the latest values from the class currently being edited.
		/// </summary>
		protected override void RefreshValues()
		{
			base.RefreshValues();

			SuspendLayout();

			CompositeType type = Shape.CompositeType;

            int cursorPositionStereotype = txtStereotype.SelectionStart;
            int cursorPositionNHMTable = txtNHMTableName.SelectionStart;

            txtStereotype.Text = type.Stereotype;
            txtNHMTableName.Text = type.NHMTableName;
            cboIdentityGenerator.SelectedItem = type.IdGenerator;

			// Remove the angled characters when the stereotype is displayed in the editor.
			if (!string.IsNullOrEmpty(txtStereotype.Text))
			{
				txtStereotype.Text = txtStereotype.Text.Replace("«", "").Replace("»", "");
			}

            if(CodeGenerator.Settings.Default.GenerateNHibernateMapping
                && !string.IsNullOrEmpty(Shape.CompositeType.NHMTableName)
                && !isAdvancedOptionsExpanded)
            {
                ExpandAdvancedOptions();
                Refresh();
            }

            RefreshIdentityGeneratorParameters();
            Refresh();

            txtStereotype.SelectionStart = cursorPositionStereotype;
            txtNHMTableName.SelectionStart = cursorPositionNHMTable;

			ResumeLayout();
		}

		/// <summary>
		/// Validates all of the data that can be edited through this editor.
		/// </summary>
		public override void ValidateData()
		{
			base.ValidateData();

			ValidateStereotype();
            ValidateNHMTableName();

			SetError(null);
		}

		/// <summary>
		/// Validates the stereotype that has been entered into the stereotype textbox.
		/// </summary>
		/// <returns>True if the stereotype is valid, false if it is not.</returns>
		private bool ValidateStereotype()
		{
			if (NeedValidation)
			{
				try
				{
					// There's no stereotype given by the user, so remove the stereotype.
					if (string.IsNullOrEmpty(txtStereotype.Text))
					{
						Shape.CompositeType.Stereotype = null;
					}
					else
					{
						Shape.CompositeType.Stereotype = "«" + txtStereotype.Text + "»";
					}
					RefreshValues();
				}
				catch (BadSyntaxException ex)
				{
					SetError(ex.Message);
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Handles the keydown event for the stereotype textbox.
		/// </summary>
		/// <param name="sender">The object that raised the event.</param>
		/// <param name="e">Any extra event data.</param>
		private void txtStereotype_KeyDown(object sender, KeyEventArgs e)
		{
			HandleCompositeTypeTextBoxKeyDown(ValidateStereotype, e.KeyCode, e.Modifiers);
		}

		/// <summary>
		/// Validates the stereotype that has been entered into the stereotype textbox.
		/// </summary>
		/// <param name="sender">The object that raised the event.</param>
		/// <param name="e">Any extra event data.</param>
		private void txtStereotype_Validating(object sender, CancelEventArgs e)
		{
			ValidateStereotype();
		}

		/// <summary>
		/// Shows the advanced options section, and changes the advanced options button image appropriately.
		/// </summary>
		private void ExpandAdvancedOptions()
		{
            this.Height = pnlAdvancedOptions.Bottom + pnlAdvancedOptions.Margin.Bottom;

			toolAdvancedOptions.Image = Properties.Resources.CollapseSingle;
			
            isAdvancedOptionsExpanded = true;
		}

		/// <summary>
		/// Hides the advanced options section, and changes the advanced options button image appropriately.
		/// </summary>
		private void CollapseAdvancedOptions()
		{
			this.Height = pnlAdvancedOptions.Location.Y;

			toolAdvancedOptions.Image = Properties.Resources.ExpandSingle;

			isAdvancedOptionsExpanded = false;
		}

		/// <summary>
		/// Expands or collapses the advanced options for this editor.
		/// </summary>
		/// <param name="sender">The object that raised the event.</param>
		/// <param name="e">Any extra event data.</param>
		private void toolAdvancedOptions_Click(object sender, EventArgs e)
		{
			if (isAdvancedOptionsExpanded)
			{
				CollapseAdvancedOptions();
			}
			else
			{
				ExpandAdvancedOptions();
                RefreshIdentityGeneratorParameters();
			}

			Refresh();
		}

        private void borderedTextBox1_Validating(object sender, CancelEventArgs e)
        {
            ValidateNHMTableName();
        }

        /// <summary>
        /// Validates the NHMTableName that has been entered into the NHMTableName textbox.
        /// </summary>
        /// <returns>True if the NHMTableName is valid, false if it is not.</returns>
        private bool ValidateNHMTableName()
        {
            if (NeedValidation)
            {
                try
                {
                    if (string.IsNullOrEmpty(txtNHMTableName.Text))
                    {
                        Shape.CompositeType.NHMTableName = null;
                    }
                    else
                    {
                        if ((new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9_ ]").IsMatch(txtNHMTableName.Text)))
                            throw new BadSyntaxException("Invalid NHM Table Name");

                        Shape.CompositeType.NHMTableName = txtNHMTableName.Text;
                    }
                    RefreshValues();
                }
                catch (BadSyntaxException ex)
                {
                    SetError(ex.Message);
                    return false;
                }
            }
            return true;
        }

        private void borderedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            HandleCompositeTypeTextBoxKeyDown(ValidateNHMTableName, e.KeyCode, e.Modifiers);
        }

        private void cboIdGenerator_SelectedIndexChanged(object sender, EventArgs e)
        {
            Shape.CompositeType.IdGenerator = (string)cboIdentityGenerator.SelectedItem;
            RefreshValues();
            txtName.Focus();
        }

        private void RefreshIdentityGeneratorParameters()
        {
            XmlDocument xmlGeneratorParameters = new XmlDocument();
            
            if (!string.IsNullOrEmpty(Shape.CompositeType.GeneratorParameters))
                xmlGeneratorParameters.LoadXml(Shape.CompositeType.GeneratorParameters);

            if (Shape.CompositeType.IdGenerator == "HiLo")
            {
                this.Height = pnlGeneratorParameters.Bottom + pnlGeneratorParameters.Margin.Bottom;
                
                CodeGenerator.HiLoIdentityGeneratorParameters hiLo = null;

                if(xmlGeneratorParameters.SelectSingleNode("//HiLoIdentityGeneratorParameters") != null)
                    hiLo = CodeGenerator.GeneratorParametersDeSerializer.Deserialize<CodeGenerator.HiLoIdentityGeneratorParameters>(Shape.CompositeType.GeneratorParameters);
                else
                {
                    Shape.CompositeType.GeneratorParameters = null;
                    hiLo = new CodeGenerator.HiLoIdentityGeneratorParameters();
                }

                prgGeneratorParameters.SelectedObject = hiLo;
            }
            else if (Shape.CompositeType.IdGenerator == "SeqHiLo")
            {
                this.Height = pnlGeneratorParameters.Bottom + pnlGeneratorParameters.Margin.Bottom;

                CodeGenerator.SeqHiLoIdentityGeneratorParameters seqHiLo = null;

                if (xmlGeneratorParameters.SelectSingleNode("//SeqHiLoIdentityGeneratorParameters") != null)
                    seqHiLo = CodeGenerator.GeneratorParametersDeSerializer.Deserialize<CodeGenerator.SeqHiLoIdentityGeneratorParameters>(Shape.CompositeType.GeneratorParameters);
                else
                {
                    Shape.CompositeType.GeneratorParameters = null;
                    seqHiLo = new CodeGenerator.SeqHiLoIdentityGeneratorParameters();
                }

                prgGeneratorParameters.SelectedObject = seqHiLo;
            }
            else if (Shape.CompositeType.IdGenerator == "Sequence")
            {
                this.Height = pnlGeneratorParameters.Bottom + pnlGeneratorParameters.Margin.Bottom;
                
                CodeGenerator.SequenceIdentityGeneratorParameters sequence = null;

                if (xmlGeneratorParameters.SelectSingleNode("//SequenceIdentityGeneratorParameters") != null)
                    sequence = CodeGenerator.GeneratorParametersDeSerializer.Deserialize<CodeGenerator.SequenceIdentityGeneratorParameters>(Shape.CompositeType.GeneratorParameters);
                else
                {
                    Shape.CompositeType.GeneratorParameters = null;
                    sequence = new CodeGenerator.SequenceIdentityGeneratorParameters();
                }

                prgGeneratorParameters.SelectedObject = sequence;
            }
            else if (Shape.CompositeType.IdGenerator == "UuidHex")
            {
                this.Height = pnlGeneratorParameters.Bottom + pnlGeneratorParameters.Margin.Bottom;

                CodeGenerator.UuidHexIdentityGeneratorParameters uuidHex = null;

                if (xmlGeneratorParameters.SelectSingleNode("//UuidHexIdentityGeneratorParameters") != null)
                    uuidHex = CodeGenerator.GeneratorParametersDeSerializer.Deserialize<CodeGenerator.UuidHexIdentityGeneratorParameters>(Shape.CompositeType.GeneratorParameters);
                else
                {
                    Shape.CompositeType.GeneratorParameters = null;
                    uuidHex = new CodeGenerator.UuidHexIdentityGeneratorParameters();
                }

                prgGeneratorParameters.SelectedObject = uuidHex;
            }
            else if (Shape.CompositeType.IdGenerator == "Foreign")
            {
                this.Height = pnlGeneratorParameters.Bottom + pnlGeneratorParameters.Margin.Bottom;

                CodeGenerator.ForeignIdentityGeneratorParameters foreign = null;

                if (xmlGeneratorParameters.SelectSingleNode("//ForeignIdentityGeneratorParameters") != null)
                    foreign = CodeGenerator.GeneratorParametersDeSerializer.Deserialize<CodeGenerator.ForeignIdentityGeneratorParameters>(Shape.CompositeType.GeneratorParameters);
                else
                {
                    Shape.CompositeType.GeneratorParameters = null;
                    foreign = new CodeGenerator.ForeignIdentityGeneratorParameters();
                }

                prgGeneratorParameters.SelectedObject = foreign;
            }
            else if (Shape.CompositeType.IdGenerator == "Custom")
            {
                this.Height = pnlGeneratorParameters.Bottom + pnlGeneratorParameters.Margin.Bottom;

                CodeGenerator.CustomIdentityGeneratorParameters custom = null;

                if (xmlGeneratorParameters.SelectSingleNode("//CustomIdentityGeneratorParameters") != null)
                    custom = CodeGenerator.GeneratorParametersDeSerializer.Deserialize<CodeGenerator.CustomIdentityGeneratorParameters>(Shape.CompositeType.GeneratorParameters);
                else
                {
                    Shape.CompositeType.GeneratorParameters = null;
                    custom = new CodeGenerator.CustomIdentityGeneratorParameters();
                }

                prgGeneratorParameters.SelectedObject = custom;
            }
            else
            {
                this.Height = pnlAdvancedOptions.Bottom + pnlAdvancedOptions.Margin.Bottom;
                Shape.CompositeType.GeneratorParameters = null;
            }
        }

        private void prgGeneratorParameters_Validating(object sender, CancelEventArgs e)
        {
            if (Shape.CompositeType.IdGenerator == "HiLo")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.HiLoIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "SeqHiLo")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.SeqHiLoIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "Sequence")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.SequenceIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "UuidHex")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.UuidHexIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "Foreign")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.ForeignIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "Custom")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.CustomIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);

            RefreshValues();
        }

        private void prgGeneratorParameters_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (Shape.CompositeType.IdGenerator == "HiLo")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.HiLoIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "SeqHiLo")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.SeqHiLoIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "Sequence")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.SequenceIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "UuidHex")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.UuidHexIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "Foreign")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.ForeignIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);
            else if (Shape.CompositeType.IdGenerator == "Custom")
                Shape.CompositeType.GeneratorParameters = CodeGenerator.GeneratorParametersDeSerializer.Serialize((CodeGenerator.CustomIdentityGeneratorParameters)prgGeneratorParameters.SelectedObject);

            RefreshValues();
        }
	}
}
