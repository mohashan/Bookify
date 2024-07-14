using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Application.Abstractions.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest,TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
	where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;

		try
		{
			logger.LogInformation($"Executing request {name}");

			var result = await next();

			if (result.IsSuccess)
			{
				logger.LogInformation($"Request {name} processed successfully");
			}
			else
			{
				using (LogContext.PushProperty("Error", result.Error, true))
				{
					logger.LogError($"Request {name} processed with error");
				}

			}
            return result;
		}
		catch (Exception exception)
		{
			logger.LogError(exception, $"Request {name} processing failed");
			throw;
		}
    }
}
