using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartiePantsAPI.Models
{
    public class Instance
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range (1, 100)]
        public double Rate { get; set; }

        [Required]
        [RegularExpression(("Google|Meta|Unity"), ErrorMessage = "You can choose between Google, Meta and Unity" )]
        public string AdNetwork { get; set; }

        [ForeignKey("Waterfall")]
        public int WaterfallId { get; set; }

    }
}
