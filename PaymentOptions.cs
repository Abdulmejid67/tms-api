using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

// Option 1: Simple with Data Annotations
public class PaymentOptions
{
    [Required(ErrorMessage = "GatewayUrl is required")]
    [Url(ErrorMessage = "GatewayUrl must be a valid URL")]
    public required string GatewayUrl { get; init; }

    [Range(100, 100000, ErrorMessage = "MaxDepositBirr must be between 100 and 100000")]
    public decimal MaxDepositBirr { get; init; }

    [Range(0, 100, ErrorMessage = "TimeoutSeconds must be between 0 and 100")]
    public int TimeoutSeconds { get; init; } = 30;
}

// Option 2: Using IValidateOptions for custom validation logic
public class PaymentOptionsValidator : IValidateOptions<PaymentOptions>
{
    public ValidateOptionsResult Validate(string? name, PaymentOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.GatewayUrl))
        {
            return ValidateOptionsResult.Fail("GatewayUrl is required");
        }

        if (!Uri.IsWellFormedUriString(options.GatewayUrl, UriKind.Absolute))
        {
            return ValidateOptionsResult.Fail("GatewayUrl must be a valid URI");
        }

        if (options.MaxDepositBirr < 100 || options.MaxDepositBirr > 100000)
        {
            return ValidateOptionsResult.Fail("MaxDepositBirr must be between 100 and 100000");
        }

        return ValidateOptionsResult.Success;
    }
}