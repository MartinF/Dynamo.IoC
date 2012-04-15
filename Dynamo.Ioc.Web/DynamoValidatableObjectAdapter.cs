using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Dynamo.Ioc.Web
{
	public class DynamoValidatableObjectAdapter : ValidatableObjectAdapter
	{
		private readonly IServiceProvider _provider;

		public DynamoValidatableObjectAdapter(IServiceProvider provider, ModelMetadata metadata, ControllerContext context)
			: base(metadata, context)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			_provider = provider;
		}

		// Copied from the ValidatableObjectAdapter.Validate() method
		// Changed to support the IServiceProvider for loose coupling, by using the ValidationContext.GetService() method.
		public override IEnumerable<ModelValidationResult> Validate(object container)
		{
			object model = base.Metadata.Model;
			if (model == null)
			{
				return Enumerable.Empty<ModelValidationResult>();
			}

			IValidatableObject validatableObject = model as IValidatableObject;
			if (validatableObject == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The model object inside the metadata claimed to be compatible with {0}, but was actually {1}." /*MvcResources.ValidatableObjectAdapter_IncompatibleType*/, new object[]			// Changed to include the Exception description directly
				{
					typeof(IValidatableObject).FullName, 
					model.GetType().FullName
				}));
			}

			ValidationContext validationContext = new ValidationContext(validatableObject, _provider, null);		// Inject the provider here
			return this.ConvertResults(validatableObject.Validate(validationContext));
		}

		// Copied from ValidatableObjectAdapter.ConvertResults - No changes made
		private IEnumerable<ModelValidationResult> ConvertResults(IEnumerable<ValidationResult> results)
		{
			foreach (ValidationResult current in results)
			{
				if (current != ValidationResult.Success)
				{
					if (current.MemberNames == null || !current.MemberNames.Any<string>())
					{
						yield return new ModelValidationResult
						{
							Message = current.ErrorMessage
						};
					}
					else
					{
						foreach (string current2 in current.MemberNames)
						{
							yield return new ModelValidationResult
							{
								Message = current.ErrorMessage,
								MemberName = current2
							};
						}
					}
				}
			}
			yield break;
		}
	}
}
