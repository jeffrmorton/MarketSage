using System;

namespace MarketSage
{
	// OneTimeSchedule is used to schedule an event to run only once
	// Used by specific tasks to check self status
    [Serializable]
	public class OneTimeSchedule : Schedule
	{
		public OneTimeSchedule(string name, DateTime startTime) 
			: base(name, startTime, ScheduleType.ONETIME)
		{
		}
        public OneTimeSchedule() : base()
        {
        }

		internal override void CalculateNextInvokeTime()
		{
			// it does not matter, since this is a one time schedule
			m_nextTime = DateTime.MaxValue;
		}
	}

	// IntervalSchedule is used to schedule an event to be invoked at regular intervals
	// the interval is specified in seconds. Useful mainly in checking status of threads
	// and connections. Use an interval of 60 hours for an hourly schedule
	public class IntervalSchedule : Schedule
	{
		public IntervalSchedule(string name, DateTime startTime, int secs,
					TimeSpan fromTime, TimeSpan toTime) // time range for the day
			: base(name, startTime, ScheduleType.INTERVAL)
		{
			m_fromTime = fromTime;
			m_toTime = toTime;
			Interval = secs;
		}
        public IntervalSchedule() : base()
        {
        }

		internal override void CalculateNextInvokeTime()
		{
			// add the interval of m_seconds
			m_nextTime = m_nextTime.AddSeconds(Interval);

			// if next invoke time is not within the time range, then set it to next start time
			if (! IsInvokeTimeInTimeRange())
			{
				if (m_nextTime.TimeOfDay < m_fromTime)
					m_nextTime.AddSeconds(m_fromTime.Seconds - m_nextTime.TimeOfDay.Seconds);
				else
					m_nextTime.AddSeconds((24 * 3600) - m_nextTime.TimeOfDay.Seconds + m_fromTime.Seconds);
			}

			// check to see if the next invoke time is on a working day
			while (! CanInvokeOnNextWeekDay())
				m_nextTime = m_nextTime.AddDays(1); // start checking on the next day
		}
	}

	// Daily schedule is used set off to the event every day
	// Mainly useful in maintanance, recovery, logging and report generation
	// Restictions can be imposed on the week days on which to run the schedule
	public class DailySchedule : Schedule
	{
		public DailySchedule(string name, DateTime startTime) : base(name, startTime, ScheduleType.DAILY)
		{
		}

        public DailySchedule() : base()
        {
            this.Type = ScheduleType.DAILY;
        }

		internal override void CalculateNextInvokeTime()
		{
			// add a day, and check for any weekday restrictions and keep adding a day
			m_nextTime = m_nextTime.AddDays(1);
			while (! CanInvokeOnNextWeekDay())
				m_nextTime = m_nextTime.AddDays(1);
		}
	}

	// Weekly schedules, useful generally in lazy maintanance jobs and
	// restarting services and others major jobs
	public class WeeklySchedule : Schedule
	{
		public WeeklySchedule(string name, DateTime startTime)
			: base(name, startTime, ScheduleType.WEEKLY)
		{
		}

        public WeeklySchedule() : base()
        {
        }

		// add a week (or 7 days) to the date
		internal override void CalculateNextInvokeTime()
		{
			m_nextTime = m_nextTime.AddDays(7);
		}
	}

	// Monthly schedule - used to kick off an event every month on the same day as scheduled
	// and also at the same hour and minute as given in start time
	public class MonthlySchedule : Schedule
	{
		public MonthlySchedule(string name, DateTime startTime)
			: base(name, startTime, ScheduleType.MONTHLY)
		{
		}
        public MonthlySchedule() : base()
        {
        }

		// add a month to the present time
		internal override void CalculateNextInvokeTime()
		{
			m_nextTime = m_nextTime.AddMonths(1);
		}
	}
}
