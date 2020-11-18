using System;

namespace Seagate.AAS.Parsel.Services
{
	/// <summary>
	/// Summary description for TraceMessage.
	/// </summary>
    [Serializable()]
	public class TraceMessage
	{
		private string source;
		private string message;
		private DateTime timeStamp;

		public TraceMessage(string source, string message)
		{
			timeStamp = DateTime.Now;
			this.source = source;
			this.message = message;
		}

		public string Time
		{
			get { return timeStamp.ToString("h:mm:ss.ff"); }
		}

		public string Source
		{
			get { return source; }
		}

		public string Message
		{
			get { return message; }
		}

		public DateTime TimeStamp
		{
			get { return timeStamp; }
		}

		public string GetMessage(bool includeTime)
		{
			if(includeTime)
				return String.Format("({0}) {1}",Time,message);
			else
				return Message;
		}
	}
}
