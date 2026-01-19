using System.Collections.Generic;
using System.Threading.Tasks;
using Siwate.Web.Models;

namespace Siwate.Web.Services
{
    public interface IMachineLearningService
    {
        // Fitur Train tidak lagi wajib/dibutuhkan by default untuk Gemini, tapi kita keep supaya tidak error di Controller lama sebelum dihapus
        void Train(IEnumerable<Dataset> data);

        // Update: Predict sekarang butuh QuestionText dan bersifat Async
        Task<(float Score, string Feedback)> PredictAsync(string questionText, string answerText);
    }
}
