using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RumourMill.Models;

namespace RumourMill.Controllers
{
    public class HomeController : Controller
    {
        private RumourMillMainEntities db = new RumourMillMainEntities();

        public ActionResult Index()
        {
            return View(db.Questions.ToList());
        }
        [AllowAnonymous]
        public ActionResult Save(string questionText, bool isApproved, bool isAnswered, string answeredBy)
        {

            using (db)
            {
                var question = db.Set<Question>();
                question.Add(new Question { QuestionText = questionText, IsApproved = isApproved, IsAnswered = isAnswered, AnsweredBy = answeredBy });

                db.SaveChanges();
                return RedirectToAction("Index","Home");
            }
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}