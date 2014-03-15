using JetBrains.Annotations;

namespace LDtoVHDL.Model
{
	public enum MessageSeverity
	{
		Warning,
		Error
	}

	public class ValidationMessage
	{
		public MessageSeverity Severity { get; private set; }
		public string Message { get; private set; }

		public ValidationMessage(MessageSeverity severity, string message)
		{
			Severity = severity;
			Message = message;
		}

		[StringFormatMethod("format")]
		public static ValidationMessage Error(string format, params object[] args)
		{
			return new ValidationMessage(MessageSeverity.Error, string.Format(format, args));
		}
	}
}
