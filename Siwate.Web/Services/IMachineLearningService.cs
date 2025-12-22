using Siwate.Web.Models;
using System.Collections.Generic;

namespace Siwate.Web.Services
{
    public interface IMachineLearningService
    {
        void Train(IEnumerable<Dataset> data);
        float Predict(string answerText);
    }
}
