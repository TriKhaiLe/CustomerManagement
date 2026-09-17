using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using CustomerManagement.Client.Services;

namespace CustomerManagement.Client.Validation
{
    public class ServerAwareValidator
    {
        // The trimmer can remove public properties on the model if it does not know they are needed.
        // We take the runtime type once here so the published app still keeps the metadata needed for DataAnnotations validation.
        private readonly Type _modelType;
        private readonly Dictionary<string, string[]> _serverErrors = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _propertyNames = new(StringComparer.OrdinalIgnoreCase);

        public ServerAwareValidator(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type modelType)
        {
            _modelType = modelType;

            foreach (var property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                _propertyNames.Add(property.Name);
            }
        }

        public Func<object, string, IEnumerable<string>> Validate => ValidateProperty;

        public IReadOnlyList<string> UnmappedErrors { get; private set; } = Array.Empty<string>();

        public bool HasUnmappedErrors => UnmappedErrors.Count > 0;

        public void SetServerErrors(ApiException exception)
        {
            _serverErrors.Clear();
            UnmappedErrors = Array.Empty<string>();

            if (exception is null)
            {
                return;
            }

            var unmapped = new List<string>();

            foreach (var entry in exception.ValidationErrors)
            {
                var normalizedKey = NormalizeKey(entry.Key);
                var propertyName = _propertyNames.FirstOrDefault(property => string.Equals(property, normalizedKey, StringComparison.OrdinalIgnoreCase));

                if (propertyName is null)
                {
                    unmapped.AddRange(entry.Value);
                    continue;
                }

                if (!_serverErrors.TryGetValue(propertyName, out var existingMessages))
                {
                    _serverErrors[propertyName] = entry.Value;
                    continue;
                }

                var merged = new List<string>(existingMessages);
                merged.AddRange(entry.Value);
                _serverErrors[propertyName] = merged.ToArray();
            }

            if (unmapped.Count > 0)
            {
                UnmappedErrors = unmapped.ToArray();
            }
        }

        public void ClearServerErrors()
        {
            _serverErrors.Clear();
            UnmappedErrors = Array.Empty<string>();
        }

        private IEnumerable<string> ValidateProperty(object model, string propertyName)
        {
            var results = new List<string>();

            var propertyInfo = _modelType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo is not null)
            {
                var validationContext = new ValidationContext(model)
                {
                    MemberName = propertyName
                };

                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(propertyInfo.GetValue(model), validationContext, validationResults);

                foreach (var validationResult in validationResults)
                {
                    if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                    {
                        results.Add(validationResult.ErrorMessage);
                    }
                    else
                    {
                        results.Add("Invalid value.");
                    }
                }
            }

            if (_serverErrors.TryGetValue(propertyName, out var serverMessages))
            {
                results.AddRange(serverMessages);
            }

            return results;
        }

        private static string NormalizeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            var normalized = key.Trim();
            normalized = normalized.TrimStart('$', '.');

            var lastDotIndex = normalized.LastIndexOf('.');
            if (lastDotIndex >= 0)
            {
                normalized = normalized[(lastDotIndex + 1)..];
            }

            return normalized;
        }
    }
}
