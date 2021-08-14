using FluentValidation;

namespace Ethereal.WebAPI.Controllers.MainController.Models
{
    public class GetChaptersByUrlRequestDto
    {
        public string Url { get; set; }

        public string Description { get; set; }
        
        public bool IncludeThumbnails { get; set; }
        
        public class Validator : AbstractValidator<GetChaptersByUrlRequestDto>
        {
            public Validator()
            {
                RuleFor(c => c.Url)
                    .NotEmpty();
            }
        }
    }
}