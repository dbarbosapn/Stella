using System;

namespace Stella.Models
{
    public class Customer
    {
        public string Id { get; set; }
        public string Card { get; set; }
        public string Name { get; set; }
        public int? Number { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string Gender {
            get { return _gender; }
            set {
                switch (value)
                {
                    case "male":
                        SerializedGender = Locale.Locale.male;
                        break;
                    case "female":
                        SerializedGender = Locale.Locale.female;
                        break;
                    case "other":
                        SerializedGender = Locale.Locale.other;
                        break;
                    default:
                        SerializedGender = "";
                        break;
                }
                _gender = value;
            }
        }
        private string _gender;

        public DateTime? Birthdate {
            get { return _birthdate; }
            set {
                if (value.HasValue)
                    SerializedBirthdate = $"{value.Value.Year}-{value.Value.Month}-{value.Value.Day}";
                else
                    SerializedBirthdate = "";

                _birthdate = value;
            }
        }
        private DateTime? _birthdate;

        public string Notes {
            get { return _notes; }
            set {
                if (value != null)
                    SerializedNotes = value.Replace("\n", " ").Replace("\r", "");
                else
                    SerializedNotes = "";
                _notes = value;
            }
        }
        private string _notes;

        public string SerializedBirthdate { get; private set; }
        public string SerializedNotes { get; private set; }
        public string SerializedGender { get; private set; }

        public Customer(string id, string card, string name, int? number, string pin, string address, string phone, string email, string gender, DateTime? birthdate, string notes)
        {
            Id = id;
            Card = card;
            Name = name;
            Number = number;
            Pin = pin;
            Address = address;
            Phone = phone;
            Email = email;
            Gender = gender;
            Birthdate = birthdate;
            Notes = notes;
        }
    }
}
