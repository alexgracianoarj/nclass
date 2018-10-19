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
using System.ComponentModel;
using System.Windows.Forms;
using NClass.Core;
using NClass.DiagramEditor.ClassDiagram.Shapes;
using NClass.Translations;
using NClass.DiagramEditor.ClassDiagram.Dialogs;

namespace NClass.DiagramEditor.ClassDiagram.Editors
{
    public partial class CompositeTypeEditor : TypeEditor
	{
		public CompositeTypeEditor()
		{
			InitializeComponent();

			toolStrip.Renderer = ToolStripSimplifiedRenderer.Default;

			if (MonoHelper.IsRunningOnMono)
				toolNewMember.Alignment = ToolStripItemAlignment.Left;
		}

		protected CompositeTypeShape Shape { get; set; }

		protected bool NeedValidation { get; set; }

		internal override void Init(DiagramElement element)
		{
			UpdateTexts();

			Shape = (CompositeTypeShape) element;
			RefreshToolAvailability();
			RefreshValues();
		}

		protected virtual void RefreshToolAvailability()
		{

			toolOverrideList.Visible = Shape.CompositeType is SingleInharitanceType;

			IInterfaceImplementer implementer = Shape.CompositeType as IInterfaceImplementer;
			if (implementer != null)
			{
				toolImplementList.Visible = true;
				toolImplementList.Enabled = implementer.ImplementsInterface;
			}
			else
			{
				toolImplementList.Visible = false;
			}
		}

		protected virtual void UpdateTexts()
		{
			toolNewField.Text = Strings.NewField;
			toolNewMethod.Text = Strings.NewMethod;
			toolNewConstructor.Text = Strings.NewConstructor;
			toolNewDestructor.Text = Strings.NewDestructor;
			toolNewProperty.Text = Strings.NewProperty;
			toolNewEvent.Text = Strings.NewEvent;
			toolOverrideList.Text = Strings.OverrideMembers;
			toolImplementList.Text = Strings.Implementing;
			toolSortByKind.Text = Strings.SortByKind;
			toolSortByAccess.Text = Strings.SortByAccess;
			toolSortByName.Text = Strings.SortByName;
		}

		protected virtual void RefreshValues()
		{
			CompositeType type = Shape.CompositeType;
			SuspendLayout();

			int cursorPosition = txtName.SelectionStart;
			txtName.Text = type.Name;
			txtName.SelectionStart = cursorPosition;

			SetError(null);
			NeedValidation = false;

			bool hasMember = (type.MemberCount > 0);
			toolSortByAccess.Enabled = hasMember;
			toolSortByKind.Enabled = hasMember;
			toolSortByName.Enabled = hasMember;

			RefreshVisibility();
			RefreshModifiers();
			RefreshNewMembers();

			ResumeLayout();
		}

		private void RefreshVisibility()
		{
			Language language = Shape.CompositeType.Language;
			CompositeType type = Shape.CompositeType;

			toolVisibility.Image = Icons.GetImage(type);
			toolVisibility.Text = language.ValidAccessModifiers[type.AccessModifier];

			// Public
			if (language.ValidAccessModifiers.ContainsKey(AccessModifier.Public))
			{
				toolPublic.Visible = true;
				toolPublic.Text = language.ValidAccessModifiers[AccessModifier.Public];
				toolPublic.Image = Icons.GetImage(type.EntityType, AccessModifier.Public);
			}
			else
			{
				toolPublic.Visible = false;
			}
			// Protected Internal
			if (type.IsNested && language.ValidAccessModifiers.ContainsKey(AccessModifier.ProtectedInternal))
			{
				toolProtint.Visible = true;
				toolProtint.Text = language.ValidAccessModifiers[AccessModifier.ProtectedInternal];
				toolProtint.Image = Icons.GetImage(type.EntityType, AccessModifier.ProtectedInternal);
			}
			else
			{
				toolProtint.Visible = false;
			}
			// Internal
			if (language.ValidAccessModifiers.ContainsKey(AccessModifier.Internal))
			{
				toolInternal.Visible = true;
				toolInternal.Text = language.ValidAccessModifiers[AccessModifier.Internal];
				toolInternal.Image = Icons.GetImage(type.EntityType, AccessModifier.Internal);
			}
			else
			{
				toolInternal.Visible = false;
			}
			// Protected
			if (type.IsNested && language.ValidAccessModifiers.ContainsKey(AccessModifier.Protected))
			{
				toolProtected.Visible = true;
				toolProtected.Text = language.ValidAccessModifiers[AccessModifier.Protected];
				toolProtected.Image = Icons.GetImage(type.EntityType, AccessModifier.Protected);
			}
			else
			{
				toolProtected.Visible = false;
			}
			// Private
			if (type.IsNested && language.ValidAccessModifiers.ContainsKey(AccessModifier.Private))
			{
				toolPrivate.Visible = true;
				toolPrivate.Text = language.ValidAccessModifiers[AccessModifier.Private];
				toolPrivate.Image = Icons.GetImage(type.EntityType, AccessModifier.Private);
			}
			else
			{
				toolPrivate.Visible = false;
			}
			// Default
			if (language.ValidAccessModifiers.ContainsKey(AccessModifier.Default))
			{
				toolDefault.Visible = true;
				toolDefault.Text = language.ValidAccessModifiers[AccessModifier.Default];
				toolDefault.Image = Icons.GetImage(type.EntityType, AccessModifier.Default);
			}
			else
			{
				toolDefault.Visible = false;
			}
		}

		private void RefreshModifiers()
		{
			Language language = Shape.CompositeType.Language;

			ClassType classType = Shape.CompositeType as ClassType;
			if (classType != null)
			{
				toolModifier.Visible = true;
				if (classType.Modifier == ClassModifier.None)
					toolModifier.Text = Strings.None;
				else
					toolModifier.Text = language.ValidClassModifiers[classType.Modifier];

				// Abstract modifier
				if (language.ValidClassModifiers.ContainsKey(ClassModifier.Abstract))
				{
					toolAbstract.Visible = true;
					toolAbstract.Text = language.ValidClassModifiers[ClassModifier.Abstract];
				}
				else
				{
					toolAbstract.Visible = false;
				}
				// Sealed modifier
				if (language.ValidClassModifiers.ContainsKey(ClassModifier.Sealed))
				{
					toolSealed.Visible = true;
					toolSealed.Text = language.ValidClassModifiers[ClassModifier.Sealed];
				}
				else
				{
					toolSealed.Visible = false;
				}
				// Static modifier
				if (language.ValidClassModifiers.ContainsKey(ClassModifier.Static))
				{
					toolStatic.Visible = true;
					toolStatic.Text = language.ValidClassModifiers[ClassModifier.Static];
				}
				else
				{
					toolStatic.Visible = false;
				}
			}
			else
			{
				toolModifier.Visible = false;
			}
		}

		private void RefreshNewMembers()
		{
			bool valid = false;
			switch (NewMemberType)
			{
				case MemberType.Field:
					if (Shape.CompositeType.SupportsFields)
					{
						toolNewMember.Image = Properties.Resources.NewField;
						toolNewMember.Text = Strings.NewField;
						valid = true;
					}
					break;

				case MemberType.Method:
					if (Shape.CompositeType.SupportsMethods)
					{
						toolNewMember.Image = Properties.Resources.NewMethod;
						toolNewMember.Text = Strings.NewMethod;
						valid = true;
					}
					break;

				case MemberType.Constructor:
					if (Shape.CompositeType.SupportsConstuctors)
					{
						toolNewMember.Image = Properties.Resources.NewConstructor;
						toolNewMember.Text = Strings.NewConstructor;
						valid = true;
					}
					break;

				case MemberType.Destructor:
					if (Shape.CompositeType.SupportsDestructors)
					{
						toolNewMember.Image = Properties.Resources.NewDestructor;
						toolNewMember.Text = Strings.NewDestructor;
						valid = true;
					}
					break;

				case MemberType.Property:
					if (Shape.CompositeType.SupportsProperties)
					{
						toolNewMember.Image = Properties.Resources.NewProperty;
						toolNewMember.Text = Strings.NewProperty;
						valid = true;
					}
					break;

				case MemberType.Event:
					if (Shape.CompositeType.SupportsEvents)
					{
						toolNewMember.Image = Properties.Resources.NewEvent;
						toolNewMember.Text = Strings.NewEvent;
						valid = true;
					}
					break;
			}

			if (!valid)
			{
				NewMemberType = MemberType.Method;
				toolNewMember.Image = Properties.Resources.NewMethod;
				toolNewMember.Text = Strings.NewMethod;
			}

			toolNewField.Visible = Shape.CompositeType.SupportsFields;
			toolNewMethod.Visible = Shape.CompositeType.SupportsMethods;
			toolNewConstructor.Visible = Shape.CompositeType.SupportsConstuctors;
			toolNewDestructor.Visible = Shape.CompositeType.SupportsDestructors;
			toolNewProperty.Visible = Shape.CompositeType.SupportsProperties;
			toolNewEvent.Visible = Shape.CompositeType.SupportsEvents;
		}

		public override void ValidateData()
		{
			ValidateName();

			SetError(null);
		}

		private bool ValidateName()
		{
			if (NeedValidation)
			{
				try
				{
					Shape.CompositeType.Name = txtName.Text;
                    
                    if(CodeGenerator.Settings.Default.GenerateNHibernateMapping
                        && string.IsNullOrEmpty(Shape.CompositeType.NHMTableName))
                    {
                        if (CodeGenerator.Settings.Default.UseUnderscoreAndLowercaseInDB)
                            Shape.CompositeType.NHMTableName = new CodeGenerator.LowercaseAndUnderscoreTextFormatter().FormatText(Shape.CompositeType.Name);
                        else
                            Shape.CompositeType.NHMTableName = Shape.CompositeType.Name;
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

		protected void SetError(string message)
		{
			if (MonoHelper.IsRunningOnMono && MonoHelper.IsOlderVersionThan("2.4"))
				return;

			errorProvider.SetError(this, message);
		}

		private void ChangeAccess(AccessModifier access)
		{
			if (ValidateName())
			{
				try
				{
					Shape.CompositeType.AccessModifier = access;
					RefreshValues();
				}
				catch (BadSyntaxException ex)
				{
					RefreshValues();
					SetError(ex.Message);
				}
			}
		}

		private void ChangeModifier(ClassModifier modifier)
		{
			if (ValidateName())
			{
				ClassType classType = Shape.CompositeType as ClassType;
				if (classType != null)
				{
					try
					{
						classType.Modifier = modifier;
						RefreshValues();
					}
					catch (BadSyntaxException ex)
					{
						RefreshValues();
						SetError(ex.Message);
					}
				}
			}
		}

		private void txtName_KeyDown(object sender, KeyEventArgs e)
		{
			HandleCompositeTypeTextBoxKeyDown(ValidateName, e.KeyCode, e.Modifiers);
		}

		/// <summary>
		/// Handles keydown logic for textboxes that are used to modify attributes of a composite type.
		/// </summary>
		/// <param name="textBoxValidationMethod">The validation method used to ensure that the text entered is valid.</param>
		/// <param name="keyDown">The key that has been pressed.</param>
		/// <param name="modifierKeys">Any modifier keys that have been pressed.</param>
		protected void HandleCompositeTypeTextBoxKeyDown(Func<bool> textBoxValidationMethod, Keys keyCode, Keys modifierKeys)
		{
			switch (keyCode)
			{
				case Keys.Enter:
					if (modifierKeys == Keys.Control || modifierKeys == Keys.Shift)
					{
						OpenNewMemberDropDown();
					}
					else
					{
						textBoxValidationMethod();
					}
					break;

				case Keys.Escape:
					NeedValidation = false;
					Shape.HideEditor();
					break;

				case Keys.Down:
					Shape.ActiveMemberIndex = 0;
					break;
			}

			if (modifierKeys == (Keys.Control | Keys.Shift))
			{
				switch (keyCode)
				{
					case Keys.A:
						AddNewMember();
						break;

					case Keys.F:
						AddNewMember(MemberType.Field);
						break;

					case Keys.M:
						AddNewMember(MemberType.Method);
						break;

					case Keys.C:
						AddNewMember(MemberType.Constructor);
						break;

					case Keys.D:
						AddNewMember(MemberType.Destructor);
						break;

					case Keys.P:
						AddNewMember(MemberType.Property);
						break;

					case Keys.E:
						AddNewMember(MemberType.Event);
						break;
				}
			}
		}

		private void OpenNewMemberDropDown()
		{
			toolNewMember.ShowDropDown();

			switch (NewMemberType)
			{
				case MemberType.Field:
					toolNewField.Select();
					break;

				case MemberType.Method:
					toolNewMethod.Select();
					break;

				case MemberType.Constructor:
					toolNewConstructor.Select();
					break;

				case MemberType.Destructor:
					toolNewDestructor.Select();
					break;

				case MemberType.Property:
					toolNewProperty.Select();
					break;

				case MemberType.Event:
					toolNewEvent.Select();
					break;
			}
		}

		protected void textBox_TextChanged(object sender, EventArgs e)
		{
			NeedValidation = true;
		}

		private void txtName_Validating(object sender, CancelEventArgs e)
		{
			ValidateName();
		}

		private void toolPublic_Click(object sender, EventArgs e)
		{
			ChangeAccess(AccessModifier.Public);
		}

		private void toolProtint_Click(object sender, EventArgs e)
		{
			ChangeAccess(AccessModifier.ProtectedInternal);
		}

		private void toolInternal_Click(object sender, EventArgs e)
		{
			ChangeAccess(AccessModifier.Internal);
		}

		private void toolProtected_Click(object sender, EventArgs e)
		{
			ChangeAccess(AccessModifier.Protected);
		}

		private void toolPrivate_Click(object sender, EventArgs e)
		{
			ChangeAccess(AccessModifier.Private);
		}

		private void toolDefault_Click(object sender, EventArgs e)
		{
			ChangeAccess(AccessModifier.Default);
		}

		private void toolNone_Click(object sender, EventArgs e)
		{
			ChangeModifier(ClassModifier.None);
		}

		private void toolAbstract_Click(object sender, EventArgs e)
		{
			ChangeModifier(ClassModifier.Abstract);
		}

		private void toolSealed_Click(object sender, EventArgs e)
		{
			ChangeModifier(ClassModifier.Sealed);
		}

		private void toolStatic_Click(object sender, EventArgs e)
		{
			ChangeModifier(ClassModifier.Static);
		}

		private void AddNewMember()
		{
			AddNewMember(NewMemberType);
		}

		private void AddNewMember(MemberType type)
		{
			if (!ValidateName())
				return;

			NewMemberType = type;
			switch (type)
			{
				case MemberType.Field:
					if (Shape.CompositeType.SupportsFields)
					{
						Shape.CompositeType.AddField();
						Shape.ActiveMemberIndex = Shape.CompositeType.FieldCount - 1;
					}
					break;

				case MemberType.Method:
					if (Shape.CompositeType.SupportsMethods)
					{
						Shape.CompositeType.AddMethod();
						Shape.ActiveMemberIndex = Shape.CompositeType.MemberCount - 1;
					}
					break;

				case MemberType.Constructor:
					if (Shape.CompositeType.SupportsConstuctors)
					{
						Shape.CompositeType.AddConstructor();
						Shape.ActiveMemberIndex = Shape.CompositeType.MemberCount - 1;
					}
					break;

				case MemberType.Destructor:
					if (Shape.CompositeType.SupportsDestructors)
					{
						Shape.CompositeType.AddDestructor();
						Shape.ActiveMemberIndex = Shape.CompositeType.MemberCount - 1;
					}
					break;

				case MemberType.Property:
					if (Shape.CompositeType.SupportsProperties)
					{
						Shape.CompositeType.AddProperty();
						Shape.ActiveMemberIndex = Shape.CompositeType.MemberCount - 1;
					}
					break;

				case MemberType.Event:
					if (Shape.CompositeType.SupportsEvents)
					{
						Shape.CompositeType.AddEvent();
						Shape.ActiveMemberIndex = Shape.CompositeType.MemberCount - 1;
					}
					break;
			}
			txtName.SelectionStart = 0;
		}

		private void toolNewMember_ButtonClick(object sender, EventArgs e)
		{
			AddNewMember();
		}

		private void toolNewField_Click(object sender, EventArgs e)
		{
			AddNewMember(MemberType.Field);
		}

		private void toolNewMethod_Click(object sender, EventArgs e)
		{
			AddNewMember(MemberType.Method);
		}

		private void toolNewProperty_Click(object sender, EventArgs e)
		{
			AddNewMember(MemberType.Property);
		}

		private void toolNewEvent_Click(object sender, EventArgs e)
		{
			AddNewMember(MemberType.Event);
		}

		private void toolNewConstructor_Click(object sender, EventArgs e)
		{
			AddNewMember(MemberType.Constructor);			
		}

		private void toolNewDestructor_Click(object sender, EventArgs e)
		{
			AddNewMember(MemberType.Destructor);
		}

		private void toolOverrideList_Click(object sender, EventArgs e)
		{
			SingleInharitanceType type = Shape.CompositeType as SingleInharitanceType;
			if (type != null)
			{
				using (OverrideDialog dialog = new OverrideDialog())
				{
					if (dialog.ShowDialog(type) == DialogResult.OK)
					{
						foreach (Operation operation in dialog.GetSelectedOperations())
						{
							type.Override(operation);
						}
					}
				}
			}
		}

		private void toolImplementList_Click(object sender, EventArgs e)
		{
			IInterfaceImplementer type = Shape.CompositeType as IInterfaceImplementer;
			if (type != null)
			{
				using (ImplementDialog dialog = new ImplementDialog())
				{
					if (dialog.ShowDialog(type) == DialogResult.OK)
					{
						foreach (Operation operation in dialog.GetSelectedOperations())
						{
							Operation defined = type.GetDefinedOperation(operation);
							bool implementExplicitly = dialog.ImplementExplicitly &&
								type.Language.SupportsExplicitImplementation;

							if (defined == null)
							{
								type.Implement(operation, implementExplicitly);
							}
							else if (defined.Type != operation.Type)
							{
								type.Implement(operation, true);
							}
						}
					}
				}
			}
		}

		private void toolSortByKind_Click(object sender, EventArgs e)
		{
			Shape.CompositeType.SortMembers(SortingMode.ByKind);
		}

		private void toolSortByAccess_Click(object sender, EventArgs e)
		{
			Shape.CompositeType.SortMembers(SortingMode.ByAccess);
		}

		private void toolSortByName_Click(object sender, EventArgs e)
		{
			Shape.CompositeType.SortMembers(SortingMode.ByName);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			txtName.SelectionStart = 0;
		}
	}
}
