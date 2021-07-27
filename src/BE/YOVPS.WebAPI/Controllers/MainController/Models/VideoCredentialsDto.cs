using FluentValidation;

namespace YOVPS.WebAPI.Controllers.MainController.Models
{
    public class VideoCredentialsDto
    {
        public string Url { get; set; }

        public string Description { get; set; }

        public int? Index { get; set; }

        public class Validator : AbstractValidator<VideoCredentialsDto>
        {
            public Validator()
            {
                RuleFor(c => c.Url)
                    .NotEmpty();
            }
        }
    }
}