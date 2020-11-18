//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/16/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;

namespace Seagate.AAS.Parsel.Services
{
    [Serializable]
	public class MessageRecord
	{
		public MessageRecord()
		{
			Init();
		}

		public MessageRecord(IErrorMessage msg)
		{
			Init();
			text = msg.FullDescription;
			source = msg.Source;
		}

		public string text;
		public string source;

		public void Init()
		{
			text = null;
			source = null;
			count = 0;
			avgTime = TimeSpan.Zero;
			totalTime = TimeSpan.Zero;
			minTime = TimeSpan.MaxValue;
			maxTime = TimeSpan.MinValue;
		}

		public void Start()
		{
			count++;
			startTime = DateTime.Now;
		}

		public void Stop()
		{
			TimeSpan duration = DateTime.Now - startTime;
			totalTime += duration;
			if(duration < minTime)
				minTime = duration;
			if(duration > maxTime)
				maxTime = duration;

			long ticks = avgTime.Ticks;
			ticks += (duration.Ticks - ticks) / count;
			avgTime = TimeSpan.FromTicks(ticks);
		}

		public string AverageTime() { return timeSpanString(avgTime); }
		public string TotalTime()	{ return timeSpanString(totalTime); }
		public string MinTime()		{ return timeSpanString(minTime); }
		public string MaxTime()		{ return timeSpanString(maxTime); }
		public string Count()		{ return count.ToString(); }
        public DateTime StartTime() { return startTime; }

		private string timeSpanString(TimeSpan ts)
		{
			return string.Format("{0}:{1}",ts.Minutes.ToString("d2"),ts.Seconds.ToString("d2"));
		}
		private DateTime startTime;
		private TimeSpan totalTime;
		private TimeSpan minTime;
		private TimeSpan maxTime;
		private TimeSpan avgTime;
		private int		 count;
	}
}