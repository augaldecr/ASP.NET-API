namespace Shared.DTOs
{
    public class ResourcesCollectionDTO<T> : ResourceDTO where T : ResourceDTO
    {
        public List<T> Values { get; set; }
    }
}
