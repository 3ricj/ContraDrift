namespace ContraDrift
{
    public class FitsManagerConfig
    {
        public string Ucac4Path { get; set; } = "";
        public string AstapExePath { get; set; } = @"C:\Program Files\astap\astap_cli.exe";
        public string VppwExePath { get; set; } = @"C:\VisualPinPointWrapper\VisualPinPointWrapper.exe";
        public bool EnableParentDirSolve { get; set; } = false;
    }
}
