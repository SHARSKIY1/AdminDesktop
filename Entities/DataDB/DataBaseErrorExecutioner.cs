using Npgsql;
using System.Data.Common;
using System.Net.Sockets;

namespace AdminDesktop.Controls.General;

public static class DatabaseErrorHelper
{
    public static bool IsConnectionError(Exception exception)
    {
        if (exception is NpgsqlException ||
            exception is DbException ||
            exception is TimeoutException ||
            exception is SocketException)
        {
            return true;
        }

        return exception.InnerException != null &&
               IsConnectionError(exception.InnerException);
    }
}