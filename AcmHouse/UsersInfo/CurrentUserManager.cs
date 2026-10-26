using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AcmHouse.UsersInfo
{
    public class CurrentUserManager
    {
        private static CurrentUser currentUser;
        public SQLiteManager manager = new SQLiteManager();

        public static CurrentUser CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; }
        }
    }
}
