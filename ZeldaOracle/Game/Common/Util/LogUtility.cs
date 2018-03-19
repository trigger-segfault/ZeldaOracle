using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZeldaOracle.Common.Util {

	public enum LogLevel {
		All = 0,

		Info = 0,
		Notice = 1,
		Warning = 2,
		Error = 3,

		Count = 4,
		None = 4,
	}

	public class LogMessage {
		public Logger Logger { get; set; }
		public string Text { get; set; }
		public string FileName { get; set; }
		public string MethodName { get; set; }
		public int FileLineNumber { get; set; }
		public LogLevel LogLevel { get; set; }
		public int GameTime { get; set; }
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


		//-----------------------------------------------------------------------------
		// Log Message
		//-----------------------------------------------------------------------------

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void LogInfo(string format, params object[] args) {
			LogMessage(LogLevel.Info, format, args);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void LogNotice(string format, params object[] args) {
			LogMessage(LogLevel.Notice, format, args);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void LogWarning(string format, params object[] args) {
			LogMessage(LogLevel.Error, format, args);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void LogError(string format, params object[] args) {
			LogMessage(LogLevel.Error, format, args);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void Log(string format, params object[] args) {
			LogMessage(LogLevel.Info, format, args);
		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		[MethodImpl(MethodImplOptions.NoInlining)]
		public void LogMessage(LogLevel level, string format, params object[] args) {
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(2);

			LogMessage message = new LogMessage() {
				Logger			= this,
				LogLevel		= level,
				Text			= string.Format(format, args),
				FileName		= stackFrame.GetFileName(),
				MethodName		= stackFrame.GetMethod().Name,
				FileLineNumber	= stackFrame.GetFileLineNumber(),
				GameTime		= loggingSystem.GameTimeFunction(),
			};

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
		public static LoggingSystem LoggingSystem { get; set; }  = null;

		public static Logger Initialization { get; set; }
		public static Logger Scripts { get; set; }
		public static Logger Entity { get; set; }
		public static Logger Physics { get; set; }
		public static Logger Interactions { get; set; }
		public static Logger Monsters { get; set; }
		public static Logger Player { get; set; }
		public static Logger Tile { get; set; }


		public static void InitializeLogs() {
			if (LoggingSystem == null) {
				LoggingSystem = new LoggingSystem();
				foreach (PropertyInfo property in typeof(Logs).GetProperties()) {
					if (property.PropertyType == typeof(Logger))
						property.SetValue(null, LoggingSystem.CreateLog(property.Name));
				}
			}
		}
	}


	public delegate int GameTimeFunction();


	public class LoggingSystem {

		private List<Logger> loggers;
		private List<LogMessage> messages;
		private LogLevel logLevel;
		private ConsoleColor[] logLevelColors;
		private string[] logLevelNames;
		private GameTimeFunction gameTimeFunction;
		private bool colorizeLogMessages;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public LoggingSystem() {
			loggers				= new List<Logger>();
			messages			= new List<LogMessage>();
			logLevel			= LogLevel.All;
			gameTimeFunction	= delegate() { return 0; };
			colorizeLogMessages	= false;

			logLevelColors = new ConsoleColor[(int) LogLevel.Count];
			logLevelColors[(int) LogLevel.Info]		= ConsoleColor.Gray;
			logLevelColors[(int) LogLevel.Notice]	= ConsoleColor.White;
			logLevelColors[(int) LogLevel.Warning]	= ConsoleColor.Yellow;
			logLevelColors[(int) LogLevel.Error]	= ConsoleColor.Red;

			logLevelNames = new string[(int) LogLevel.Count];
			logLevelNames[(int) LogLevel.Info]		= "INFO";
			logLevelNames[(int) LogLevel.Notice]	= "NOTE";
			logLevelNames[(int) LogLevel.Warning]	= "WARN";
			logLevelNames[(int) LogLevel.Error]		= "ERR!";
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
			if (message.LogLevel >= logLevel) {
				ConsoleColor placeholder = ConsoleColor.Gray;
				if (colorizeLogMessages) {
					placeholder = Console.ForegroundColor;
					Console.ForegroundColor = logLevelColors[(int) message.LogLevel];
				}

				Console.WriteLine("{0} : {1} : {2} : {3}",
					logLevelNames[(int) message.LogLevel], message.GameTime,
					message.Logger.Name, message.Text);

				if (colorizeLogMessages)
					Console.ForegroundColor = placeholder;
			}
		}

		/// <summary>Print all log messages to the console.</summary>
		public void PrintAllLogMessages() {
			for (int i = 0; i < messages.Count; i++)
				PrintLogMessage(messages[i]);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public LogLevel LogLevel {
			get { return LogLevel; }
			set { logLevel = value; }
		}

		public GameTimeFunction GameTimeFunction {
			get { return gameTimeFunction; }
			set { gameTimeFunction = value; }
		}

		public bool ColorizeLogMessages {
			get { return colorizeLogMessages ; }
			set { colorizeLogMessages = value; }
		}
	}
}
