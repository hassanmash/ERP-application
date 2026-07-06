namespace Application.DTOs.PlatformAdmin
{
    /// <summary>
    /// Full-replace semantics: whatever module names are listed here become
    /// the complete set of enabled modules for the org. Any existing module
    /// row not in this list gets removed; any name in this list not already
    /// present gets added. The Angular client just sends its current checkbox
    /// state — no diffing required client-side.
    /// </summary>
    public class UpdateOrganizationModulesRequest
    {
        public List<string> EnabledModules { get; set; } = new();
    }
}
