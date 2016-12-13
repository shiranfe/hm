using System;
using System.Net;

namespace Common.Helpers
{

    /// <summary>
    /// 401 - Unauthorized  - if thrown, server automatic set user Unauthorized, so cant use it for client
    /// How Client see this statuses
    ///     403 Forbidden- isInternal==true ? not logged in : site is down
    /// </summary>
    public class ErrorHelper
    {
        //throw ErrorHelper.NotAcceptable();





        /// <summary>
        /// 404
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception CantSeeThisPage(string message)
        {
            /** IsExternal = false: */
            return Get(message, HttpStatusCode.NotFound);
        }


        /// <summary>
        /// 406
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception EntityMissing(string message)
        {
            return Get(message, HttpStatusCode.NotAcceptable);
        }

        /// <summary>
        /// 417 - mo permision to do action
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception NoPermission(string message)
        {
            return Get(message, HttpStatusCode.ExpectationFailed);
        }





        /// <summary>
        /// 409 - have more then 1 value
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception Conflict(string message)
        {
            return Get(message, HttpStatusCode.Conflict);
        }

        /// <summary>
        /// 407  - ExpectationFailed
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception GeneralError(string message)
        {
            return Get(message, HttpStatusCode.ExpectationFailed);
        }

        /// <summary>
        /// 200 - when want to log only, and not throw error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Exception OkButLog(string message)
        {
            return Get(message, HttpStatusCode.OK);
        }


        public static Exception Get(string message, HttpStatusCode httpStatusCode)
        {
            return new MyException(message, httpStatusCode);
        }

        public static Exception Full(string message, Exception e)
        {

            var inner = ExceptionMessage(e);
            message += " | " + inner;
            return new MyException(message, HttpStatusCode.BadRequest);
        }


        public static string ExceptionMessage(Exception e)
        {
            if (e.InnerException == null)
                return e.Message;

            return ExceptionMessage(e.InnerException);
        }

        public static string InnerExceptionMessage(Exception e)
        {
            if (e.InnerException == null)
                return string.Empty;

            return ExceptionMessage(e.InnerException);
        }
    }

    public class MyException : Exception
    {
        public readonly HttpStatusCode Status;

        public MyException()
        {
        }

        public MyException(string message)
            : base(message)
        {
        }

        public MyException(string message, HttpStatusCode status, Exception inner)
          : base(message, inner)
        {
            Status = status;
        }

        public MyException(string message, HttpStatusCode status)
            : base(message)
        {
            Status = status;
        }
    }
}
