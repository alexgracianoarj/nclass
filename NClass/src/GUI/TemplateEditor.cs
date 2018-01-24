using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Drawing.Drawing2D;

using NClass.CodeGenerator;

namespace NClass.GUI
{
    public partial class TemplateEditor : Form
    {
        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while", "add", "alias", "ascending", "descending", "dynamic", "from", "get", "global", "group", "into", "join", "let", "orderby", "partial", "remove", "select", "set", "value", "var", "where", "yield" };
        string[] methods = { "Equals()", "GetHashCode()", "GetType()", "ToString()" };
        string[] snippets = { "if(^)\n{\n;\n}", "if(^)\n{\n;\n}\nelse\n{\n;\n}", "for(^;;)\n{\n;\n}", "while(^)\n{\n;\n}", "do\n{\n^;\n}while();", "switch(^)\n{\ncase : break;\n}" };
        string[] declarationSnippets = { 
               "public class ^\n{\n}", "private class ^\n{\n}", "internal class ^\n{\n}",
               "public struct ^\n{\n;\n}", "private struct ^\n{\n;\n}", "internal struct ^\n{\n;\n}",
               "public void ^()\n{\n;\n}", "private void ^()\n{\n;\n}", "internal void ^()\n{\n;\n}", "protected void ^()\n{\n;\n}",
               "public ^{ get; set; }", "private ^{ get; set; }", "internal ^{ get; set; }", "protected ^{ get; set; }"
               };
        Style invisibleCharsStyle = new InvisibleCharsRenderer(Pens.Gray);
        Color currentLineColor = Color.FromArgb(100, 210, 210, 255);
        Color changedLineColor = Color.FromArgb(255, 230, 230, 255);

        TemplateSettings template;
        TemplatesSettings templates;

        private Style sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(50, Color.Gray)));

        public TemplateEditor()
        {
            InitializeComponent();

            fctbCode.Font = new Font("Consolas", 9.75f);
            fctbCode.ContextMenuStrip = cmMain;
            fctbCode.Dock = DockStyle.Fill;
            fctbCode.BorderStyle = BorderStyle.Fixed3D;
            //fctb.VirtualSpace = true;
            fctbCode.LeftPadding = 17;
            fctbCode.Language = Language.CSharp;
            fctbCode.AddStyle(sameWordsStyle);//same words style
            fctbCode.Tag = new TbInfo();
            fctbCode.Focus();
            fctbCode.DelayedTextChangedInterval = 1000;
            fctbCode.DelayedEventsInterval = 500;
            fctbCode.TextChangedDelayed += new EventHandler<TextChangedEventArgs>(tb_TextChangedDelayed);
            fctbCode.SelectionChangedDelayed += new EventHandler(tb_SelectionChangedDelayed);
            fctbCode.KeyDown += new KeyEventHandler(tb_KeyDown);
            fctbCode.MouseMove += new MouseEventHandler(tb_MouseMove);
            fctbCode.ChangedLineColor = changedLineColor;
            if (btHighlightCurrentLine.Checked)
                fctbCode.CurrentLineColor = currentLineColor;
            fctbCode.ShowFoldingLines = btShowFoldingLines.Checked;
            fctbCode.HighlightingRangeType = HighlightingRangeType.VisibleRange;
            
            //create autocomplete popup menu
            AutocompleteMenu popupMenu = new AutocompleteMenu(fctbCode);
            popupMenu.Items.ImageList = ilAutocomplete;
            popupMenu.Opening += new EventHandler<CancelEventArgs>(popupMenu_Opening);
            BuildAutocompleteMenu(popupMenu);
            (fctbCode.Tag as TbInfo).popupMenu = popupMenu;

            //init menu images
            copyToolStripMenuItem.Image = global::NClass.GUI.Properties.Resources.Copy;
            cutToolStripMenuItem.Image = global::NClass.GUI.Properties.Resources.Cut;
            pasteToolStripMenuItem.Image = global::NClass.GUI.Properties.Resources.Paste;

            PopulateTemplates();
        }

        private void PopulateTemplates()
        {
            templates = TemplatesSettings.Load();
            toolStripComboBox1.ComboBox.DataSource = templates.Templates;
            toolStripComboBox1.ComboBox.DisplayMember = "Name";
        }

        private void BindData()
        {
            toolStripTextBox1.Text = template.Name;
            toolStripButton1.Checked = template.Enabled;
            toolStripButton11.Checked = template.PerEntity;
            toolStripTextBox2.Text = template.FileName;
            fctbCode.Text = template.Code;

            fctbCode.ClearUndo();
        }

        void popupMenu_Opening(object sender, CancelEventArgs e)
        {
            //---block autocomplete menu for comments
            //get index of green style (used for comments)
            var iGreenStyle = fctbCode.GetStyleIndex(fctbCode.SyntaxHighlighter.GreenStyle);
            if (iGreenStyle >= 0)
                if (fctbCode.Selection.Start.iChar > 0)
                {
                    //current char (before caret)
                    var c = fctbCode[fctbCode.Selection.Start.iLine][fctbCode.Selection.Start.iChar - 1];
                    //green Style
                    var greenStyleIndex = Range.ToStyleIndex(iGreenStyle);
                    //if char contains green style then block popup menu
                    if ((c.style & greenStyleIndex) != 0)
                        e.Cancel = true;
                }
        }

        private void BuildAutocompleteMenu(AutocompleteMenu popupMenu)
        {
            List<AutocompleteItem> items = new List<AutocompleteItem>();

            foreach (var item in snippets)
                items.Add(new SnippetAutocompleteItem(item) { ImageIndex = 1 });
            foreach (var item in declarationSnippets)
                items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
            foreach (var item in methods)
                items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
            foreach (var item in keywords)
                items.Add(new AutocompleteItem(item));

            items.Add(new InsertSpaceSnippet());
            items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            items.Add(new InsertEnterSnippet());

            //set as autocomplete source
            popupMenu.Items.SetAutocompleteItems(items);
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
        }

        void tb_MouseMove(object sender, MouseEventArgs e)
        {
            var tb = sender as FastColoredTextBox;
            var place = tb.PointToPlace(e.Location);
            var r = new Range(tb, place, place);

            string text = r.GetFragment("[a-zA-Z]").Text;
            lbWordUnderMouse.Text = text;
        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.OemMinus)
            {
                NavigateBackward();
                e.Handled = true;
            }

            if (e.Modifiers == (Keys.Control|Keys.Shift) && e.KeyCode == Keys.OemMinus)
            {
                NavigateForward();
                e.Handled = true;
            }

            if (e.KeyData == (Keys.Control | Keys.Space))
            {
                //forced show (MinFragmentLength will be ignored)
                (fctbCode.Tag as TbInfo).popupMenu.Show(true);
                e.Handled = true;
            }

            if (e.KeyData == (Keys.Control | Keys.D))
            {
                //expand selection
                fctbCode.Selection.Expand();
                //get text of selected lines
                string text = Environment.NewLine + fctbCode.Selection.Text;
                //move caret to end of selected lines
                fctbCode.Selection.Start = fctbCode.Selection.End;
                //insert text
                fctbCode.InsertText(text);
                e.Handled = true;
            }
        }

        void tb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            var tb = sender as FastColoredTextBox;
            //remember last visit time
            if (tb.Selection.IsEmpty && tb.Selection.Start.iLine < tb.LinesCount)
            {
                if (lastNavigatedDateTime != tb[tb.Selection.Start.iLine].LastVisit)
                {
                    tb[tb.Selection.Start.iLine].LastVisit = DateTime.Now;
                    lastNavigatedDateTime = tb[tb.Selection.Start.iLine].LastVisit;
                }
            }

            //highlight same words
            tb.VisibleRange.ClearStyle(sameWordsStyle);
            if (!tb.Selection.IsEmpty)
                return;//user selected diapason
            //get fragment around caret
            var fragment = tb.Selection.GetFragment(@"\w");
            string text = fragment.Text;
            if (text.Length == 0)
                return;
            //highlight same words
            Range[] ranges = tb.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();

            if (ranges.Length > 1)
                foreach (var r in ranges)
                    r.SetStyle(sameWordsStyle);
        }

        void tb_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            //rebuild object explorer
            string text = fctbCode.Text;
            ThreadPool.QueueUserWorkItem(
                (o)=>ReBuildObjectExplorer(text)
            );

            //show invisible chars
            HighlightInvisibleChars(e.ChangedRange);
        }

        private void HighlightInvisibleChars(Range range)
        {
            range.ClearStyle(invisibleCharsStyle);
            if (btInvisibleChars.Checked)
                range.SetStyle(invisibleCharsStyle, @".$|.\r\n|\s");
        }

        List<ExplorerItem> explorerList = new List<ExplorerItem>();

        private void ReBuildObjectExplorer(string text)
        {
            try
            {
                List<ExplorerItem> list = new List<ExplorerItem>();
                int lastClassIndex = -1;
                //find classes, methods and properties
                Regex regex = new Regex(@"^(?<range>[\w\s]+\b(class|struct|enum|interface)\s+[\w<>,\s]+)|^\s*(public|private|internal|protected)[^\n]+(\n?\s*{|;)?", RegexOptions.Multiline);
                foreach (Match r in regex.Matches(text))
                    try
                    {
                        string s = r.Value;
                        int i = s.IndexOfAny(new char[] { '=', '{', ';' });
                        if (i >= 0)
                            s = s.Substring(0, i);
                        s = s.Trim();

                        var item = new ExplorerItem() { title = s, position = r.Index };
                        if (Regex.IsMatch(item.title, @"\b(class|struct|enum|interface)\b"))
                        {
                            item.title = item.title.Substring(item.title.LastIndexOf(' ')).Trim();
                            item.type = ExplorerItemType.Class;
                            list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());
                            lastClassIndex = list.Count;
                        }
                        else
                            if (item.title.Contains(" event "))
                            {
                                int ii = item.title.LastIndexOf(' ');
                                item.title = item.title.Substring(ii).Trim();
                                item.type = ExplorerItemType.Event;
                            }
                            else
                                if (item.title.Contains("("))
                                {
                                    var parts = item.title.Split('(');
                                    item.title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "(" + parts[1];
                                    item.type = ExplorerItemType.Method;
                                }
                                else
                                    if (item.title.EndsWith("]"))
                                    {
                                        var parts = item.title.Split('[');
                                        if (parts.Length < 2) continue;
                                        item.title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "[" + parts[1];
                                        item.type = ExplorerItemType.Method;
                                    }
                                    else
                                    {
                                        int ii = item.title.LastIndexOf(' ');
                                        item.title = item.title.Substring(ii).Trim();
                                        item.type = ExplorerItemType.Property;
                                    }
                        list.Add(item);
                    }
                    catch { ;}

                list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());

                BeginInvoke(
                    new Action(() =>
                        {
                            explorerList = list;
                            dgvObjectExplorer.RowCount = explorerList.Count;
                            dgvObjectExplorer.Invalidate();
                        })
                );
            }
            catch { ;}
        }

        enum ExplorerItemType
        {
            Class, Method, Property, Event
        }

        class ExplorerItem
        {
            public ExplorerItemType type;
            public string title;
            public int position;
        }

        class ExplorerItemComparer : IComparer<ExplorerItem>
        {
            public int Compare(ExplorerItem x, ExplorerItem y)
            {
                return x.title.CompareTo(y.title);
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.Selection.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fctbCode.UndoEnabled)
                fctbCode.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fctbCode.RedoEnabled)
                fctbCode.Redo();
        }

        private void tmUpdateInterface_Tick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(fctbCode.Text))
            {
                undoStripButton.Enabled = undoToolStripMenuItem.Enabled = fctbCode.UndoEnabled;
                redoStripButton.Enabled = redoToolStripMenuItem.Enabled = fctbCode.RedoEnabled;
                cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
                copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = !fctbCode.Selection.IsEmpty;
                toolStripButton5.Enabled = true;
            }
            else
            {
                undoStripButton.Enabled = undoToolStripMenuItem.Enabled = false;
                redoStripButton.Enabled = redoToolStripMenuItem.Enabled = false;
                cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
                copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = false;
                toolStripButton5.Enabled = false;
                dgvObjectExplorer.RowCount = 0;
            }
        }
        
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.ShowReplaceDialog();
        }

        private void PowerfulCSharpEditor_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void dgvObjectExplorer_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var item = explorerList[e.RowIndex];
            fctbCode.GoEnd();
            fctbCode.SelectionStart = item.position;
            fctbCode.DoSelectionVisible();
            fctbCode.Focus();
        }

        private void dgvObjectExplorer_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                ExplorerItem item = explorerList[e.RowIndex];
                if (e.ColumnIndex == 1)
                    e.Value = item.title;
                else
                    switch (item.type)
                    {
                        case ExplorerItemType.Class:
                            e.Value = global::NClass.GUI.Properties.Resources.class_libraries;
                            return;
                        case ExplorerItemType.Method:
                            e.Value = global::NClass.GUI.Properties.Resources.box;
                            return;
                        case ExplorerItemType.Event:
                            e.Value = global::NClass.GUI.Properties.Resources.lightning;
                            return;
                        case ExplorerItemType.Property:
                            e.Value = global::NClass.GUI.Properties.Resources.property;
                            return;
                    }
            }
            catch{;}
        }

        private void backStripButton_Click(object sender, EventArgs e)
        {
            NavigateBackward();
        }

        private void forwardStripButton_Click(object sender, EventArgs e)
        {
            NavigateForward();
        }

        DateTime lastNavigatedDateTime = DateTime.Now;

        private bool NavigateBackward()
        {
            DateTime max = new DateTime();
            int iLine = -1;

            for (int i = 0; i < fctbCode.LinesCount; i++)
                if (fctbCode[i].LastVisit < lastNavigatedDateTime && fctbCode[i].LastVisit > max)
                {
                    max = fctbCode[i].LastVisit;
                    iLine = i;
                }

            if (iLine >= 0)
            {
                fctbCode.Navigate(iLine);
                lastNavigatedDateTime = fctbCode[iLine].LastVisit;
                Console.WriteLine("Backward: " + lastNavigatedDateTime);
                fctbCode.Focus();
                fctbCode.Invalidate();
                return true;
            }
            else
                return false;
        }

        private bool NavigateForward()
        {
            DateTime min = DateTime.Now;
            int iLine = -1;

            for (int i = 0; i < fctbCode.LinesCount; i++)
                if (fctbCode[i].LastVisit > lastNavigatedDateTime && fctbCode[i].LastVisit < min)
                {
                    min = fctbCode[i].LastVisit;
                    iLine = i;
                }

            if (iLine >= 0)
            {
                fctbCode.Navigate(iLine);
                lastNavigatedDateTime = fctbCode[iLine].LastVisit;
                Console.WriteLine("Forward: " + lastNavigatedDateTime);
                fctbCode.Focus();
                fctbCode.Invalidate();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This item appears when any part of snippet text is typed
        /// </summary>
        class DeclarationSnippet : SnippetAutocompleteItem
        {
            public DeclarationSnippet(string snippet)
                : base(snippet)
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var pattern = Regex.Escape(fragmentText);
                if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
                    return CompareResult.Visible;
                return CompareResult.Hidden;
            }
        }

        /// <summary>
        /// Divides numbers and words: "123AND456" -> "123 AND 456"
        /// Or "i=2" -> "i = 2"
        /// </summary>
        class InsertSpaceSnippet : AutocompleteItem
        {
            string pattern;

            public InsertSpaceSnippet(string pattern)
                : base("")
            {
                this.pattern = pattern;
            }

            public InsertSpaceSnippet()
                : this(@"^(\d+)([a-zA-Z_]+)(\d*)$")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                if (Regex.IsMatch(fragmentText, pattern))
                {
                    Text = InsertSpaces(fragmentText);
                    if (Text != fragmentText)
                        return CompareResult.Visible;
                }
                return CompareResult.Hidden;
            }

            public string InsertSpaces(string fragment)
            {
                var m = Regex.Match(fragment, pattern);
                if (m == null)
                    return fragment;
                if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
                    return fragment;
                return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return Text;
                }
            }
        }

        /// <summary>
        /// Inerts line break after '}'
        /// </summary>
        class InsertEnterSnippet : AutocompleteItem
        {
            Place enterPlace = Place.Empty;

            public InsertEnterSnippet()
                : base("[Line break]")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var r = Parent.Fragment.Clone();
                while (r.Start.iChar > 0)
                {
                    if (r.CharBeforeStart == '}')
                    {
                        enterPlace = r.Start;
                        return CompareResult.Visible;
                    }

                    r.GoLeftThroughFolded();
                }

                return CompareResult.Hidden;
            }

            public override string GetTextForReplace()
            {
                //extend range
                Range r = Parent.Fragment;
                Place end = r.End;
                r.Start = enterPlace;
                r.End = r.End;
                //insert line break
                return Environment.NewLine + r.Text;
            }

            public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
            {
                base.OnSelected(popupMenu, e);
                if (Parent.Fragment.tb.AutoIndent)
                    Parent.Fragment.tb.DoAutoIndent();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return "Insert line break after '}'";
                }
            }
        }

        private void autoIndentSelectedTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.DoAutoIndent();
        }

        private void btInvisibleChars_Click(object sender, EventArgs e)
        {
            HighlightInvisibleChars(fctbCode.Range);
            fctbCode.Invalidate();
        }

        private void btHighlightCurrentLine_Click(object sender, EventArgs e)
        {
            if (btHighlightCurrentLine.Checked)
                fctbCode.CurrentLineColor = currentLineColor;
            else
                fctbCode.CurrentLineColor = Color.Transparent;

            fctbCode.Invalidate();
        }

        private void commentSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.InsertLinePrefix("//");
        }

        private void uncommentSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.RemoveLinePrefix("//");
        }

        private void cloneLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //expand selection
            fctbCode.Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + fctbCode.Selection.Text;
            //move caret to end of selected lines
            fctbCode.Selection.Start = fctbCode.Selection.End;
            //insert text
            fctbCode.InsertText(text);
        }

        private void cloneLinesAndCommentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //start autoUndo block
            fctbCode.BeginAutoUndo();
            //expand selection
            fctbCode.Selection.Expand();
            //get text of selected lines
            string text = Environment.NewLine + fctbCode.Selection.Text;
            //comment lines
            fctbCode.InsertLinePrefix("//");
            //move caret to end of selected lines
            fctbCode.Selection.Start = fctbCode.Selection.End;
            //insert text
            fctbCode.InsertText(text);
            //end of autoUndo block
            fctbCode.EndAutoUndo();
        }

        private void bookmarkPlusButton_Click(object sender, EventArgs e)
        {
            if(fctbCode == null) 
                return;
            fctbCode.BookmarkLine(fctbCode.Selection.Start.iLine);
        }

        private void bookmarkMinusButton_Click(object sender, EventArgs e)
        {
            if (fctbCode == null)
                return;
            fctbCode.UnbookmarkLine(fctbCode.Selection.Start.iLine);
        }

        private void gotoButton_DropDownOpening(object sender, EventArgs e)
        {
            gotoButton.DropDownItems.Clear();
            
            foreach (var bookmark in fctbCode.Bookmarks)
            {
                var item = gotoButton.DropDownItems.Add(bookmark.Name + " [" + Path.GetFileNameWithoutExtension(fctbCode.Tag as String) + "]");
                item.Tag = bookmark;
                item.Click += (o, a) => {
                    var b = (Bookmark)(o as ToolStripItem).Tag;
                    try
                    {
                        fctbCode = b.TB;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    b.DoVisible();
                };
            }
        }

        private void btShowFoldingLines_Click(object sender, EventArgs e)
        {
            fctbCode.ShowFoldingLines = btShowFoldingLines.Checked;
            fctbCode.Invalidate();
        }

        private void Zoom_click(object sender, EventArgs e)
        {
            fctbCode.Zoom = int.Parse((sender as ToolStripItem).Tag.ToString());
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            NewTemplate();
        }

        private void NewTemplate()
        {
            template = new TemplateSettings
            {
                Id = Guid.NewGuid(),
                Name = Translations.Strings.Untitled,
                Enabled = true,
                PerEntity = true,
                FileName = "Eg{{ model.name }}{{ entity.name }}",
                Code = ""
            };

            BindData();

            int index = SaveTemplate();

            PopulateTemplates();

            toolStripComboBox1.ComboBox.SelectedIndex = index;
            
            toolStripTextBox1.Focus();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (ofdMain.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    fctbCode.OpenFile(ofdMain.FileName);
                    fctbCode.Focus();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int SaveTemplate()
        {
            template.Name = toolStripTextBox1.Text.Trim();
            template.Enabled = toolStripButton1.Checked;
            template.PerEntity = toolStripButton11.Checked;
            template.FileName = toolStripTextBox2.Text.Trim();
            template.Code = fctbCode.Text;

            return templates.SaveOrUpdateTemplate(template);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (template.Name != toolStripTextBox1.Text.Trim())
            {
                int index = SaveTemplate();

                PopulateTemplates();

                toolStripComboBox1.ComboBox.SelectedIndex = index;
            }
            else
            {
                SaveTemplate();
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var settings = new PrintDialogSettings();
            settings.Title = template.Name;
            settings.Header = "&b&w&b";
            settings.Footer = "&b&p";
            fctbCode.Print(settings);
        }

        private void toolStripButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(toolStripButton1.Checked)
            {
                toolStripButton1.Image = global::NClass.GUI.Properties.Resources.check;
            }
            else
            {
                toolStripButton1.Image = global::NClass.GUI.Properties.Resources.uncheck;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            template = (TemplateSettings)toolStripComboBox1.SelectedItem;

            BindData();

            fctbCode.Focus();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            templates.DeleteTemplate(template);

            toolStripTextBox1.Clear();
            toolStripTextBox2.Clear();
            fctbCode.Clear();

            PopulateTemplates();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            fctbCode.ShowFindDialog();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            fctbCode.ShowReplaceDialog();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            fctbCode.Zoom += 10;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            fctbCode.Zoom -= 10;
        }

        private void toolStripButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton11.Checked)
            {
                toolStripButton11.Image = global::NClass.GUI.Properties.Resources.check;
            }
            else
            {
                toolStripButton11.Image = global::NClass.GUI.Properties.Resources.uncheck;
            }
        }

        private void fctbCode_LostFocus(object sender, EventArgs e)
        {
            if (toolStripButton12.Checked)
            {
                SaveTemplate();
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Shopify/liquid/wiki/Liquid-for-Designers");
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            NewTemplate();
            string model = @"// /examples/shapes.ncp
{
  ""ProjectName"": ""Shapes"",
  ""Name"": ""Shapes"",
  ""AssemblyName"": ""Shapes"",
  ""RootNamespace"": ""Shapes"",
  ""Entities"": [
    {
      ""FieldsCount"": 2,
      ""Fields"": [
        {
          ""Type"": ""Color"",
          ""MemberType"": ""Field"",
          ""Access"": ""Private"",
          ""Name"": ""color""
        },
        {
          ""Type"": ""PointF"",
          ""MemberType"": ""Field"",
          ""Access"": ""Private"",
          ""Name"": ""location""
        }
      ],
      ""OperationsCount"": 4,
      ""Operations"": [
        {
          ""Type"": ""Color"",
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Color""
        },
        {
          ""Type"": ""PointF"",
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Location""
        },
        {
          ""MemberType"": ""Constructor"",
          ""Access"": ""Protected"",
          ""Name"": ""Shape""
        },
        {
          ""Type"": ""void"",
          ""MemberType"": ""Method"",
          ""Access"": ""Public"",
          ""Name"": ""Draw""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Shape"",
    },
    {
      ""FieldsCount"": 1,
      ""Fields"": [
        {
          ""Type"": ""float"",
          ""MemberType"": ""Field"",
          ""Access"": ""Private"",
          ""Name"": ""radius""
        }
      ],
      ""OperationsCount"": 3,
      ""Operations"": [
        {
          ""Type"": ""float"",
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Radius""
        },
        {
          ""MemberType"": ""Constructor"",
          ""Access"": ""Public"",
          ""Name"": ""Circle""
        },
        {
          ""Type"": ""void"",
          ""MemberType"": ""Method"",
          ""Access"": ""Public"",
          ""Name"": ""Draw""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Circle"",
    },
    {
      ""FieldsCount"": 1,
      ""Fields"": [
        {
          ""Type"": ""float"",
          ""MemberType"": ""Field"",
          ""Access"": ""Private"",
          ""Name"": ""size""
        }
      ],
      ""OperationsCount"": 3,
      ""Operations"": [
        {
          ""Type"": ""float"",
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Size""
        },
        {
          ""MemberType"": ""Constructor"",
          ""Access"": ""Public"",
          ""Name"": ""Square""
        },
        {
          ""Type"": ""void"",
          ""MemberType"": ""Method"",
          ""Access"": ""Public"",
          ""Name"": ""Draw""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Square"",
    },
    {
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 1,
      ""Operations"": [
        {
          ""Type"": ""void"",
          ""MemberType"": ""Method"",
          ""Access"": ""Public"",
          ""Name"": ""Draw""
        }
      ],
      ""EntityType"": ""Interface"",
      ""Access"": ""Public"",
      ""Name"": ""IDrawable"",
    }
  ],
  ""EntitiesNames"": [
    ""Shape"",
    ""Circle"",
    ""Square"",
    ""IDrawable""
  ],
  ""Relationships"": [
    {
      ""RelationshipType"": ""Generalization"",
      ""SupportsLabel"": false,
      ""Label"": """",
      ""FirstEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Circle"",
      },
      ""SecondEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Shape"",
      }
    },
    {
      ""RelationshipType"": ""Generalization"",
      ""SupportsLabel"": false,
      ""Label"": """",
      ""FirstEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Square"",
      },
      ""SecondEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Shape"",
      }
    },
    {
      ""RelationshipType"": ""Realization"",
      ""SupportsLabel"": false,
      ""Label"": """",
      ""FirstEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Shape"",
      },
      ""SecondEntity"": {
        ""EntityType"": ""Interface"",
        ""Name"": ""IDrawable"",
      }
    }
  ]
}
";
            fctbCode.Text = model;
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            NewTemplate();
            string entity = @"// /examples/shapes.ncp, Shape Entity
{
  ""ProjectName"": ""Shapes"",
  ""ModelName"": ""Shapes"",
  ""RootNamespace"": ""Shapes"",
  ""EntitiesNames"": [
    ""Shape"",
    ""Circle"",
    ""Square"",
    ""IDrawable""
  ],
  ""EntityType"": ""Class"",
  ""Access"": ""Public"",
  ""Name"": ""Shape"",
  ""FieldsCount"": 2,
  ""Fields"": [
    {
      ""Type"": ""Color"",
      ""MemberType"": ""Field"",
      ""Access"": ""Private"",
      ""Name"": ""color""
    },
    {
      ""Type"": ""PointF"",
      ""MemberType"": ""Field"",
      ""Access"": ""Private"",
      ""Name"": ""location""
    }
  ],
  ""OperationsCount"": 4,
  ""Operations"": [
    {
      ""Type"": ""Color"",
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Color""
    },
    {
      ""Type"": ""PointF"",
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Location""
    },
    {
      ""MemberType"": ""Constructor"",
      ""Access"": ""Protected"",
      ""Name"": ""Shape""
    },
    {
      ""Type"": ""void"",
      ""MemberType"": ""Method"",
      ""Access"": ""Public"",
      ""Name"": ""Draw""
    }
  ],
  ""Relationships"": [
    {
      ""RelationshipType"": ""Generalization"",
      ""SupportsLabel"": false,
      ""Label"": """",
      ""FirstEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Circle"",
      },
      ""SecondEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Shape"",
      }
    },
    {
      ""RelationshipType"": ""Generalization"",
      ""SupportsLabel"": false,
      ""Label"": """",
      ""FirstEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Square"",
      },
      ""SecondEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Shape"",
      }
    },
    {
      ""RelationshipType"": ""Realization"",
      ""SupportsLabel"": false,
      ""Label"": """",
      ""FirstEntity"": {
        ""EntityType"": ""Class"",
        ""Name"": ""Shape"",
      },
      ""SecondEntity"": {
        ""EntityType"": ""Interface"",
        ""Name"": ""IDrawable"",
      }
    }
  ]
}
";
            fctbCode.Text = entity;
        }
    }

    public class InvisibleCharsRenderer : Style
    {
        Pen pen;

        public InvisibleCharsRenderer(Pen pen)
        {
            this.pen = pen;
        }

        public override void Draw(Graphics gr, Point position, Range range)
        {
            var tb = range.tb;
            using(Brush brush = new SolidBrush(pen.Color))
            foreach (var place in range)
            {
                switch (tb[place].c)
                {
                    case ' ':
                        var point = tb.PlaceToPoint(place);
                        point.Offset(tb.CharWidth / 2, tb.CharHeight / 2);
                        gr.DrawLine(pen, point.X, point.Y, point.X + 1, point.Y);
                        break;
                }

                if (tb[place.iLine].Count - 1 == place.iChar)
                {
                    var point = tb.PlaceToPoint(place);
                    point.Offset(tb.CharWidth, 0);
                    gr.DrawString("¶", tb.Font, brush, point);
                }
            }
        }
    }

    public class TbInfo
    {
        public AutocompleteMenu popupMenu;
    }
}
