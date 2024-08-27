namespace Website.Models
{
    public class EventEditModel
    {
        public required Event Event { get; set; }

        public required IList<Employee> Employees { get; set; }
    }
}
