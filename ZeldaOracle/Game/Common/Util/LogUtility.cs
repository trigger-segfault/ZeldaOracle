using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ZeldaOracle.Common.Util {

	public class LogMessage {
		public Logger Logger { get; set; }
		public string Text { get; set; }
		public string FileName { get; set; }
		public string MethodName { get; set; }
		public int FileLineNumber { get; set; }
	}

	public class Logger {

		private string name;
		private LoggingSystem loggingSystem;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Logger(string name, LoggingSystem loggingSystem) {
			this.name = name;
			this.loggingSystem = loggingSystem;
		}

		public Logger() :
			this("Unknown", null)
		{
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Log(string format, params object[] args) {
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(1);

			LogMessage message = new LogMessage();
			message.Logger = this;
			message.Text = string.Format(format, args);
			message.FileName = stackFrame.GetFileName();
			message.MethodName = stackFrame.GetMethod().Name;
			message.FileLineNumber = stackFrame.GetFileLineNumber();
			
			if (loggingSystem != null)
				loggingSystem.LogMessage(message);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public LoggingSystem LoggingSystem {
			get { return loggingSystem; }
			set { loggingSystem = value; }
		}
	}

	public static class Logs {
		public static LoggingSystem LoggingSystem { get; set; } 
		public static Logger Scripts { get; set; }
		public static Logger Entity { get; set; }
		public static Logger Physics { get; set; }
		public static Logger Interactions { get; set; }
		public static Logger Monsters { get; set; }
		public static Logger Player { get; set; }
		public static Logger Tile { get; set; }
	}

	public class LoggingSystem {

		private List<Logger> loggers;
		private List<LogMessage> messages;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public LoggingSystem() {
			loggers = new List<Logger>();
			messages = new List<LogMessage>();
		}
		

		//-----------------------------------------------------------------------------
		// Logging Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create a new logger with the given name.</summary>
		public Logger CreateLog(string name) {
			Logger logger = new Logger(name, this);
			return logger;
		}

		public void AddLogger(Logger logger) {
			loggers.Add(logger);
			logger.LoggingSystem = this;
		}

		/// <summary>Log a message, usually printing it to the console.</summary>
		public void LogMessage(LogMessage message) {
			messages.Add(message);
			PrintLogMessage(message);
		}

		/// <summary>Print a single log message to the console.</summary>
		private void PrintLogMessage(LogMessage message) {
			Console.WriteLine("{0}: {1}", message.Logger.Name, message.Text);
		}

		/// <summary>Print all log messages to the console.</summary>
		public void PrintAllLogMessages() {
			for (int i = 0; i < messages.Count; i++) {
				PrintLogMessage(messages[i]);
			}
		}
	}
}
