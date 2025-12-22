using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siwate.Web.Data;
using Siwate.Web.Models;
using Siwate.Web.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Siwate.Web.Controllers
{
    [Authorize]
    public class InterviewController : Controller
    {
        private readonly SiwateDbContext _context;
        private readonly IMachineLearningService _mlService;

        public InterviewController(SiwateDbContext context, IMachineLearningService mlService)
        {
            _context = context;
            _mlService = mlService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Start()
        {
            // Pick a random question
            var count = await _context.Questions.CountAsync();
            if (count == 0)
            {
                ViewBag.Error = "Belum ada pertanyaan tersedia.";
                return View("Index");
            }

            // Simple random approach (not efficient for huge tables but fine here)
            var question = await _context.Questions.OrderBy(q => Guid.NewGuid()).FirstOrDefaultAsync();
            return View("Question", question);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswer(Guid questionId, string answerText)
        {
            if (string.IsNullOrWhiteSpace(answerText))
            {
                // Retrieve question again to show view
                var question = await _context.Questions.FindAsync(questionId);
                ViewBag.Error = "Jawaban tidak boleh kosong.";
                return View("Question", question);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // 1. Save Answer
            var answer = new Answer
            {
                QuestionId = questionId,
                UserId = userId,
                AnswerText = answerText
            };
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();

            // 2. Predict Score
            float predictedScore = _mlService.Predict(answerText);
            int score = (int)Math.Round(predictedScore);

            // 3. Generate Feedback
            string feedback;
            if (score >= 80) feedback = "Luar biasa! Jawaban Anda sangat relevan dan terstruktur dengan baik.";
            else if (score >= 60) feedback = "Cukup baik, namun bisa lebih mendetail pada poin-poin kunci.";
            else feedback = "Jawaban perlu perbaikan. Cobalah untuk lebih spesifik dan relevan dengan pertanyaan.";

            // 4. Save Result
            var result = new InterviewResult
            {
                UserId = userId,
                AnswerId = answer.Id,
                Score = score,
                Feedback = feedback
            };
            _context.InterviewResults.Add(result);
            await _context.SaveChangesAsync();

            return RedirectToAction("Result", new { id = result.Id });
        }

        public async Task<IActionResult> Result(Guid id)
        {
            var result = await _context.InterviewResults
                .Include(r => r.Answer)
                .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null || result.UserId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(result);
        }

        public async Task<IActionResult> History()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var history = await _context.InterviewResults
                .Include(r => r.Answer)
                    .ThenInclude(a => a.Question)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(history);
        }
    }
}
