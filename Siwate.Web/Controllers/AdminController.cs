using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siwate.Web.Data;
using Siwate.Web.Models;
using Siwate.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Siwate.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly SiwateDbContext _context;
        private readonly IMachineLearningService _mlService;

        public AdminController(SiwateDbContext context, IMachineLearningService mlService)
        {
            _context = context;
            _mlService = mlService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Questions()
        {
            var questions = await _context.Questions.OrderByDescending(q => q.CreatedAt).ToListAsync();
            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(string questionText)
        {
            if (!string.IsNullOrWhiteSpace(questionText))
            {
                _context.Questions.Add(new Question { QuestionText = questionText });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Questions");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q != null)
            {
                _context.Questions.Remove(q);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Questions");
        }

        public async Task<IActionResult> Datasets()
        {
            var datasets = await _context.Datasets.OrderByDescending(d => d.CreatedAt).ToListAsync();
            return View(datasets);
        }

        [HttpPost]
        public async Task<IActionResult> AddDataset(string answerText, int score)
        {
            if (!string.IsNullOrWhiteSpace(answerText))
            {
                _context.Datasets.Add(new Dataset { AnswerText = answerText, Score = score });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Datasets");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDataset(Guid id)
        {
            var d = await _context.Datasets.FindAsync(id);
            if (d != null)
            {
                _context.Datasets.Remove(d);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Datasets");
        }

        [HttpPost]
        public async Task<IActionResult> TrainModel()
        {
            var data = await _context.Datasets.ToListAsync();
            if (data.Count < 5)
            {
                TempData["Message"] = "Dataset terlalu sedikit (minimal 5).";
                return RedirectToAction("Index");
            }

            try
            {
                _mlService.Train(data);
                TempData["Message"] = "Model berhasil dilatih!";
            }
            catch (Exception ex)
            {
                 TempData["Message"] = "Error training model: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
