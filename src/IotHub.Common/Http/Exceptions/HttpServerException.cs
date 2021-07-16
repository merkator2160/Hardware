﻿using System;
using System.Net;

namespace IotHub.Common.Http.Exceptions
{
	public class HttpServerException : ApplicationException
	{
		public HttpServerException(HttpStatusCode statusCode, String message) : base(message)
		{
			StatusCode = statusCode;
		}

		public HttpStatusCode StatusCode { get; }
	}
}