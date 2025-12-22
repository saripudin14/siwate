using Microsoft.ML.Data;

namespace Siwate.Web.Services
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public string AnswerText { get; set; }

        [LoadColumn(1)]
        public float TextLength { get; set; }

        [LoadColumn(2)]
        public float Score { get; set; } // Label
    }
}
