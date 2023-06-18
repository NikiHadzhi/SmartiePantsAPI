using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace SmartiePantsAPI.Models
{
    public class Waterfall
    {
        public int WaterfallId { get; set; }

        public ICollection<Instance> Instances { get; } = new List<Instance>();

    }
}
