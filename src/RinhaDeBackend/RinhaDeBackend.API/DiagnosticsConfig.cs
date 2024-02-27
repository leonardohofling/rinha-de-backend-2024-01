using System.Diagnostics;

namespace RinhaDeBackend.API
{
    public class DiagnosticsConfig
    {
        public const string SourceName = "MyCustomTracing";
        public ActivitySource Source = new ActivitySource(SourceName);
    }
}
