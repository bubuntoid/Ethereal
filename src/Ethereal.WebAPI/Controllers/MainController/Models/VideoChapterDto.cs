namespace Ethereal.WebAPI.Controllers.MainController.Models
{
    public class VideoChapterDto
    {
        public string Original { get; set; }

        public string YoutubeTitle { get; set; }
        
        public string Name { get; set; }

        public string StartTimespan { get; set; }
        
        public string EndTimespan { get; set; }

        public string Duration { get; set; }
        
        public string ThumbnailBase64 { get; set; }
    }
}