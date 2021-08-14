using FluentValidation;

namespace Ethereal.WebAPI.Controllers.MainController.Models
{
    public class DownloadThumbnailRequestDto
    {
        public string Url { get; set; }

        public class Validator : AbstractValidator<DownloadThumbnailRequestDto>
        {
            public Validator()
            {
                RuleFor(c => c.Url)
                    .NotEmpty();
            }
        }
    }
}