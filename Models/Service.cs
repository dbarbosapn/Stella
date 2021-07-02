namespace Stella.Models
{
    public class Service
    {
        public string Id { get; set; }
        public string Name { get; set; }

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

        public string SerializedNotes { get; private set; }

        public Service(string id, string name, string notes)
        {
            Id = id;
            Name = name;
            Notes = notes;
        }
    }
}
