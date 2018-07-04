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
        public ActionResult Save(string questionText)
        {

            using (db)
            {

                var question = db.Set<Question>();
                question.Add(new Question { QuestionText = questionText, IsApproved = false, IsAnswered = false});

                if (string.IsNullOrEmpty(questionText))
                {
                    ModelState.AddModelError("Question", "Name Required");
                }

                if (ModelState.IsValid)
                {
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                
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