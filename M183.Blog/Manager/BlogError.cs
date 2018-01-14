using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183.Blog.Manager
{
    public class BlogError: Exception
    {
        public BlogErrorType BlogErrorType { get; set; }

        public BlogError()
        {
        }
        public BlogError(string message)
        : base(message)
        {
        }
        public BlogError(string message, BlogErrorType type)
        : base(message)
        {
            BlogErrorType = type;
        }
    }

    public enum BlogErrorType
    {
        TokenNotValid,
        WrongUsernameOrPassword,
        ToManyLoginsAttempts,
        UserBlocked
    }
}