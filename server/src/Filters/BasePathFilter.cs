using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HiringManagerServer.Filters
{
    /// <summary>
    /// BasePath Document Filter sets BasePath property of Swagger and removes it from the individual URL paths
    /// </summary>
    public class BasePathFilter : IDocumentFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath">BasePath to remove from Operations</param>
        public BasePathFilter(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// Gets the BasePath of the Swagger Doc
        /// </summary>
        /// <returns>The BasePath of the Swagger Doc</returns>
        public string BasePath { get; }

        /// <summary>
        /// Apply the filter
        /// </summary>
        /// <param name="swaggerDoc">OpenApiDocument</param>
        /// <param name="context">FilterContext</param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            //swaggerDoc.BasePath = BasePath;

            var pathsToModify = swaggerDoc.Paths.Where(p => p.Key.StartsWith(BasePath)).ToList();

            foreach (var (key, value) in pathsToModify)
            {
                if (key.StartsWith(BasePath))
                {
                    var newKey = Regex.Replace(key, $"^{BasePath}", string.Empty);
                    swaggerDoc.Paths.Remove(key);
                    swaggerDoc.Paths.Add(newKey, value);
                }
            }
        }
    }
}
