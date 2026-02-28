namespace CRUDWithAuth.Helpers
{
    public class ApiException : Exception
    {
        public int code = 0;
        public int StatusCode { get; set; }
        public Exception Errors { get; set; }
        public ApiException(string message, int code, Exception errors) : base(message, errors)
        {
            StatusCode = code;
            Errors = errors;
        }
        public ApiException(int code, string message) : base(message)
        {
            StatusCode = code;
        }

        public class ServerResult
        {
            public static void ThrowInvalidToken()
            {
                throw new ApiException(401, "invalid_token");
            }

            public static void ThrowServerError(string msg)
            {
                throw new ApiException(500, msg);
            }

            public static void ThrowForbidden()
            {
                throw new ApiException(403, "forbidden");
            }

            public static void ThrowEmptyToken()
            {
                throw new ApiException(401, "empty_token");
            }

            public static void ThrowAlreadyExists(string msg)
            {
                throw new ApiException(409, msg);
            }

            public static void ThrowDoesNotExist(string msg)
            {
                throw new ApiException(404, msg);
            }

            public static void ThrowSyncError(string msg)
            {
                throw new ApiException(500, msg);
            }

            public static void ThrowDataException(string msg)
            {
                if (msg == null)
                    msg = "data is missing";
                throw new ApiException(200, msg);
            }

            public static void ThrowMissingJSON()
            {
                throw new ApiException(400, "missing body / json");
            }
            public static void ThrowValidationError()
            {
                throw new ApiException(400, "validation error");
            }

            public static void ThrowImageDimension()
            {
                throw new ApiException(400, "image dimension error");
            }

            public static void ThrowImageFormat()
            {
                throw new ApiException(400, "image format error");
            }

            public static void ThrowLengthError(string msg)
            {
                if (msg == null)
                    msg = "?";
                throw new ApiException(413, msg);
            }

        }
    }
}
