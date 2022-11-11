namespace Core
{
    public class Project:BaseEntity
    {
        public string Name { get; set; }
        public DateTime PublishDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}