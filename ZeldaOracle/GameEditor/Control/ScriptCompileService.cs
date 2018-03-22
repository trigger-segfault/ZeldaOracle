using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Control {

	public delegate void CompileCompletedCallback(ScriptCompileResult result);

	public class CompileTask {

		public Task<ScriptCompileResult> Task { get; set; }
		public Thread Thread { get; set; }
		public ScriptCompileResult Result { get; set; }
		public GeneratedScriptCode Code { get; set; }
		public bool IsCancelled { get; set; }
		private event CompileCompletedCallback completed;
		private event Action cancelled;
		
		public CompileTask(GeneratedScriptCode code) {
			Code = code;
			Result = null;
			Thread = null;
			Task = null;
			completed = null;
			cancelled = null;
		}

		public void Cancel() {
			if (Task != null & Thread != null && !Task.IsCompleted)
				Thread.Abort();
			Task = null;
			Thread = null;
			Result = null;
			cancelled?.Invoke();
		}

		public void Complete() {
			Result = Task.Result;
			Task = null;
			Thread = null;
			completed?.Invoke(Result);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public event CompileCompletedCallback Completed {
			add { completed += value; }
			remove { completed -= value; }
		}

		public event Action Cancelled {
			add { cancelled += value; }
			remove { cancelled -= value; }
		}
	}

	/// <summary>
	/// Service class responsible for compiling scripts
	/// </summary>
	public class ScriptCompileService {
		
		//public delegate void ScriptCompileCallback(ScriptCompileResult result);
		
		//private List<ScriptStart>			scriptStarts;
		//private HashSet<string>				scriptsToRecompile;
		//private HashSet<Event>				eventsToRecompile;
		//private bool                        isCompilingForScriptEditor;
		//private Task<List<Event>>           eventCacheTask;
		//private List<Event>                 eventCache;
		//private bool                        needsNewEventCache;
		private List<CompileTask>			compileTasks;
		//private Thread						compileThread;
		//private Script                      currentCompilingScript;
		//private ScriptCompileResult			compileResult;
		//private event CompileEventHandler	compiled;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ScriptCompileService() {
			//scriptsToRecompile		= new HashSet<string>();
			//eventsToRecompile		= new HashSet<Event>();
			//isCompilingForScriptEditor = false;
			//eventCache				= new List<Event>();
			//eventCacheTask			= null;
			//needsNewEventCache		= false;
			//scriptStarts			= new List<ScriptStart>();
			//currentCompilingScript	= null;
			//compileResult			= null;
			//compiled				= null;
			compileTasks			= new List<CompileTask>();
		}
		
		
		//-----------------------------------------------------------------------------
		// Script Compiling
		//-----------------------------------------------------------------------------

		public void UpdateScriptCompiling() {
			// Check if any compile task have completed
			int taskCount = compileTasks.Count;
			for (int i = 0; i < taskCount; i++) {
				CompileTask task = compileTasks[i];
				if (task.IsCancelled) {
					compileTasks.RemoveAt(i);
				}
				else if (task.Task != null &&  task.Task.IsCompleted) {
					compileTasks.RemoveAt(i);
					task.Complete();
				}
			}
		}


		/// <summary>Begin compiling the scripts in a background task.</summary>
		public CompileTask CompileAllScripts(World world) {
			Logs.Scripts.LogNotice("Compiling all scripts...");
			// Generate the code first
			ScriptCodeGenerator codeGenerator = new ScriptCodeGenerator(world);
			GeneratedScriptCode code = codeGenerator.GenerateCode(true);
			return BeginCompileTask(code);
		}

		/// <summary>Compile a single script in order to check for errors/warnings.
		/// </summary>
		public CompileTask CompileSingleScript(Script script, World world) {
			Logs.Scripts.LogInfo("Compiling script {0}...", script.ID);
			ScriptCodeGenerator codeGenerator = new ScriptCodeGenerator(world);
			GeneratedScriptCode code =
				codeGenerator.GenerateTestCode(script, script.Code);
			return BeginCompileTask(code);
		}

		/// <summary>Compile a single trgiger script in order to check for
		/// errors/warnings.</summary>
		public CompileTask CompileSingleTrigger(Trigger trigger, World world) {
			Logs.Scripts.LogInfo("Compiling trigger {0}...", trigger.Name);
			ScriptCodeGenerator codeGenerator = new ScriptCodeGenerator(world);
			GeneratedScriptCode code =
				codeGenerator.GenerateTestCode(trigger, trigger.Script.Code);
			return BeginCompileTask(code);
		}

		/// <summary>Cancel all compile tasks that are running.</summary>
		public void CancelAllTasks() {
			int taskCount = compileTasks.Count;
			for (int i = 0; i < taskCount; i++)
				compileTasks[i].Cancel();
			compileTasks.Clear();
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private CompileTask BeginCompileTask(GeneratedScriptCode code) {
			return BeginCompileTask(new CompileTask(code));
		}

		private CompileTask BeginCompileTask(CompileTask task) {
			task.Result = null;
			task.Thread = null;
			compileTasks.Add(task);

			task.Task = Task.Run(() => {
				task.Thread = Thread.CurrentThread;
				ScriptCompiler compiler = new ScriptCompiler();
				try {
					return compiler.Compile(task.Code.Code);
				}
				catch (ThreadAbortException) {
					return null;
				}
			});

			return task;
		}

		/// <summary>Compile the specified code.</summary>
		//private static ScriptCompileResult Compile(string code,
		//	bool generateAssembly = true, int firstLineStart = 0)
		//{
		//	ScriptCompileResult result = new ScriptCompileResult();
		//	string pathToAssembly = "";
		//	bool hasErrors = false;
			
		//	// Setup the compile options
		//	CompilerParameters options = new CompilerParameters();
		//	options.GenerateExecutable = false;
		//	options.GenerateInMemory = !generateAssembly;
		//	options.OutputAssembly =
		//		Path.GetFileNameWithoutExtension(
		//			Assembly.GetEntryAssembly().Location) + "Scripts.dll";

		//	// Add the assembly references
		//	options.ReferencedAssemblies.Add(Assemblies.ZeldaCommon.Location);
		//	options.ReferencedAssemblies.Add(Assemblies.ZeldaAPI.Location);

		//	// Create a C# code provider and compile the code.
		//	// The 'using' statement is necessary so the created DLL file isn't
		//	// locked when we try to load its contents.
		//	using (CSharpCodeProvider csProvider = new CSharpCodeProvider()) {
		//		CompilerResults csResult = csProvider.CompileAssemblyFromSource(options, code);
		//		pathToAssembly = csResult.PathToAssembly;
		//		hasErrors = csResult.Errors.HasErrors;
				
		//		// Disable System.IO access
		//		int index = code.IndexOf("System.IO");
		//		if (index != -1) {
		//			int line = code.Take(index).Count(c => c == '\n') + 1;
		//			int column = 0;
		//			for (int i = index; i >= 0; i--, column++) {
		//				if (code[i] == '\n')
		//					break;
		//			}
		//			if (line == 0)
		//				column -= firstLineStart;
		//			result.Errors.Add(new ScriptCompileError(
		//				line, column, "", "System.IO is not allowed", false));
		//		}

		//		// Copy warnings and errors into the compile result
		//		foreach (CompilerError csError in csResult.Errors) {
		//			if (csError.Line == 0)
		//				csError.Column -= firstLineStart;
		//			ScriptCompileError error = new ScriptCompileError(
		//				csError.Line, csError.Column, csError.ErrorNumber,
		//				csError.ErrorText, csError.IsWarning);
		//			if (error.IsWarning)
		//				result.Warnings.Add(error);
		//			else
		//				result.Errors.Add(error);
		//		}
		//	}

		//	// If the compile was successful, then load the DLL's file contents and
		//	// remove the DLL file.
		//	if (!hasErrors && generateAssembly) {
		//		result.RawAssembly = File.ReadAllBytes(pathToAssembly);
		//		try {
		//			File.Delete(pathToAssembly);
		//		} catch { }
		//	}

		//	return result;
		//}

		/// <summary>Compile a script in a background task.</summary>
		//public void CompileScriptAsync(Script script,
		//	ScriptCompileCallback callback, bool scriptEditor)
		//{
		//	if (!CancelCompilation(scriptEditor))
		//		return;

		//	compileCallback = callback;
		//	compileTask = Task.Run(delegate() {
		//		compileThread = Thread.CurrentThread;
		//		try {
		//			int scriptStart;
		//			string code = world.ScriptManager.CreateTestScriptCode(
		//				script, script.Code, out scriptStart);
		//			return world.ScriptManager.Compile(code, false, scriptStart);
		//		}
		//		catch (ThreadAbortException) {
		//			return null;
		//		}
		//	});
		//	if (!scriptEditor)
		//		editorWindow.SetStatusBarTask("Compiling scripts...");
		//}

		//public void ScriptRenamed(string oldName, string newName) {
		//	CancelCompilation();

		//	if (scriptsToRecompile != null && oldName != null && newName != null && scriptsToRecompile.Contains(oldName)) {
		//		scriptsToRecompile.Remove(oldName);
		//		scriptsToRecompile.Add(newName);
		//	}
		//	foreach (Script script in world.ScriptManager.Scripts.Values) {
		//		if (!scriptsToRecompile.Contains(script.ID)) {
		//			if (ScriptCallsScript(script, oldName) || ScriptCallsScript(script, newName)) {
		//				scriptsToRecompile.Add(script.ID);
		//			}
		//		}
		//	}
		//	foreach (Event evnt in world.GetDefinedEvents()) {
		//		if (evnt.GetExistingScript(world.ScriptManager.Scripts) == null) {
		//			Script script = evnt.Script;
		//			if (!eventsToRecompile.Contains(evnt)) {
		//				if (ScriptCallsScript(script, oldName) || ScriptCallsScript(script, newName)) {
		//					eventsToRecompile.Add(evnt);
		//				}
		//			}
		//		}
		//	}
		//	if (HasScriptsToCheck)
		//		needsRecompiling = true;
		//}

		//private void CheckAllScriptsIndividually() {
		//	foreach (Script script in world.ScriptManager.Scripts.Values) {
		//		if (!scriptsToRecompile.Contains(script.ID)) {
		//			scriptsToRecompile.Add(script.ID);
		//		}
		//	}
		//	foreach (Event evnt in world.GetDefinedEvents()) {
		//		if (evnt.GetExistingScript(world.ScriptManager.Scripts) == null) {
		//			Script script = evnt.Script;
		//			if (!eventsToRecompile.Contains(evnt)) {
		//				eventsToRecompile.Add(evnt);
		//			}
		//		}
		//	}
		//}

		//private void OnCompileScriptCompleted(ScriptCompileResult result) {
		//	compileTask = null;
		//	compileThread = null;

		//	currentCompilingScript.Errors   = result.Errors;
		//	currentCompilingScript.Warnings = result.Warnings;
		//	currentCompilingScript = null;

		//	if (!HasScriptsToCheck) {
		//		editorWindow.ClearStatusBarTask();
		//		editorWindow.WorldTreeView.RefreshScripts(true, true);
		//	}
		//}

		//private void CompileNextScript() {
		//	Script script = null;
		//	if (scriptsToRecompile.Any()) {
		//		string first = scriptsToRecompile.First();
		//		script = world.GetScript(first);
		//		scriptsToRecompile.Remove(first);
		//	}
		//	else if (eventsToRecompile.Any()) {
		//		Event first = eventsToRecompile.First();
		//		script = eventsToRecompile.First().Script;
		//		eventsToRecompile.Remove(first);
		//	}
		//	if (script != null) {
		//		CompileScriptAsync(script, OnCompileScriptCompleted, false);
		//		currentCompilingScript = script;
		//	}
		//}

		//private bool ScriptCallsScript(Script script, string scriptName) {
		//	if (scriptName == null)
		//		return false;
		//	int index = script.Code.IndexOf(scriptName + "(");
		//	if (index != -1) {
		//		if (index != 0) {
		//			char c = script.Code[index - 1];
		//			if (!char.IsLetterOrDigit(c) && c != '_')
		//				return true;
		//		}
		//		else {
		//			return true;
		//		}
		//	}
		//	return false;
		//}

		//private bool HasScriptsToCheck {
		//	get { return scriptsToRecompile.Any() || eventsToRecompile.Any(); }
		//}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if there are any compile tasks running.</summary>
		public bool IsCompiling {
			get { return (compileTasks.Count > 0); }
		}
	}
}
