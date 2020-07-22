namespace DAL.Entities
{
    public class ThemeHashtag
    {
        public int ThemeId { get; set; }
        public int HashtagId { get; set; }
        public virtual Hashtag Hashtag { get; set; }
    }
}
