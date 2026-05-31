using FluentValidation;
using StitchArtisan.Backend.DTOs;

namespace StitchArtisan.Backend.Validators
{
    public class ProductVariantDtoValidator : AbstractValidator<ProductVariantDto>
    {
        public ProductVariantDtoValidator()
        {
            RuleFor(x => x.VariantName).NotEmpty().WithMessage("Tên phân loại không được để trống");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Giá không được âm");
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).WithMessage("Tồn kho không được âm");
        }
    }

    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .MaximumLength(200).WithMessage("Tên sản phẩm không được vượt quá 200 ký tự");

            RuleForEach(x => x.Variants).SetValidator(new ProductVariantDtoValidator());
        }
    }
}
