using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Dynamo.Ioc.Web
{
	class DynamoDataAnnotationsModelValidator : DataAnnotationsModelValidator
	{
		private readonly IServiceProvider _provider;

		public DynamoDataAnnotationsModelValidator(IServiceProvider provider, ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
			: base(metadata, context, attribute)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			_provider = provider;
		}

		// Copied from the DataAnnotationsModelValidator.Validate() method
		// Changed to support the IServiceProvider for loose coupling, by using the ValidationContext.GetService() method.
		public override IEnumerable<ModelValidationResult> Validate(object container)
		{
			ValidationContext validationContext = new ValidationContext(container ?? base.Metadata.Model, _provider, null);		// Changed to inject provider
			validationContext.DisplayName = base.Metadata.GetDisplayName();
			ValidationResult validationResult = this.Attribute.GetValidationResult(base.Metadata.Model, validationContext);

			if (validationResult != ValidationResult.Success)
			{
				yield return new ModelValidationResult
				{
					Message = validationResult.ErrorMessage
				};
			}

			yield break;
		}
	}
}
