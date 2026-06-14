using System;
using System.Collections.Generic;
using System.Text;

namespace Online_Battleship.Models
{
    public class User
    {
        public int id {  get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string image {  get; set; }
        public DateTime dateOfBirth { get; set; }
        public int played { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int points { get; set; }
        public DateTime dateCreatedAt { get; set; }
        public bool isOnline { get; set; }
        public DateTime dateLastSeen { get; set; }

        public User(string username, string email, string password, string image, DateTime dateOfBirth, DateTime dateCreatedAt)
        {
            this.id = 0;
            this.username = username;
            this.email = email;
            this.password = password;
            this.image = image;
            this.dateOfBirth = dateOfBirth;
            this.wins = 0;
            this.losses = 0;
            this.played = 0;
            this.points = 0;
            this.dateCreatedAt = dateCreatedAt;
            this.isOnline = false;
            this.dateLastSeen = dateCreatedAt;
        }
    }
}
