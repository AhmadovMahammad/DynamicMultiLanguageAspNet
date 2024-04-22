namespace TestEnvironment.Models.Entities
{
    public class Faq : BaseEntity<int>
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}
