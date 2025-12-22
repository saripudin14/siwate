using Microsoft.ML.Data;

namespace Siwate.Web.Services
{
    public class ModelOutput
    {
        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
