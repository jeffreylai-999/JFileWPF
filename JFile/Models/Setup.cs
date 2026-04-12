namespace JFile.Models
{
    public class Setup
    {
        public bool? Trim { get; set; } = false;
        public bool? ConvertTabToSpace { get; set; } = false;
        public int? Space { get; set; } = 3;
        public bool? ConvertKeywordToUppercase { get; set; } = false;
        public bool? Override { get; set; } = false;
    }
}
