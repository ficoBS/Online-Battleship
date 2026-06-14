using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public class UsersList
    {
        public List<User> users;

        public UsersList()
        {
            users = new List<User>();
            users.Add(new User("fico", "filipjovanvoski006@gmail.com", "password", "", DateTime.Now, DateTime.Now));
            users.Add(new User("ibro", "ibro123@gmail.com", "password123", "", DateTime.Now, DateTime.Now));
            users.Add(new User("kosta", "kostakosta@yahoo.com", "passwordKosta", "", DateTime.Now, DateTime.Now));
            users.Add(new User("trajce", "trajce@gmail.com", "passwordt", "", DateTime.Now, DateTime.Now));
        }
    }
}
