using PawnCloud.Debugging;

namespace PawnCloud;

public class PawnCloudConsts
{
    public const string LocalizationSourceName = "PawnCloud";

    public const string ConnectionStringName = "Default";

    public const bool MultiTenancyEnabled = true;


    /// <summary>
    /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
    /// </summary>
    public static readonly string DefaultPassPhrase =
        DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "ea7ec5f4778a4137844e34f2ae6febd1";
}
