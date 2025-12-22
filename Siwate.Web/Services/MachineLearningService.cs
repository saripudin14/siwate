using Microsoft.Extensions.Configuration;
using Microsoft.ML;
using Siwate.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Siwate.Web.Services
{
    public class MachineLearningService : IMachineLearningService
    {
        private readonly MLContext _mlContext;
        private readonly string _modelPath;
        private ITransformer _model;

        public MachineLearningService(IConfiguration configuration)
        {
            _mlContext = new MLContext(seed: 0);
            _modelPath = configuration["MLModelPath"] ?? "interview_model.zip";
            
            // Try to load model if exists
            if (File.Exists(_modelPath))
            {
                try
                {
                    _model = _mlContext.Model.Load(_modelPath, out _);
                }
                catch
                {
                    // Handle model load error or ignore
                }
            }
        }

        public void Train(IEnumerable<Dataset> data)
        {
            // 1. Prepare Data
            var trainData = data.Select(d => new ModelInput
            {
                AnswerText = d.AnswerText,
                TextLength = (float)d.AnswerText.Length,
                Score = (float)d.Score
            });

            IDataView dataView = _mlContext.Data.LoadFromEnumerable(trainData);

            // 2. Build Pipeline
            // Extract features: TF-IDF from AnswerText, and just use TextLength as is
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("TextFeatures", nameof(ModelInput.AnswerText))
                .Append(_mlContext.Transforms.Concatenate("Features", "TextFeatures", nameof(ModelInput.TextLength)))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(ModelInput.Score), maximumNumberOfIterations: 100));

            // 3. Train
            _model = pipeline.Fit(dataView);

            // 4. Save Model
            _mlContext.Model.Save(_model, dataView.Schema, _modelPath);
        }

        public float Predict(string answerText)
        {
            if (_model == null)
            {
                // Fallback or throw if model not trained
                return 0;
            }

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);

            var input = new ModelInput
            {
                AnswerText = answerText,
                TextLength = answerText.Length
            };

            var prediction = predictionEngine.Predict(input);
            
            // Clamp score between 0 and 100
            return Math.Max(0, Math.Min(100, prediction.Score));
        }
    }
}
