namespace GstSharp.ManualBinding;

public static class Module
{
    private static bool _isInitialized;

    /// <summary>
    ///     Initialize the <c>Gst</c> module.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Calling this method is necessary to correctly initialize the bindings
    ///         and should be done before using anything else in the <see cref="Gst" />
    ///         namespace.
    ///     </para>
    /// </remarks>
    public static void Initialize()
    {
        if (_isInitialized)
            return;

        ImportResolver.RegisterAsDllImportResolver();

        _isInitialized = true;
    }
}