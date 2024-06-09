using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.Abstractions.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;

		try
		{
			logger.LogInformation($"Executing command {name}");

			var result = await next();

			logger.LogInformation($"Command {name} processed successfully");

			return result;
		}
		catch (Exception exception)
		{
			logger.LogError(exception, $"Commad {name} processing failed");
			throw;
		}
    }
}
