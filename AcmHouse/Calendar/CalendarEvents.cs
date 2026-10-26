using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.Profile
{
    public class CalendarEvent
    {
        private static int lastID = 0;
        public int ID { get; set; }
        private int userID;
        private string title;
        private string description;
        private DateTime startTime;
        private DateTime endTime;
        public int notify;
        private string userInfo;
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }
        public int Notify
        {
            get { return notify; }
            set { notify = value; }
        }
        public string UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; }
        }
        public CalendarEvent(string title, string description, DateTime startTime, DateTime endTime, int notify)
        {
            ID = ++lastID;
            Title = title;
            Description = description;
            StartTime = startTime;
            EndTime = endTime;
            Notify = notify;
        }
        public CalendarEvent(int id, string title, string description, DateTime startTime, DateTime endTime, int notify)
        {
            ID = id;
            Title = title;
            Description = description;
            StartTime = startTime;
            EndTime = endTime;
            Notify = notify;
        }
        public CalendarEvent(int id, string title, string description, DateTime startTime, DateTime endTime, int notify, string userInfo)
        {
            ID = id;
            Title = title;
            Description = description;
            StartTime = startTime;
            EndTime = endTime;
            Notify = notify;
            UserInfo = userInfo;
        }
    }
}
