using System;
using System.Collegctions.Generic;  // Please correct a typo, should be Collections instead of Collegctions
using System.Linq;

namespace Utility.Valocity.ProfileHelper
{ 
    public class People            // People class represents a single entity, I would recommend renaming it to Person instead. Also, for better reading, please fix indentation for the whole class
    {
     private static readonly DateTimeOffset Under16 = DateTimeOffset.UtcNow.AddYears(-15); // This value is calculated only once when the application starts (or class is accessed). 
	                                                                                       // If the application runs for days, this "Under 16" threshold becomes stale. This should be calculated dynamically.
																						   // I would even recommend to remove it at all
     public string Name { get; private set; }
     public DateTimeOffset DOB { get; private set; }                                       // Why DateTimeOffset data type is used for DOB? Should be changed to DateTime and ideally renamed to DateOfBirth
     public People(string name) : this(name, Under16.Date) { }                             // Why do we even need to default this to a 15 year old person ?
     public People(string name, DateTime dob) {                                            // parameter dob and DOB have different data types. Please fix. Also, please rename to dateOfBirth instead of dob
	                                                                                       // Also, please move { to the next line
         Name = name;
         DOB = dob;
	}}                                                                                    // Please move second } to the next line

    public class BirthingUnit                // Name and functionality of the class is a bit weird. Is it factory or repository ? Currently sounds more like POCO object, but functionality is different. Please rename
    {
        /// <summary>
        /// MaxItemsToRetrieve              // This summary does not bring any value. Instead it is confusing (MaxItemsToRetrieve for _people list). Please add proper summary or remove summary completely
        /// </summary>
        private List<People> _people;       // Please initialize (private readonly List<Person> _people = new List<Person>();) here and remove constructor

        public BirthingUnit()               // Could be removed if _people initialized on the previous line of code
        {
            _people = new List<People>();
        }

        /// <summary>
        /// GetPeoples                        // This summary does not bring any value (it just repeats method name). Please add proper summary or remove summary completely
        /// </summary>
        /// <param name="j"></param>          // Should match actual parameter name
        /// <returns>List<object></returns>   // It returns List<People>, not List<object>. Please fix
        public List<People> GetPeople(int i)  // Please rename method name to match functionality and rename parameter i to something more meaningful (like numberOfPeople, count, etc)
        {
            for (int j = 0; j < i; j++)
            {
                try
                {
                    // Creates a dandon Name       // Should be random instead of dandon
                    string name = string.Empty;
                    var random = new Random();     // You should not use Random() inside the loop. Random uses the system clock for its seed which could lead to getting the exact same "Random" values for every person generated in that batch.
					                               // Please move new Random() to a static readonly field or inject it
                    if (random.Next(0, 1) == 0) {  // First, random.Next(0, 1) has an exclusive upper bound which means it will always return 0 in your case and you will never get "Betty".
					                               //     Should be replaced with random.Next(0, 2)
					                               // Second, please move { to the next line
                        name = "Bob";              // I would recommend to use constants or configuration settings instead of hard-coding names
                    }
                    else {                         // Please move { to the next line
                        name = "Betty";            // I would recommend to use constants or configuration settings instead of hard-coding names
                    }
                    // Adds new people to the list // Should be: Adds new person to the list
                    _people.Add(new People(name, DateTime.UtcNow.Subtract(new TimeSpan(random.Next(18, 85) * 356, 0, 0, 0)))); // Very complex and strange Date of birth calculation. I would completely replace with something more simple
					                                                                                                           // Also, year has 365/366 days, not 356. Please fix
                }
                catch (Exception e)
                {
                    // Dont think this should ever happen                      // It's better to remove try-catch in this method
                    throw new Exception("Something failed in user creation");  // Catching a specific exception but throwing a generic one, losing the stack trace and the original error message.
                                                            				   // Please remove the try/catch block entirely (let the caller handle it) or wrap it properly while preserving the inner exception.
                }
            }
            return _people;
        }

        private IEnumerable<People> GetBobs(bool olderThan30)                 // This is a private method and it is not used. Could be removed
		                                                                      // Also, if keeping, I would recommend to pass person's name as a parameter and make method more generic (GetPeopleByName)
        {
            return olderThan30 ? _people.Where(x => x.Name == "Bob" && x.DOB >= DateTime.Now.Subtract(new TimeSpan(30 * 356, 0, 0, 0))) : _people.Where(x => x.Name == "Bob");  // Again 356 days in a year and complex calculations. Please fix
			                                                                  // FYI, the logic above (x.DOB >= DateTime.Now.Subtract(...)) checks if the Date of Birth is greater (more recent) than the date 30 years ago. This returns people younger than 30, not older
        }

        public string GetMarried(People p, string lastName)                   // Method name should be changed to match functionality
        {
            if (lastName.Contains("test"))                                    // Please add {} for if statement to match other if-s
                return p.Name;
            if ((p.Name.Length + lastName).Length > 255)                      // Could you please explain your logic here to add lenght of p.Name and lastname (string). Please fix
            {
                (p.Name + " " + lastName).Substring(0, 255);                  // Why do we need to create a new substring? It is not assigned to any variable and not used/returned
            }

            return p.Name + " " + lastName;
        }
    }
}