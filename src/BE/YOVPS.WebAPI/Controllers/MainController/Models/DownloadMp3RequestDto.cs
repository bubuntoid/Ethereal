using FluentValidation;

namespace YOVPS.WebAPI.Controllers.MainController.Models
{
    public class DownloadMp3RequestDto
    {
        public string Url { get; set; }

        public string Description { get; set; }

        public int? Index { get; set; }

        public class Validator : AbstractValidator<DownloadMp3RequestDto>
        {
            public Validator()
            {
                RuleFor(c => c.Url)
                    .NotEmpty();
            }
        }
    }
}