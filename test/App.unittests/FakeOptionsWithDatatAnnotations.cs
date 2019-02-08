using System.ComponentModel.DataAnnotations;

namespace App.unittests
{
    public class FakeOptionsWithDatatAnnotations
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value should be greater than or equal to 1")]
        public int Id { get; set; }

        [MinLength(2)]
        [MaxLength(4)]
        public string Name { get; set; }

        [RegularExpression(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", ErrorMessage = "Must be valid Guid Id")]
        public string GuidId { get; set; }
    }
}
