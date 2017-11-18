using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using FastColoredTextBoxNS;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Geometry;

namespace ZeldaEditor.Scripting {
		
	//-----------------------------------------------------------------------------
	// Script Editor Form
	//-----------------------------------------------------------------------------
	public partial class ScriptEditor : Form {

		private EditorControl				editorControl;
		private Script						script;				// The script that's being edited.
		private FastColoredTextBox			codeEditor;			// The code editor control.
        private AutocompleteMenu			autoCompleteMenu;	// The auto-complete menu.
		private Task<ScriptCompileResult>	compileTask;		// The async task that compiles the code as it changes.
		private bool						needsRecompiling;	// Has the code changed and needs to be recompiled?
		private ScriptCompileError			displayedError;
		//private System.Timers.Timer			timer;
		private String						previousName;		// The name of the script when the editor was opened.
		private String						previousCode;		// The code of the script when the editor was opened.
		private bool						autoCompile;
		private bool						compileOnClose;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptEditor(Script script, EditorControl editorControl) {
			InitializeComponent();

			this.script				= script;
			this.editorControl		= editorControl;
			this.previousName		= script.ID;
			this.previousCode		= script.Code;
			this.autoCompile		= true;
			this.compileOnClose		= true;
			this.needsRecompiling	= false;
			this.compileTask		= null;

			if (script.HasErrors) {
				displayedError = script.Errors[0];
				labelErrorMessage.Text	= displayedError.ToString();
				labelErrorMessage.Image	= ZeldaEditor.ResourceProperties.Resources.exclamation;
			}
			else {
				displayedError = null;
				labelErrorMessage.Text	= "";
				labelErrorMessage.Image	= null;
			}

			// Set some UI text.
			base.Text				= "Script Editor: " + script.ID;
			textBoxName.Text		= script.ID;
			
			// Create the code editor text box.
			codeEditor = new FastColoredTextBox();
			panelCode.Controls.Add(codeEditor);
			codeEditor.Dock			= DockStyle.Fill;
			codeEditor.Language		= Language.CSharp;
			codeEditor.Text			= script.Code;
			codeEditor.ReservedCountOfLineNumberChars = 4;
			codeEditor.IsChanged = false;
			codeEditor.ClearUndo();
			codeEditor.TextChanged			+= codeEditor_TextChanged;
			codeEditor.SelectionChanged		+= codeEditor_SelectionChanged;
			codeEditor.UndoRedoStateChanged	+= UpdateUndoRedoButtonStates;

            // Create the auto-complete menu.
            autoCompleteMenu = new AutocompleteMenu(codeEditor);
            autoCompleteMenu.ForeColor		= Color.Black;
            autoCompleteMenu.BackColor		= Color.White;
            autoCompleteMenu.SelectedColor	= Color.Orange;
            autoCompleteMenu.SearchPattern	= @"[\w\.]";
            autoCompleteMenu.AllowTabKey	= true;
            autoCompleteMenu.Items.SetAutocompleteItems(new DynamicCollection(autoCompleteMenu, script, codeEditor));
			
			// Start a timer to auto-compile the script every 2 seconds.
			if (autoCompile) {
				/*
				timer = new System.Timers.Timer(2000);
				timer.AutoReset = true;
				timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e) {

					//if (needsRecompiling && compileTask == null) {
					if (needsRecompiling && !editorControl.IsBusyCompiling) {
						//BeginCompilingScript();
						editorControl.CompileScript(script, OnCompileComplete);
						editorControl.NeedsRecompiling = true;
					}
				};
				timer.Start();
				*/
				// Add an idle method to check for compile task completion.
				Application.Idle += RecompileUpdate;
			}
			
			// Setup the initial undo/redo button states.
			UpdateUndoRedoButtonStates(this, EventArgs.Empty);
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Begin compiling the script asyncronously.
		private void BeginCompilingScript() {
			script.Code = codeEditor.Text;

			editorControl.CompileScript(script, OnCompileComplete);

			//compileTask = ScriptEditorCompiler.CompileScriptAsync(script);
			needsRecompiling = false;
		}

		// Check when the compiling has finished.
		private void RecompileUpdate(object sender, EventArgs e) {
			// Check if we have finished compiling.
			if (IsCompiling) {
				if (compileTask.IsCompleted) {
					ScriptCompileResult results = compileTask.Result;
					compileTask = null;
					OnCompileComplete(results);
				}
			}
			// Begin recompiling.
			else if (needsRecompiling && !editorControl.IsBusyCompiling) {
				BeginCompilingScript();
			}
			// Check if the form has been closed.
			else if (IsDisposed) {
				Application.Idle -= RecompileUpdate; // <--- Concurrency error much??
			}
		}

		// Called once an asyncronous compiling task has completed.
		private void OnCompileComplete(ScriptCompileResult results) {
			// Update the script object.
			script.Code				= codeEditor.Text;
			script.Errors			= results.Errors;
			script.Warnings			= results.Warnings;

			// Update the error message status-strip.
			if (script.HasErrors) {
				displayedError = script.Errors[0];
				labelErrorMessage.Text	= displayedError.ToString();
				labelErrorMessage.Image	= ZeldaEditor.ResourceProperties.Resources.exclamation;
			}
			else {
				displayedError = null;
				labelErrorMessage.Text	= "";
				labelErrorMessage.Image	= null;
			}

			// Update the world-tree-view's scripts (if there is an error icon for a failed compile).
			if (!script.IsHidden)
				editorControl.EditorForm.worldTreeView.RefreshScripts();
		}


		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------

		// Done.
		private void buttonDone_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			script.Code = codeEditor.Text;
			Close();
		}

		//-----------------------------------------------------------------------------

		// Undo.
		private void buttonUndo_Click(object sender, EventArgs e) {
			codeEditor.Undo();
		}

		// Redo.
		private void buttonRedo_Click(object sender, EventArgs e) {
			codeEditor.Redo();
		}

		void UpdateUndoRedoButtonStates(object sender, EventArgs e) {
				buttonUndo.Enabled = codeEditor.UndoEnabled;
				buttonRedo.Enabled = codeEditor.RedoEnabled;
		}


		//-----------------------------------------------------------------------------

		// Cut.
		private void buttonCut_Click(object sender, EventArgs e) {
			codeEditor.Cut();
		}
		
		// Copy.
		private void buttonCopy_Click(object sender, EventArgs e) {
			codeEditor.Copy();
		}

		// Paste.
		private void buttonPaste_Click(object sender, EventArgs e) {
			codeEditor.Paste();
		}
		
		//-----------------------------------------------------------------------------

		// Script Name.
		private void textBoxName_TextChanged(object sender, EventArgs e) {
			script.ID = textBoxName.Text;
			Text = "Script Editor: " + script.ID; // Update form caption.
		}

		//-----------------------------------------------------------------------------

		// When the editor is closed, prompt to save changes to the code.
		private void ScriptEditor_FormClosing(object sender, FormClosingEventArgs e) {
			if (DialogResult != DialogResult.OK && codeEditor.IsChanged) {
				// Prompt whether to save changes.
				DialogResult result = MessageBox.Show("Save changes to the code?", "Warning",
					MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
				
				if (result == DialogResult.Yes) {
					DialogResult = DialogResult.OK;
					script.Code = codeEditor.Text;
				}
				else if (result == DialogResult.No) {
					DialogResult = DialogResult.Cancel;
					script.ID = previousName;
					script.Code = previousCode;
				}
				else {
					e.Cancel = true;
				}
			}

			if (DialogResult == DialogResult.OK && MadeChanges) {
				editorControl.NeedsRecompiling = true;
				//ScriptCompileResult result = editorControl.World.ScriptManager.CompileScripts();
				//Console.WriteLine("Compiled scripts with " + result.Errors.Count + " errors and " + result.Warnings.Count + " warnings.");
			}
		}

		// When the form is closed, compile it if it needs to be recompiled.
		private void ScriptEditor_FormClosed(object sender, FormClosedEventArgs e) {
			if (autoCompile || compileOnClose) {
				//timer.Stop();
				if (needsRecompiling && editorControl.IsBusyCompiling) {
				//if (needsRecompiling && compileTask == null) {
					BeginCompilingScript();
				}
					//BeginCompilingScript();
			}
		}

		// When the code changes, flag the script to be recomplied.
		void codeEditor_TextChanged(object sender, TextChangedEventArgs e) {
			needsRecompiling = true;
		}
		
		// When the selection changes, update status-strip text cursor position labels.
		void codeEditor_SelectionChanged(object sender, EventArgs e) {
			Place cursor = codeEditor.Selection.Start;
			labelLineNumber.Text	= "Line " + (cursor.iLine + 1);
			labelColumnNumber.Text	= "Col " + (cursor.iChar + 1);
		}

		// When clicking on the error message, move the cursor to the line/column of the error.
		private void statusStripErrorMessage_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			if (displayedError != null && displayedError.Line > 0 && displayedError.Column > 0) {
				int lineIndex = GMath.Clamp(displayedError.Line - 1, 0, codeEditor.Lines.Count - 1);
				int charIndex = GMath.Clamp(displayedError.Column - 1, 0, codeEditor.Lines[lineIndex].Length );
				codeEditor.Selection.Start = new Place(charIndex, lineIndex);
				codeEditor.DoRangeVisible(codeEditor.Selection, true);
				codeEditor.Invalidate();
				codeEditor.Focus();
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsCompiling {
			get { return (compileTask != null); }
		}

		public Script Script {
			get { return script; }
			set { script = value; }
		}

		public string CodeText {
			get { return codeEditor.Text; }
			set { codeEditor.Text = value; }
		}

		public bool MadeChanges {
			get { return codeEditor.IsChanged; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}
	}
	

	//-----------------------------------------------------------------------------
	// Dynamic Collection - Used to fill the autocomplete popup menu with items.
	//-----------------------------------------------------------------------------
    internal class DynamicCollection : IEnumerable<AutocompleteItem> {

        private AutocompleteMenu menu;
        private FastColoredTextBox tb;
		private Script script;

        public DynamicCollection(AutocompleteMenu menu, Script script, FastColoredTextBox tb) {
            this.menu = menu;
			this.script = script;
            this.tb = tb;
        }

        public IEnumerator<AutocompleteItem> GetEnumerator() {
            // Get current fragment of the text
            string text = menu.Fragment.Text;

            // Extract class name (part before dot)
            string[] parts = text.Split('.');
            if (parts.Length < 2)
                yield break;
            string className = parts[parts.Length - 2];

			bool isStatic = true;

            // Find type for given className.
            Type type = FindTypeByName(className);
			if (className == "room") {
				type = typeof(ZeldaAPI.Room);
				isStatic = false;
			}
			else if (className == "game") {
				type = typeof(ZeldaAPI.Game);
				isStatic = false;
			}
			else {
				foreach (ScriptParameter param in script.Parameters) {
					if (className == param.Name) {
						type = FindTypeByName(param.Type);
						isStatic = false;
						break;
					}
				}
			}

            if (type == null)
                yield break;
			
			if (type.IsEnum) {
				if (!isStatic)
					yield break;

				foreach(string enumName in type.GetEnumNames()) {
					yield return new MethodAutocompleteItem(enumName)
					{
						ToolTipTitle = enumName,
						ToolTipText = "Description of enum name " + type.Name + "." + enumName + " goes here.",
					};
				}
			}

			IEnumerable<MethodInfo> methods;
			
			if (!type.IsEnum)
				methods = type.GetMethods().Where(m => !typeof(object).GetMethods().Select(me => me.Name).Contains(m.Name));
			else
				methods = type.GetMethods().Where(m => !typeof(Enum).GetMethods().Select(me => me.Name).Contains(m.Name));
			
			foreach (MethodInfo methodInfo in methods) {
				if (methodInfo.Name.StartsWith("set_") || methodInfo.Name.StartsWith("get_"))
					continue;

				// Return type.
				string returnTypeName = methodInfo.ReturnType.Name;
				if (methodInfo.ReturnType == typeof(void))
					returnTypeName = "void";

				string methodName = returnTypeName + " " + methodInfo.Name + "(";

				// Parameters.
				ParameterInfo[] parameters = methodInfo.GetParameters();
				for (int i = 0; i < parameters.Length; i++) {
					if (i > 0)
						methodName += ", ";
					methodName += parameters[i].ParameterType.Name + " " + parameters[i].Name;
				}
				methodName += ")";

				// Description.
				string description = "No description available.";
				ZeldaAPI.Attributes.Description descriptionAttribute = methodInfo.GetCustomAttribute<ZeldaAPI.Attributes.Description>();
				if (descriptionAttribute != null)
					description = descriptionAttribute.Text;
				
				yield return new MethodAutocompleteItem(methodInfo.Name)
				{
                    ToolTipTitle = methodName,
                    ToolTipText = description,
                };
			}
			
            /*foreach (var methodName in type.GetMethods().AsEnumerable().Select(mi => mi.Name).Distinct()) {
                yield return new MethodAutocompleteItem(methodName + "()")
				{
                    ToolTipTitle = methodName,
                    ToolTipText = "Description of method " + methodName + " goes here.",
                };
			}*/
			
            // Return static properties of the class
            foreach (PropertyInfo pi in type.GetProperties()) {
                yield return new MethodAutocompleteItem(pi.Name)
                {
                    ToolTipTitle = pi.Name,
                    ToolTipText = "Description of property " + pi.Name + " goes here.",
                };
			}
        }

        private Type FindTypeByName(string name) {
			Assembly[] assemblies = { Assembly.GetAssembly(typeof(ZeldaAPI.Room)) };
			//Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                foreach (Type type in assembly.GetTypes()) {
                    if (type.Name == name)
                        return type;
				}
            }
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
