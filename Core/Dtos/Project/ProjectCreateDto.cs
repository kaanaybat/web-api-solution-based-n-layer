namespace Core.Dtos
{
    public class ProjectCreateDto
    {
        public string Name { get; set; }
        public DateTime PublishDate { get; set; }
        public int CategoryId { get; set; }
    }
}