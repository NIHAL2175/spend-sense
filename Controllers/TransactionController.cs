using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Geom;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace Expense_Tracker.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var applicationDbContext = _context.Transactions
                .Where(t => t.UserId == userId)
                .Include(t => t.Category);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();

            if (id == 0)
                return View(new Transaction());
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var transaction = _context.Transactions
                    .FirstOrDefault(t => t.TransactionId == id && t.UserId == userId);

                if (transaction == null)
                    return NotFound();

                return View(transaction);
            }
        }

        // POST: Transaction/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                {
                    transaction.UserId = userId;
                    _context.Add(transaction);
                }
                else
                {
                    var existingTransaction = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.TransactionId == transaction.TransactionId && t.UserId == userId);

                    if (existingTransaction == null)
                        return NotFound();

                    existingTransaction.CategoryId = transaction.CategoryId;
                    existingTransaction.Amount = transaction.Amount;
                    existingTransaction.Note = transaction.Note;
                    existingTransaction.Date = transaction.Date;

                    _context.Update(existingTransaction);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateCategories();
            return View(transaction);
        }

        // DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == id && t.UserId == userId);

            if (transaction == null)
                return NotFound();

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();

            Category DefaultCategory = new Category()
            {
                CategoryId = 0,
                Title = "Choose a Category"
            };

            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }

        // EXPORT PDF (FIXED)
        public async Task<IActionResult> ExportPdf(DateTime? startDate, DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // FIXED DATE RANGE
            var start = (startDate ?? DateTime.Today.AddDays(-6)).Date;
            var end = (endDate ?? DateTime.Today).Date;

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.Date >= start && t.Date < end.AddDays(1))
                .Include(t => t.Category)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            // FIXED CATEGORY CHECK
            var totalIncome = transactions
                .Where(t => t.Category?.Type?.ToLower() == "income")
                .Sum(t => t.Amount);

            var totalExpense = transactions
                .Where(t => t.Category?.Type?.ToLower() == "expense")
                .Sum(t => t.Amount);

            var balance = totalIncome - totalExpense;

            using var ms = new MemoryStream();

            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, PageSize.A4);
            document.SetMargins(40, 40, 40, 40);

            // COLORS
            var green = new DeviceRgb(74, 222, 128);
            var red = new DeviceRgb(248, 113, 113);
            var blue = new DeviceRgb(96, 165, 250);
            var darkBg = new DeviceRgb(13, 20, 25);
            var cardBg = new DeviceRgb(17, 25, 32);
            var mutedText = new DeviceRgb(122, 145, 128);
            var lightText = new DeviceRgb(232, 240, 233);

            // FONTS
            var bold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var regular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // HEADER
            document.Add(new Paragraph("SpendSense")
                .SetFont(bold).SetFontSize(22).SetFontColor(green));

            document.Add(new Paragraph($"Financial Report • {start:dd MMM yyyy} – {end:dd MMM yyyy}")
                .SetFont(regular).SetFontSize(9).SetFontColor(mutedText)
                .SetMarginBottom(20));

            // SUMMARY
            document.Add(new Paragraph($"Total Income: ₹ {totalIncome:N0}")
                .SetFont(bold).SetFontColor(green));

            document.Add(new Paragraph($"Total Expense: ₹ {totalExpense:N0}")
                .SetFont(bold).SetFontColor(red));

            document.Add(new Paragraph($"Balance: ₹ {balance:N0}")
                .SetFont(bold).SetFontColor(blue)
                .SetMarginBottom(20));

            // TABLE
            var table = new Table(4).UseAllAvailableWidth();

            table.AddHeaderCell("Category");
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Note");
            table.AddHeaderCell("Amount");

            foreach (var t in transactions)
            {
                var isIncome = t.Category?.Type?.ToLower() == "income";
                var amountText = (isIncome ? "+ " : "- ") + "₹ " + t.Amount.ToString("N0");

                table.AddCell(t.Category?.Title ?? "-");
                table.AddCell(t.Date.ToString("dd MMM yyyy"));
                table.AddCell(t.Note ?? "-");
                table.AddCell(amountText);
            }

            document.Add(table);

            document.Close();

            return File(ms.ToArray(), "application/pdf", "Report.pdf");
        }
    }
}