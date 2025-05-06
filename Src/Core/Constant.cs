namespace Core;

public class Constant
{
    public static readonly byte[] JwtSecret = "argfdsfaefadsfsdfdfaedfasdfafdsfadsfawadsfadfadsf"u8.ToArray();

    public static readonly string JwtUserId = "JwtUserId";

    public const string Issuer = "BitSignal";

    public const string Audience = "BitSignalWebClient";
}