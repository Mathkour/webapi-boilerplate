﻿using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;

namespace DN.WebApi.Infrastructure.Swagger;

public class AddTenantIdProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        if (context.MethodInfo?.GetCustomAttribute(typeof(SwaggerHeaderAttribute)) is SwaggerHeaderAttribute attribute)
        {
            var parameters = context.OperationDescription.Operation.Parameters;
            var existingParam = parameters.FirstOrDefault(p =>
                p.Kind == OpenApiParameterKind.Header && p.Name == attribute.HeaderName);

            // remove description from [FromHeader] argument attribute
            if (existingParam is not null)
            {
                parameters.Remove(existingParam);
            }

            parameters.Add(new OpenApiParameter
            {
                Name = attribute.HeaderName,
                Kind = OpenApiParameterKind.Header,
                Description = attribute.Description,
                IsRequired = attribute.IsRequired,
                Schema = string.IsNullOrEmpty(attribute.DefaultValue)
                    ? null
                    : new NJsonSchema.JsonSchema
                      {
                          Type = NJsonSchema.JsonObjectType.String,
                          Default = attribute.DefaultValue
                      }
            });
        }

        return true;
    }
}