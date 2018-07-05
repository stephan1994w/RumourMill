using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using RumourMill.Models;
using System.Security.Claims;
using System.Collections;

namespace RumourMill.Controllers
{
    public class HomeController : Controller
    {
        private RumourMillMainEntities db = new RumourMillMainEntities();

        [AllowAnonymous]
        public ActionResult Index()
        {
            List<QContainer> model = new List<QContainer>();

            var combinedModelQuery = (from x in db.Questions
                                      join y in db.Replies on x.QuestionId equals y.fk_QuestionId into qs
                                      from a in qs.DefaultIfEmpty()
                                      join z in db.Leaders on a.fk_LeaderId equals z.LeaderId into ls
                                      from b in ls.DefaultIfEmpty()
                                      select new
                                      {
                                          qText = x.QuestionText,
                                          qApproved = x.IsApproved,
                                          qAnswered = x.IsAnswered,
                                          qID = x.QuestionId,
                                          qTime = x.TimeAsked,
                                          rText = a.ReplyText,
                                          rTime = a.TimeReplied,
                                          lName = b.LeaderName,
                                          lImage = b.Image,
                                      }).ToList();

            List<QuestionReplyViewModel> QRModel = new List<QuestionReplyViewModel>();
            foreach (var joinedItem in combinedModelQuery)
            {
                QRModel.Add(new QuestionReplyViewModel()
                {
                    QuestionText = joinedItem.qText,
                    IsApproved = joinedItem.qApproved,
                    IsAnswered = joinedItem.qAnswered,
                    QuestionId = joinedItem.qID,
                    TimeAsked = joinedItem.qTime ?? DateTime.Now,
                    ReplyText = joinedItem.rText,
                    TimeReplied = joinedItem.rTime ?? DateTime.Now,
                    LeaderName = joinedItem.lName,
                    Image = joinedItem.lImage

                });
            }

            ArrayList qLog = new ArrayList();
            ArrayList qCount = new ArrayList();

            foreach (var i in QRModel)
            {
                if (!qLog.Contains(i.QuestionId))
                {
                    //If new question, add it and set reply num to 1
                    if (i.IsAnswered)
                    {
                        qCount.Add(1);
                        qLog.Add(i.QuestionId);
                    }
                    else
                    {
                        qCount.Add(0);
                        qLog.Add(i.QuestionId);
                    }

                }
                else if (qLog.Contains(i.QuestionId))
                {
                    if (i.IsAnswered)
                    {

                        //If already added, increment number of replies by 1
                        int index = qLog.IndexOf(i.QuestionId);
                        int currentCount = (int)qCount[qLog.IndexOf(i.QuestionId)];
                        currentCount++;
                        qCount[index] = currentCount;
                    }
                }
                
            }
            model.Add(new QContainer()
            {
                qrvmodel = QRModel,
                questionCount = qCount
            });

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