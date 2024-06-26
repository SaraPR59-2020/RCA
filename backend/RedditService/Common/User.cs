﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

public class User : TableEntity
{
    private int user_id;
    private string name;
    private string surname;
    private string address;
    private string city;
    private string country;
    private string phone_number;
    private string email;
    private string password;
    public String PhotoUrl { get; set; }
    public String ThumbnailUrl { get; set; }

    public User(int user_id, string name, string surname, string address, string city, string country, string phone_number, string email, string password)
    {
        this.user_id = user_id;
        this.name = name;
        this.surname = surname;
        this.address = address;
        this.city = city;
        this.country = country;
        this.phone_number = phone_number;
        this.email = email;
        this.password = password;
    }

    public int User_id { get => user_id; set => user_id = value; }
    public string Name { get => name; set => name = value; }
    public string Surname { get => surname; set => surname = value; }
    public string Address { get => address; set => address = value; }
    public string City { get => city; set => city = value; }
    public string Country { get => country; set => country = value; }
    public string Phone_number { get => phone_number; set => phone_number = value; }
    public string Email { get => email; set => email = value; }
    public string Password { get => password; set => password = value; }

    public User(string indexNo)
    {
        PartitionKey = "User";
        RowKey = indexNo;
    }
    public User()
    {

    }
}
