using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using RumourMill.Models;
using System.Security.Claims;

namespace RumourMill.Controllers
{
    public class HomeController : Controller
    {
        private RumourMillMainEntities db = new RumourMillMainEntities();

        [AllowAnonymous]
        public ActionResult Index()
        {
            List<QuestionReplyViewModel> model = new List<QuestionReplyViewModel>();

            var combinedModelQuery = (from x in db.Questions
                                     join y in db.Replies on x.QuestionId equals y.fk_QuestionId
                                     //join z in db.Leaders on y.fk_LeaderId equals z.LeaderId
                                     select new
                                     {                                        
                                         qText = x.QuestionText,
                                         qApproved = x.IsApproved,
                                         qAnswered = x.IsAnswered,
                                         qTime = x.TimeAsked,
                                         rText = y.ReplyText,
                                         rTime = y.TimeReplied,
                                         //lName = z.LeaderName,
                                         //lImage = z.Image,
                                     }).ToList();

            foreach(var joinedItem in combinedModelQuery)
            {
                model.Add(new QuestionReplyViewModel()
                {
                    QuestionText = joinedItem.qText,
                    IsApproved = joinedItem.qApproved,
                    IsAnswered = joinedItem.qAnswered,
                    TimeAsked = joinedItem.qTime ?? DateTime.Now,
                    ReplyText = joinedItem.rText,
                    TimeReplied = joinedItem.rTime,
                    //LeaderName = joinedItem.lName,
                    //Image = joinedItem.lImage

                });
            }


            return View(model);
        }


        [AllowAnonymous]
        public ActionResult Save(string questionText)
        {

            using (db)
            {

                var question = db.Set<Question>();
                question.Add(new Question { QuestionText = questionText, IsApproved = false, IsAnswered = false, TimeAsked=DateTime.Now});

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

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Leader leaderModel)
        {
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                return View(leaderModel); //Returns the view with the input values so that the user doesn't have to retype again
            }
            using (RumourMillMainEntities db = new RumourMillMainEntities())
            {
                var leaderDetails = db.Leaders.Where(x => x.UserName == leaderModel.UserName && x.Password == leaderModel.Password).FirstOrDefault();
                if (leaderDetails != null)
                {
                    //using ApplicationCookie to store login information
                    //Claims Type 'Admin' the only type of user we have with the 
                    //ability to log in. We could implement various types of users 
                    //by adding in further ClaimTypes


                    var identity = new ClaimsIdentity(new[] {
                 new Claim(ClaimTypes.Name, "Admin")
                 // --> add as many claims as you need
                    }, "ApplicationCookie");
                    // get owin context
                    var ctx = Request.GetOwinContext();
                    // get authentication manager
                    var authManager = ctx.Authentication;
                    //sign in as claimed identity- in this case the admin
                    //A user is authenticated by calling AuthenticationManager.SignIn
                    authManager.SignIn(identity);
                    //User is authenticated and redirected to the admin dashboard
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //User authentication failed
                    leaderModel.ErrorMessage = "Invalid Credentials Supplied. Please try again.";
                }
            }
            return View(leaderModel); //Should always be declared on the end of an action method

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}