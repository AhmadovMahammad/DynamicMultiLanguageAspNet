namespace TestEnvironment.Models.Entities
{
    public class BaseEntity<T> where T : unmanaged
    {
        public T Id { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public DateTime? DeletedTime { get; set; }
    }
}
