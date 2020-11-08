using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClassLibrary
{
    public class Person
    {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public string City { get; set; }




        //Constructors
        public Person()
        {

        }
        public Person( string firstName, string lastName, int age, string city )
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            City = city;
        }

        
    }
}

