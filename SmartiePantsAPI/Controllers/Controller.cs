using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartiePantsAPI.Data;
using SmartiePantsAPI.Models;


namespace SmartiePantsAPI.Controllers
{
    [Route("api/[action]")]
    [ApiController]

    public class Controller : ControllerBase
    {
        private readonly ApiContext _context;

        public Controller(ApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public JsonResult AddNewRandomDataToDB()
        {

            bool isDbEmpty = _context.Instances.Count() == 0;

            if (isDbEmpty)
                GenerateInstances(50);

            return new JsonResult(Ok("Data added successfully"));
        }

        [HttpGet]
        public JsonResult GetInstances()
        {
            return new JsonResult(_context.Instances);
        }

        [HttpGet]
        public JsonResult GetWaterfalls()
        {
            var data = _context.Waterfalls.Include(x => x.Instances);

            var test = _context.Waterfalls.Select(x => new
            {
                x.WaterfallId,
                Instances = x.Instances.OrderByDescending(i => i.Rate).ToList()
            })
                .ToList();

            return new JsonResult(test);
        }

        [HttpGet]
        public JsonResult GetWaterfallWithHighestRevenue()
        {
            var waterfallWithHighestSum = _context.Waterfalls
           .Select(x => new
           {
               x.WaterfallId,
               Instances = x.Instances.OrderByDescending(i => i.Rate).ToList(),
               SumOfRates = x.Instances.Sum(i => i.Rate)
           })
           .OrderByDescending(x => x.SumOfRates)
           .FirstOrDefault();

            return waterfallWithHighestSum != null ? new JsonResult(waterfallWithHighestSum) : new JsonResult(NotFound("DbIsEmpty"));
        }

        [HttpPost]
        public JsonResult AddInstances (int number)
        {
            var waterfalls = GenerateInstances(number);
            var result = waterfalls.Select(x => new
            {
                x.WaterfallId,
                Instances = x.Instances.OrderByDescending(i => i.Rate).ToList(),
            })
                .ToList();

            return new JsonResult(result);
        }

        protected List<Waterfall> GenerateInstances(int number)
        {

            List<Instance> instances = new List<Instance>();
            List<Waterfall> waterfalls = new List<Waterfall>();

            Random random = new Random();
            string[] network = { "Google", "Meta", "Unity" };

            for (int i = 0; i < number; i++)
            {
                instances.Add(new Instance
                {
                    Name = "Network" + (char)(65 + i),
                    Rate = random.Next(1, 101),
                    AdNetwork = network[random.Next(network.Length)],
                });
            }

            List<Instance> googleAdNetwork = instances.Where(x => x.AdNetwork == "Google").ToList();
            List<Instance> unityAdNetwork = instances.Where(x => x.AdNetwork == "Unity").ToList();
            List<Instance> metaAdNetwork = instances.Where(x => x.AdNetwork == "Meta").ToList();

            int count = Math.Max(googleAdNetwork.Count(), Math.Max(unityAdNetwork.Count(), metaAdNetwork.Count()));

            for (int i = 0; i < count; i++)
            {
                waterfalls.Add(new Waterfall { });

                if (googleAdNetwork.Count > 0)
                {
                    waterfalls[i].Instances.Add(googleAdNetwork[0]);
                    googleAdNetwork.RemoveAt(0);
                }

                if (unityAdNetwork.Count > 0)
                {
                    if (!waterfalls[i].Instances.Any(x => x.Rate == unityAdNetwork[0].Rate))
                    {
                        waterfalls[i].Instances.Add(unityAdNetwork[0]);
                        unityAdNetwork.RemoveAt(0);
                    }
                    else if (unityAdNetwork.Count() > 1)
                    {
                        if (!waterfalls[i].Instances.Any(x => x.Rate == unityAdNetwork[1].Rate))
                        {
                            waterfalls[i].Instances.Add(unityAdNetwork[1]);
                            unityAdNetwork.RemoveAt(1);
                        }

                    }
                }

                if (metaAdNetwork.Count > 0)
                {
                    if (!waterfalls[i].Instances.Any(x => x.Rate == metaAdNetwork[0].Rate))
                    {
                        waterfalls[i].Instances.Add(metaAdNetwork[0]);
                        metaAdNetwork.RemoveAt(0);
                    }
                    else if (metaAdNetwork.Count() > 1)
                    {
                        if (!waterfalls[i].Instances.Any(x => x.Rate == metaAdNetwork[1].Rate))
                        {
                            waterfalls[i].Instances.Add(metaAdNetwork[1]);
                            metaAdNetwork.RemoveAt(1);
                        }
                    }
                }
            }

            _context.Instances.AddRange(instances);
            _context.Waterfalls.AddRange(waterfalls);
            _context.SaveChanges();

            return waterfalls;
        }
    }
}


