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
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using NClass.DiagramEditor;
using NClass.Translations;

namespace NClass.DiagramEditor.ClassDiagram.Dialogs
{
	public partial class DiagramPrintDialog : Form
	{
		IDocument document;
		int pageIndex = 0;
		int rows = 1;
		int columns = 1;
		bool selectedOnly = false;
		Style selectedStyle = Style.CurrentStyle;
		Style printingStyle = null;

		/// <exception cref="ArgumentNullException">
		/// <paramref name="document"/> is null.
		/// </exception>
		public DiagramPrintDialog(IDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			this.document = document;

			InitializeComponent();
			printPreview.AutoZoom = true;
			printDocument.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
			printDocument.DocumentName = document.Name;
		}

		private int PageCount
		{
			get { return rows * columns; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			UpdateTexts();
			LoadStyles();
		}

		private void UpdateTexts()
		{
			this.Text = Strings.Print;
			btnPrinter.Text = Strings.ButtonSelectPrinter;
			btnPageSetup.Text = Strings.ButtonPageSetup;
			lblStyle.Text = Strings.Style;
			lblPages.Text = Strings.Pages;
			chkSelectedOnly.Text = Strings.PrintOnlySelectedElements;
			btnPrint.Text = Strings.ButtonPrint;
			btnCancel.Text = Strings.ButtonCancel;

			int buttonWidth = Math.Max(btnPrinter.Width, btnPageSetup.Width);
			btnPrinter.Width = buttonWidth;
			btnPageSetup.Width = buttonWidth;

			int minLeft = btnPrinter.Left + buttonWidth + 6;
			lblStyle.Left = minLeft;
			lblPages.Left = minLeft;

			minLeft = Math.Max(lblStyle.Right, lblPages.Right);
			cboStyle.Left = minLeft + 6;
			numColumns.Left = minLeft + 6;
			lblX.Left = numColumns.Right + 1;
			numRows.Left = lblX.Right + 1;
			chkSelectedOnly.Left = numRows.Right + 14;
		}

		private void LoadStyles()
		{
			cboStyle.Items.Clear();
			foreach (Style style in Style.AvaiableStyles)
			{
				cboStyle.Items.Add(style);
				if (style == Style.CurrentStyle)
					cboStyle.SelectedItem = style;
			}
		}

		public new DialogResult ShowDialog()
		{
			return ShowDialog(null);
		}

		public new DialogResult ShowDialog(IWin32Window owner)
		{
			if (printDocument.PrinterSettings.IsValid)
			{
				return base.ShowDialog(owner);
			}
			else
			{
				MessageBox.Show(Strings.ErrorNoPrinters, Strings.Error,
					MessageBoxButtons.OK, MessageBoxIcon.Error);

				return DialogResult.Cancel;
			}
		}

		private void Print()
		{
			try
			{
				printDocument.Print();
			}
			catch (InvalidPrinterException ex)
			{
				MessageBox.Show(string.Format(Strings.ErrorPrinting, ex.Message),
					Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static Style ConvertStyle(Style selectedStyle)
		{
			Style converted = selectedStyle.Clone();
			converted.ShadowColor = DisableTransparency(converted.ShadowColor);
			return converted;
		}

		private static Color DisableTransparency(Color color)
		{
			int red = color.R * color.A / 255 + (255 - color.A);
			int green = color.G * color.A / 255 + (255 - color.A);
			int blue = color.B * color.A / 255 + (255 - color.A);

			return Color.FromArgb(red, green, blue);
		}

		private void printDocument_BeginPrint(object sender, PrintEventArgs e)
		{
			pageIndex = 0;
			printingStyle = ConvertStyle(selectedStyle);
		}

		private void printDocument_PrintPage(object sender, PrintPageEventArgs e)
		{
			e.Graphics.PageUnit = GraphicsUnit.Inch;
			e.Graphics.PageScale = 1 / DiagramElement.Graphics.DpiX;

			int column = pageIndex % columns;
			int row = pageIndex / columns;

			RectangleF drawingArea = document.GetPrintingArea(selectedOnly);

			float scaleX = e.MarginBounds.Width * columns / drawingArea.Width;
			float scaleY = e.MarginBounds.Height * rows / drawingArea.Height;
			float scale = Math.Min(scaleX, scaleY);
			if (scale > 1) scale = 1;

			// Set the printing clip region
			Rectangle clipBounds = e.MarginBounds;
			if (column == 0)
			{
				clipBounds.X = 0;
				clipBounds.Width += e.MarginBounds.Left;
			}
			if (row == 0)
			{
				clipBounds.Y = 0;
				clipBounds.Height += e.MarginBounds.Top;
			}
			if (column == columns - 1)
			{
				clipBounds.Width += e.MarginBounds.Left;
			}
			if (row == rows - 1)
			{
				clipBounds.Height += e.MarginBounds.Top;
			}
			e.Graphics.SetClip(clipBounds);

			// Moving the image to it's right position
			e.Graphics.TranslateTransform(-column * e.MarginBounds.Width, -row * e.MarginBounds.Height);
			e.Graphics.TranslateTransform(e.MarginBounds.Left, e.MarginBounds.Top);
			e.Graphics.ScaleTransform(scale, scale);
			e.Graphics.TranslateTransform(-drawingArea.Left, -drawingArea.Top);
			// Printing
			document.Print(e.Graphics, selectedOnly, printingStyle);
			e.HasMorePages = (++pageIndex < PageCount);
		}

		private void printDocument_EndPrint(object sender, PrintEventArgs e)
		{
			printingStyle.Dispose();
		}

		private void printPreview_Click(object sender, EventArgs e)
		{
			if (printPreview.AutoZoom)
			{
				printPreview.AutoZoom = false;
				printPreview.Zoom = 1.0;
			}
			else
			{
				printPreview.AutoZoom = true;
			}
		}

		private void btnPrint_Click(object sender, EventArgs e)
		{
			Print();
		}

		private void btnPrinter_Click(object sender, EventArgs e)
		{
			if (selectPrinterDialog.ShowDialog() == DialogResult.OK)
			{
				Print();
				this.Close();
			}
		}

		private void btnPageSetup_Click(object sender, EventArgs e)
		{
			Margins originalMargins = pageSetupDialog.PageSettings.Margins;

			if (System.Globalization.RegionInfo.CurrentRegion.IsMetric &&
				!MonoHelper.IsRunningOnMono)
			{
				// This is necessary because of a bug in PageSetupDialog control.
				// More information: http://support.microsoft.com/?id=814355
				pageSetupDialog.PageSettings.Margins = PrinterUnitConvert.Convert(
					pageSetupDialog.PageSettings.Margins,
					PrinterUnit.Display, PrinterUnit.TenthsOfAMillimeter);
			}

			if (pageSetupDialog.ShowDialog() != DialogResult.OK)
				pageSetupDialog.PageSettings.Margins = originalMargins;

			printPreview.InvalidatePreview();
		}

		private void cboStyle_SelectedIndexChanged(object sender, EventArgs e)
		{
			Style style = cboStyle.SelectedItem as Style;
			if (style != null)
			{
				selectedStyle = style;
				printPreview.InvalidatePreview();
			}
		}

		private void numColumns_ValueChanged(object sender, EventArgs e)
		{
			columns = (int) numColumns.Value;
			printPreview.Columns = columns;
			printPreview.AutoZoom = true;
			printPreview.InvalidatePreview();
		}

		private void numRows_ValueChanged(object sender, EventArgs e)
		{
			rows = (int) numRows.Value;
			printPreview.Rows = rows;
			printPreview.AutoZoom = true;
			printPreview.InvalidatePreview();
		}

		private void chkSelectedOnly_CheckedChanged(object sender, EventArgs e)
		{
			selectedOnly = (chkSelectedOnly.Checked);
			printPreview.InvalidatePreview();
		}
	}
}