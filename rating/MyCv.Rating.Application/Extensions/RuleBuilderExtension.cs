using FluentValidation;

namespace MyCv.Rating.Application.Extensions
{
    /// <summary>
    /// Extension methods for validation rules.
    /// </summary>
    internal static class RuleBuilderExtension
    {
        /// <summary>
        /// Check if the string is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        internal static IRuleBuilderOptions<T, string> MustNotBeNullNorEmpty<T>(this IRuleBuilder<T, string> ruleBuilder, string errorMessage = "A value must be specified for {PropertyName}.")
        {
            return ruleBuilder.NotEmpty()
                              .WithMessage(errorMessage);
        }

        /// <summary>
        /// Check if the integer is in a range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        internal static IRuleBuilderOptions<T, int> MustBeBetween<T>(this IRuleBuilder<T, int> ruleBuilder, int min, int max, string errorMessage = "The value of {PropertyName} must be in range [{min}; {max}].")
        {
            return ruleBuilder.Must(v => v >= min)
                .Must(v => v <= max)
                .WithMessage(errorMessage);
        }
    }
}
