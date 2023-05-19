using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skynet
{
    public enum ErrorCode : int
    {
        MAINMDL_INSTANCE_INVALID = 1,
        MAINMDL_WRONG_CONFIG = 2,
    }

    public static class ErrorManager
    {
        private struct ErrorData
        {
            public ErrorCode code;
            public string message;
        }

        private static Dictionary<ErrorCode, string> ErrorMessageContext = new Dictionary<ErrorCode, string>()
        {
            { ErrorCode.MAINMDL_INSTANCE_INVALID, "ERROR: ESSENTIAL INSTANCE IS NULL - SHUTTING DOWN" },
            { ErrorCode.MAINMDL_WRONG_CONFIG, "ERROR: WRONG CONFIGURATION - SHUTTING DOWN" }
        };

        public static void WriteErrorMessage(ErrorCode errorCode, bool shutdown = false)
        {
            Console.WriteLine(ErrorMessageContext[errorCode]);

            if (shutdown)
            {
                Console.WriteLine("PROGRAM SHUTDOWN DUE TO ERROR CODE {0}", (int)errorCode);
                Environment.Exit((int)errorCode);
            }
        }
    }
}
