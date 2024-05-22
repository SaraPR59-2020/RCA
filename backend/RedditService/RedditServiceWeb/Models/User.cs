using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedditServiceWeb.Models
{
    public class User
    {
        private int id;
        private string fname;
        private string lname;
        private string address;
        private string city;
        private string country;
        private string number;
        private string email;
        private string password;
        private HttpPostedFileBase img;

        public User(int id, string fname, string lname, string address, string city, string country, string number, string email, string password, string img)
        {
            Id = id;
            Fname = fname;
            Lname = lname;
            Address = address;
            City = city;
            Country = country;
            Number = number;
            Email = email;
            Password = password;
            Img = img;
        }

        public string Fname { get => fname; set => fname = value; }
        public string Lname { get => lname; set => lname = value; }
        public string Address { get => address; set => address = value; }
        public string City { get => city; set => city = value; }
        public string Country { get => country; set => country = value; }
        public string Number { get => number; set => number = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public string Img { get => img; set => img = value; }
        public int Id { get => id; set => id = value; }
    }
}