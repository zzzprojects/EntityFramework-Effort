namespace Effort.Extra.Tests
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class PersonWithAttribute
    {
        [Column("Alias")]
        public string Name { get; set; }
    }
}