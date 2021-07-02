using System;

namespace Stella.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public DateTime? Date {
            get { return _date; }
            set {
                if (value.HasValue)
                    SerializedDate = $"{value.Value.Day}-{value.Value.Month}-{value.Value.Year} {value.Value.Hour}:{value.Value.Minute}";
                else
                    SerializedDate = "";

                _date = value;
            }
        }
        private DateTime? _date;
        public string ServiceName { get; set; }
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

        public string SerializedDate { get; private set; }
        public string SerializedNotes { get; private set; }

        public Transaction(string id, DateTime? date, string serviceName, string notes)
        {
            Id = id;
            Date = date;
            ServiceName = serviceName;
            Notes = notes;
        }
    }
}
