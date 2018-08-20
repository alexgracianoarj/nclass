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

        GroupTemplates group;
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

            PopulateGroups();
            PopulateTemplates();
        }

        private void PopulateGroups()
        {
            templates = TemplatesSettings.Load();
            toolStripComboBox2.ComboBox.DataSource = templates.Groups;
            toolStripComboBox2.ComboBox.DisplayMember = "Name";
        }

        private void PopulateTemplates()
        {
            if(templates.Groups.Count > 0)
            {
                group = (GroupTemplates)toolStripComboBox2.SelectedItem;
                toolStripComboBox1.ComboBox.DataSource = group.Templates;
                toolStripComboBox1.ComboBox.DisplayMember = "Name";
            }
            else
            {
                toolStripComboBox1.ComboBox.DataSource = new List<TemplateSettings>();
                toolStripComboBox1.ComboBox.DisplayMember = "Name";
            }
        }

        private void BindDataGroup()
        {
            toolStripTextBox3.Text = group.Name;
            toolStripButton13.Checked = group.Enabled;
        }

        private void BindDataTemplate()
        {
            toolStripTextBox1.Text = template.Name;
            toolStripButton1.Checked = template.Enabled;
            toolStripButton11.Checked = template.PerEntity;
            toolStripTextBox2.Text = template.FileExt;
            fctbCode.Text = template.Code;
            fctbCode.ClearStylesBuffer();
            fctbCode.Range.ClearStyle(StyleIndex.All);
            fctbCode.AddStyle(sameWordsStyle);
            fctbCode.Language = (Language)Enum.Parse(typeof(Language), template.Language);
            fctbCode.OnSyntaxHighlight(new TextChangedEventArgs(fctbCode.Range));
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
            undoStripButton.Enabled = undoToolStripMenuItem.Enabled = fctbCode.UndoEnabled;
            redoStripButton.Enabled = redoToolStripMenuItem.Enabled = fctbCode.RedoEnabled;
            cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
            copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = !fctbCode.Selection.IsEmpty;
            toolStripButton5.Enabled = !string.IsNullOrEmpty(fctbCode.Text);
        }
        
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.ShowFindDialog();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.ShowReplaceDialog();
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

        private void NewGroup()
        {
            group = new GroupTemplates
            {
                Id = Guid.NewGuid(),
                Name = Translations.Strings.Untitled,
                Enabled = true,
                Templates = new List<TemplateSettings>()
            };

            BindDataGroup();

            int index = SaveGroup();

            PopulateGroups();

            toolStripComboBox2.ComboBox.SelectedIndex = index;

            toolStripTextBox3.Focus();
        }

        private void NewTemplate()
        {
            if(templates.Groups.Count == 0)
            {
                NewGroup();
            }

            template = new TemplateSettings
            {
                Id = Guid.NewGuid(),
                Name = Translations.Strings.Untitled,
                Enabled = true,
                PerEntity = true,
                FileExt = "Eg{{ model.name }}{{ entity.name }}.cs",
                Code = string.Empty,
                Language = Language.CSharp.ToString()
            };

            BindDataTemplate();

            int indexT = SaveTemplate();
            int indexG = toolStripComboBox2.ComboBox.SelectedIndex;

            PopulateGroups();

            toolStripComboBox2.ComboBox.SelectedIndex = indexG;
            toolStripComboBox1.ComboBox.SelectedIndex = indexT;
            
            toolStripTextBox1.Focus();
        }

        private int SaveGroup()
        {
            group.Name = toolStripTextBox3.Text.Trim();
            group.Enabled = toolStripButton13.Checked;

            return templates.SaveGroup(group);
        }

        private int SaveTemplate()
        {
            template.Name = toolStripTextBox1.Text.Trim();
            template.Enabled = toolStripButton1.Checked;
            template.PerEntity = toolStripButton11.Checked;
            template.FileExt = toolStripTextBox2.Text.Trim();
            template.Code = fctbCode.Text;
            template.Language = fctbCode.Language.ToString();

            return templates.SaveTemplate(group, template);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if(templates.Groups.Count > 0)
            {
                if (template.Name != toolStripTextBox1.Text.Trim())
                {
                    int indexT = SaveTemplate();
                    int indexG = toolStripComboBox2.ComboBox.SelectedIndex;

                    PopulateGroups();

                    toolStripComboBox2.ComboBox.SelectedIndex = indexG;
                    toolStripComboBox1.ComboBox.SelectedIndex = indexT;
                }
                else
                {
                    SaveTemplate();
                }

                fctbCode.ClearUndo();
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

            BindDataTemplate();

            fctbCode.Focus();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if(templates.Groups.Count > 0)
            {
                templates.DeleteTemplate(group, template);

                int index = toolStripComboBox2.ComboBox.SelectedIndex;

                PopulateGroups();

                toolStripComboBox2.ComboBox.SelectedIndex = index;

                BindDataGroup();
            }
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

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Shopify/liquid/wiki/Liquid-for-Designers");
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            NewTemplate();
            string model = @"// /examples/Northwind.ncp
{
  ""ProjectName"": ""Northwind"",
  ""Name"": ""Northwind"",
  ""AssemblyName"": ""Northwind"",
  ""RootNamespace"": ""Northwind"",
  ""Entities"": [
    {
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Categories""
      ""NHMTableName"": ""Categories"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 4,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""CategoryID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CategoryId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CategoryName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CategoryName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Description"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Description""
        },
        {
          ""Type"": ""System.Byte[]"",
          ""NHMColumnName"": ""Picture"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Picture""
        }
      ],
    },
    {
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""CustomerCustomerDemo""
      ""NHMTableName"": ""CustomerCustomerDemo"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 2,
      ""Operations"": [
        {
          ""Type"": ""Customers"",
          ""NHMColumnName"": ""CustomerID"",
          ""Identity"": true,
          ""ManyToOne"": ""Customers"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CustomerId""
        },
        {
          ""Type"": ""CustomerDemographics"",
          ""NHMColumnName"": ""CustomerTypeID"",
          ""Identity"": true,
          ""ManyToOne"": ""CustomerDemographics"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CustomerTypeId""
        }
      ],
    },
    {
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Customers""
      ""NHMTableName"": ""Customers"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 11,
      ""Operations"": [
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CustomerID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CustomerId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CompanyName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CompanyName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ContactName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ContactName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ContactTitle"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ContactTitle""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Address"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Address""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""City"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""City""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Region"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Region""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""PostalCode"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""PostalCode""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Country"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Country""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Phone"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Phone""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Fax"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Fax""
        }
      ],
    },
    {
      ""NHMTableName"": ""CustomerDemographics"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 2,
      ""Operations"": [
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CustomerTypeID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CustomerTypeId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CustomerDesc"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CustomerDesc""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""CustomerDemographics""
    },
    {
      ""NHMTableName"": ""Employees"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 18,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""EmployeeID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""EmployeeId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""LastName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""LastName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""FirstName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""FirstName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Title"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Title""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""TitleOfCourtesy"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""TitleOfCourtesy""
        },
        {
          ""Type"": ""DateTime"",
          ""NHMColumnName"": ""BirthDate"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""BirthDate""
        },
        {
          ""Type"": ""DateTime"",
          ""NHMColumnName"": ""HireDate"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""HireDate""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Address"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Address""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""City"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""City""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Region"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Region""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""PostalCode"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""PostalCode""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Country"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Country""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""HomePhone"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""HomePhone""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Extension"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Extension""
        },
        {
          ""Type"": ""System.Byte[]"",
          ""NHMColumnName"": ""Photo"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Photo""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Notes"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Notes""
        },
        {
          ""Type"": ""Employees"",
          ""NHMColumnName"": ""ReportsTo"",
          ""Identity"": false,
          ""ManyToOne"": ""Employees"",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ReportsTo""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""PhotoPath"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""PhotoPath""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Employees""
    },
    {
      ""NHMTableName"": ""EmployeeTerritories"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 2,
      ""Operations"": [
        {
          ""Type"": ""Employees"",
          ""NHMColumnName"": ""EmployeeID"",
          ""Identity"": true,
          ""ManyToOne"": ""Employees"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""EmployeeId""
        },
        {
          ""Type"": ""Territories"",
          ""NHMColumnName"": ""TerritoryID"",
          ""Identity"": true,
          ""ManyToOne"": ""Territories"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""TerritoryId""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""EmployeeTerritories""
    },
    {
      ""NHMTableName"": ""Territories"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 3,
      ""Operations"": [
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""TerritoryID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""TerritoryId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""TerritoryDescription"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""TerritoryDescription""
        },
        {
          ""Type"": ""Region"",
          ""NHMColumnName"": ""RegionID"",
          ""Identity"": false,
          ""ManyToOne"": ""Region"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""RegionId""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Territories""
    },
    {
      ""NHMTableName"": ""Region"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 2,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""RegionID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""RegionId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""RegionDescription"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""RegionDescription""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Region""
    },
    {
      ""NHMTableName"": ""Order Details"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 5,
      ""Operations"": [
        {
          ""Type"": ""Orders"",
          ""NHMColumnName"": ""OrderID"",
          ""Identity"": true,
          ""ManyToOne"": ""Orders"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""OrderId""
        },
        {
          ""Type"": ""Products"",
          ""NHMColumnName"": ""ProductID"",
          ""Identity"": true,
          ""ManyToOne"": ""Products"",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ProductId""
        },
        {
          ""Type"": ""decimal"",
          ""NHMColumnName"": ""UnitPrice"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""UnitPrice""
        },
        {
          ""Type"": ""short"",
          ""NHMColumnName"": ""Quantity"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Quantity""
        },
        {
          ""Type"": ""float"",
          ""NHMColumnName"": ""Discount"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Discount""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""OrderDetails""
    },
    {
      ""NHMTableName"": ""Orders"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 14,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""OrderID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""OrderId""
        },
        {
          ""Type"": ""Customers"",
          ""NHMColumnName"": ""CustomerID"",
          ""Identity"": false,
          ""ManyToOne"": ""Customers"",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CustomerId""
        },
        {
          ""Type"": ""Employees"",
          ""NHMColumnName"": ""EmployeeID"",
          ""Identity"": false,
          ""ManyToOne"": ""Employees"",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""EmployeeId""
        },
        {
          ""Type"": ""DateTime"",
          ""NHMColumnName"": ""OrderDate"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""OrderDate""
        },
        {
          ""Type"": ""DateTime"",
          ""NHMColumnName"": ""RequiredDate"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""RequiredDate""
        },
        {
          ""Type"": ""DateTime"",
          ""NHMColumnName"": ""ShippedDate"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShippedDate""
        },
        {
          ""Type"": ""Shippers"",
          ""NHMColumnName"": ""ShipVia"",
          ""Identity"": false,
          ""ManyToOne"": ""Shippers"",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipVia""
        },
        {
          ""Type"": ""decimal"",
          ""NHMColumnName"": ""Freight"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Freight""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ShipName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ShipAddress"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipAddress""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ShipCity"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipCity""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ShipRegion"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipRegion""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ShipPostalCode"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipPostalCode""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ShipCountry"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipCountry""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Orders""
    },
    {
      ""NHMTableName"": ""Shippers"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 3,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""ShipperID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ShipperId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CompanyName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CompanyName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Phone"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Phone""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Shippers""
    },
    {
      ""NHMTableName"": ""Products"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 10,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""ProductID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ProductId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ProductName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ProductName""
        },
        {
          ""Type"": ""Suppliers"",
          ""NHMColumnName"": ""SupplierID"",
          ""Identity"": false,
          ""ManyToOne"": ""Suppliers"",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""SupplierId""
        },
        {
          ""Type"": ""Categories"",
          ""NHMColumnName"": ""CategoryID"",
          ""Identity"": false,
          ""ManyToOne"": ""Categories"",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CategoryId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""QuantityPerUnit"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""QuantityPerUnit""
        },
        {
          ""Type"": ""decimal"",
          ""NHMColumnName"": ""UnitPrice"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""UnitPrice""
        },
        {
          ""Type"": ""short"",
          ""NHMColumnName"": ""UnitsInStock"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""UnitsInStock""
        },
        {
          ""Type"": ""short"",
          ""NHMColumnName"": ""UnitsOnOrder"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""UnitsOnOrder""
        },
        {
          ""Type"": ""short"",
          ""NHMColumnName"": ""ReorderLevel"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ReorderLevel""
        },
        {
          ""Type"": ""bool"",
          ""NHMColumnName"": ""Discontinued"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Discontinued""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Products""
    },
    {
      ""NHMTableName"": ""Suppliers"",
      ""IdentityGenerator"": ""assigned"",
      ""FieldsCount"": 0,
      ""Fields"": [],
      ""OperationsCount"": 12,
      ""Operations"": [
        {
          ""Type"": ""int"",
          ""NHMColumnName"": ""SupplierID"",
          ""Identity"": true,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""SupplierId""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""CompanyName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": true,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""CompanyName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ContactName"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ContactName""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""ContactTitle"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""ContactTitle""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Address"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Address""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""City"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""City""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Region"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Region""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""PostalCode"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""PostalCode""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Country"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Country""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Phone"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Phone""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""Fax"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""Fax""
        },
        {
          ""Type"": ""string"",
          ""NHMColumnName"": ""HomePage"",
          ""Identity"": false,
          ""ManyToOne"": """",
          ""Unique"": false,
          ""NotNull"": false,
          ""MemberType"": ""Property"",
          ""Access"": ""Public"",
          ""Name"": ""HomePage""
        }
      ],
      ""EntityType"": ""Class"",
      ""Access"": ""Public"",
      ""Name"": ""Suppliers""
    }
  ]
}
";
            fctbCode.Text = model;
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            NewTemplate();
            string entity = @"// /examples/Northwind.ncp, Suppliers Entity
{
  ""ProjectName"": ""Northwind"",
  ""ModelName"": ""Northwind"",
  ""AssemblyName"": ""Northwind"",
  ""RootNamespace"": ""Northwind"",
  ""EntityType"": ""Class"",
  ""Access"": ""Public"",
  ""Name"": ""Suppliers""
  ""NHMTableName"": ""Suppliers"",
  ""IdentityGenerator"": ""assigned"",
  ""FieldsCount"": 0,
  ""Fields"": [],
  ""OperationsCount"": 12,
  ""Operations"": [
    {
      ""Type"": ""int"",
      ""NHMColumnName"": ""SupplierID"",
      ""Identity"": true,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": true,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""SupplierId""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""CompanyName"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": true,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""CompanyName""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""ContactName"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""ContactName""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""ContactTitle"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""ContactTitle""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""Address"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Address""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""City"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""City""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""Region"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Region""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""PostalCode"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""PostalCode""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""Country"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Country""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""Phone"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Phone""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""Fax"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""Fax""
    },
    {
      ""Type"": ""string"",
      ""NHMColumnName"": ""HomePage"",
      ""Identity"": false,
      ""ManyToOne"": """",
      ""Unique"": false,
      ""NotNull"": false,
      ""MemberType"": ""Property"",
      ""Access"": ""Public"",
      ""Name"": ""HomePage""
    }
  ],
}
";
            fctbCode.Text = entity;
        }

        private void cSharpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.ClearStylesBuffer();
            fctbCode.Range.ClearStyle(StyleIndex.All);
            fctbCode.AddStyle(sameWordsStyle);
            fctbCode.Language = Language.CSharp;
            fctbCode.OnSyntaxHighlight(new TextChangedEventArgs(fctbCode.Range));
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctbCode.ClearStylesBuffer();
            fctbCode.Range.ClearStyle(StyleIndex.All);
            fctbCode.AddStyle(sameWordsStyle);
            fctbCode.Language = Language.XML;
            fctbCode.OnSyntaxHighlight(new TextChangedEventArgs(fctbCode.Range));
        }

        private void menuLanguage_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mi in menuLanguage.DropDownItems)
                mi.Checked = mi.Text == fctbCode.Language.ToString();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            NewGroup();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            if(templates.Groups.Count > 0)
            {
                if (group.Name != toolStripTextBox3.Text.Trim())
                {
                    int indexG = SaveGroup();
                    int indexT = toolStripComboBox1.ComboBox.SelectedIndex;

                    PopulateGroups();

                    toolStripComboBox2.ComboBox.SelectedIndex = indexG;
                    toolStripComboBox1.ComboBox.SelectedIndex = indexT;
                }
                else
                {
                    SaveGroup();
                }
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            templates.DeleteGroup(group);

            group = new GroupTemplates
            {
                Enabled = false
            };

            PopulateGroups();

            BindDataGroup();

            if(group.Templates.Count == 0)
            {
                template = new TemplateSettings
                {
                    Enabled = false,
                    PerEntity = false,
                    Code = string.Empty,
                    Language = Language.CSharp.ToString()
                };

                PopulateTemplates();

                BindDataTemplate();
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            group = (GroupTemplates)toolStripComboBox2.SelectedItem;

            BindDataGroup();

            if(group.Templates.Count == 0)
            {
                template = new TemplateSettings
                {
                    Enabled = false,
                    PerEntity = false,
                    Code = string.Empty,
                    Language = Language.CSharp.ToString()
                };
            }

            PopulateTemplates();

            BindDataTemplate();
        }

        private void toolStripButton13_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton13.Checked)
            {
                toolStripButton13.Image = global::NClass.GUI.Properties.Resources.check;
            }
            else
            {
                toolStripButton13.Image = global::NClass.GUI.Properties.Resources.uncheck;
            }
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            ofdMain.Filter = "C# File|*.cs|XML File|*.xml|All Files|*.*";

            if (ofdMain.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    fctbCode.OpenFile(ofdMain.FileName);
                    fctbCode.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
