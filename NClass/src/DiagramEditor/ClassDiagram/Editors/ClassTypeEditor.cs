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
			if (type.Stereotype != null)
			{
				ExpandAdvancedOptions();
			}
			else
			{
				CollapseAdvancedOptions();
			}
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
            int cursorPositionHbmTable = txtNHMTableName.SelectionStart;

            txtStereotype.Text = type.Stereotype;
            txtNHMTableName.Text = type.NHMTableName;

			// Remove the angled characters when the stereotype is displayed in the editor.
			if (!string.IsNullOrEmpty(txtStereotype.Text))
			{
				txtStereotype.Text = txtStereotype.Text.Replace("«", "").Replace("»", "");
			}

            txtStereotype.SelectionStart = cursorPositionStereotype;
            txtNHMTableName.SelectionStart = cursorPositionHbmTable;

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
            this.Height = pnlAdvancedOptionsHbmTable.Bottom + pnlAdvancedOptionsHbmTable.Margin.Bottom;

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
			}

			Refresh();
		}

        private void borderedTextBox1_Validating(object sender, CancelEventArgs e)
        {
            ValidateNHMTableName();
        }

        /// <summary>
        /// Validates the stereotype that has been entered into the stereotype textbox.
        /// </summary>
        /// <returns>True if the stereotype is valid, false if it is not.</returns>
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
                            throw new BadSyntaxException("Invalid HBM Table Name");

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
	}
}
