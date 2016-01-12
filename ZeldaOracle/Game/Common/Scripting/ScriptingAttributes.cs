using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Scripting {

/*
static void TestScripting(string code) {
	string code = 
		"using ZeldaOracle.Game.Entities.Monsters;" +
        "namespace SimpleScripts" +
        "{" +
        "    public class MyScriptMul5 : ScriptingInterface.IScriptType1" +
        "    {" +
        "        public string RunScript(int value)" +
        "        {" +
        "            return this.ToString() + \" just ran! Result: \" + (value*5).ToString();" +
        "        }" +
        "    }" +
        "    public class MyScriptNegate : ScriptingInterface.IScriptType1" +
        "    {" +
        "        public string RunScript(int value)" +
        "        {" +
		"            TestMonster monster = new TestMonster();" +
		"            monster.Health = 5;" +
        "            return this.ToString() + \" just ran! Result: \" + (-value).ToString();" +
        "        }" +
        "    }" +
        "}";

	Assembly compiledScript = CompileCode(code);

	if (compiledScript != null) {
        RunScript(compiledScript);
    }
}

static Assembly CompileCode(string code) {
    // Create a code provider
    // This class implements the 'CodeDomProvider' class as its base. All of the current .Net languages (at least Microsoft ones)
    // come with thier own implemtation, thus you can allow the user to use the language of thier choice (though i recommend that
    // you don't allow the use of c++, which is too volatile for scripting use - memory leaks anyone?)
    Microsoft.CSharp.CSharpCodeProvider csProvider = new Microsoft.CSharp.CSharpCodeProvider();

    // Setup our options
    CompilerParameters options	= new CompilerParameters();
    options.GenerateExecutable	= false; // we want a Dll (or "Class Library" as its called in .Net)
    options.GenerateInMemory	= true; // Saves us from deleting the Dll when we are done with it, though you could set this to false and save start-up time by next time by not having to re-compile
    // And set any others you want, there a quite a few, take some time to look through them all and decide which fit your application best!

    // Add any references you want the users to be able to access, be warned that giving them access to some classes can allow
    // harmful code to be written and executed. I recommend that you write your own Class library that is the only reference it allows
    // thus they can only do the things you want them to.
    // (though things like "System.Xml.dll" can be useful, just need to provide a way users can read a file to pass in to it)
    // Just to avoid bloatin this example to much, we will just add THIS program to its references, that way we don't need another
    // project to store the interfaces that both this class and the other uses. Just remember, this will expose ALL public classes to
    // the "script"
    options.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

    // Compile our code
    CompilerResults result = csProvider.CompileAssemblyFromSource(options, code);
			
	// Print out the errors.
	foreach (CompilerError error in result.Errors) {
		string errorMsg = "";
		errorMsg += "At Line " + error.Line + ", Col " + error.Column + ": ";
		errorMsg += (error.IsWarning ? "warning" : "error") + " ";
		errorMsg += error.ErrorNumber + ": " + error.ErrorText;
		Console.WriteLine(errorMsg);
	}

    if (result.Errors.HasErrors) {
        // TODO: report back to the user that the script has errored
        Console.WriteLine("There were errors in the script!");
        return null;
    }

    if (result.Errors.HasWarnings) {
        // TODO: tell the user about the warnings, might want to prompt them if they want to continue
        // runnning the "script"
        Console.WriteLine("There were warnings in the script!");
		return null;
    }


    return result.CompiledAssembly;
}

static void RunScript(Assembly script) {
    // Now that we have a compiled script, lets run them
	// Find classes that implement IScriptType1
    foreach (Type type in script.GetExportedTypes()) {
        foreach (Type iface in type.GetInterfaces()) {
            if (iface == typeof(ScriptingInterface.IScriptType1)) {
                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);

                if (constructor != null && constructor.IsPublic) {
                    ScriptingInterface.IScriptType1 scriptObject = 
						constructor.Invoke(null) as ScriptingInterface.IScriptType1;

                    if (scriptObject != null) {
                        // Run the script.
						string result = scriptObject.RunScript(50);
						Console.WriteLine("Result = " + result);
                    }
                    else {
						// This shouldn't happen.
                    }
                }
                else
                {
                    // and even more friendly and explain that there was no valid constructor
                    // found and thats why this script object wasn't run
                }
            }
        }
    }
}
*/

	public static class ScriptingAttributes {

		// Use this for enum values that should be hidden in the editor.
		public class Hidden : Attribute {}

		public class Description : Attribute {
			public Description(string description) {
				this.Text = description;
			}
			public string Text { get; set; }
		}
	}
}
