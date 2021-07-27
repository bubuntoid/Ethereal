using FluentValidation;

namespace YOVPS.WebAPI.Controllers.MainController.Models
{
    public class DownloadZipRequestDto
    {
        public string Url { get; set; }

        public string Description { get; set; }

        public class Validator : AbstractValidator<DownloadZipRequestDto>
        {
            public Validator()
            {
                RuleFor(c => c.Url)
                    .NotEmpty();
            }
        }
    }
}