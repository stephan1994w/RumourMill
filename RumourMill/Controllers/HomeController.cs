﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using RumourMill.Models;
using System.Security.Claims;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

namespace RumourMill.Controllers
{
    public class HomeController : Controller
    {
        private RumourMillEntities db = new RumourMillEntities();

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
                                          qStatus = x.Status,
                                          qAnswered = x.IsAnswered,
                                          qID = x.QuestionId,
                                          qTime = x.TimeAsked,
                                          rText = a.ReplyText,
                                          rTime = a.TimeReplied,
                                          lName = b.LeaderName,
                                          lImage = b.Image
                                      }).ToList();

            List<QuestionReplyViewModel> QRModel = new List<QuestionReplyViewModel>();
            foreach (var joinedItem in combinedModelQuery)
            {
                QRModel.Add(new QuestionReplyViewModel()
                {
                    QuestionText = joinedItem.qText,
                    Status = joinedItem.qStatus,
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

        // allow anyone to ask a question
        [AllowAnonymous]
        public ActionResult Save(string questionText)
        {

            using (db)
            {

                var question = db.Set<Question>();

                question.Add(new Question
                {
                    QuestionText = questionText,
                    Status = "Submitted",
                    IsAnswered = false,
                    TimeAsked = DateTime.Now
                });

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
        
        // allow only SuperAdmin and Leader to reply
        [Authorize(Users = "SuperAdmin, Leader")]
        public ActionResult SaveReply(string replyText, int fk_QuestionId, int fk_LeaderId)
        {

            using (db)
            {
           
                var reply = db.Set<Reply>();
                reply.Add(new Reply { ReplyText = replyText, fk_QuestionId = fk_QuestionId,
                    fk_LeaderId = fk_LeaderId,  TimeReplied = DateTime.Now });

                if (string.IsNullOrEmpty(replyText))
                {
                    ModelState.AddModelError("Reply", "Name Required");
                }

                if (ModelState.IsValid)
                {
                    db.Set<Question>().SingleOrDefault(o => o.QuestionId == fk_QuestionId).IsAnswered = true;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }


            }
        }

        // allow only SuperAdmin and Moderator to accept questions
        [Authorize(Users = "SuperAdmin, Moderator")]
        public ActionResult AcceptQuestion(int fk_QuestionId)
        {

            using (db)
            {
                if (ModelState.IsValid)
                {
                    db.Set<Question>().SingleOrDefault(o => o.QuestionId == fk_QuestionId).Status = "Approved";
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }


            }
        }

        // allow only SuperAdmin and Moderator to accept questions
        [Authorize(Users = "SuperAdmin, Moderator")]
        public ActionResult RejectQuestion(int fk_QuestionId)
        {

            using (db)
            {
                if (ModelState.IsValid)
                {
                    db.Set<Question>().SingleOrDefault(o => o.QuestionId == fk_QuestionId).Status = "Rejected";
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
            using (RumourMillEntities db = new RumourMillEntities())
            {
                // hash the password and compare against database
                var hashedPassword = Sha256encrypt(leaderModel.Password);
                var leaderDetails = db.Leaders.Where(x => x.UserName == leaderModel.UserName && x.Password == hashedPassword).FirstOrDefault();
                if (leaderDetails != null)
                {

                    var userType = "";
                    if (leaderModel.UserName == "hmallon" ||
                    leaderModel.UserName == "ablair" ||
                    leaderModel.UserName == "dwardley"
                    )
                    {
                        userType = "Leader";
                    }
                    else if (leaderModel.UserName == "swilliams" ||
                        leaderModel.UserName == "dcallaghan" || leaderModel.UserName == "amorgan" || leaderModel.UserName == "emcginty")
                    {
                        userType = "SuperAdmin";
                    }
                    else if (leaderModel.UserName == "mnorman" || 
                        leaderModel.UserName == "aohara")
                    {
                        userType = "Moderator";
                    }

                    var identity = new ClaimsIdentity(new[] {
                 new Claim(ClaimTypes.Name, userType)
                 // --> add as many claims as you need
                    }, "ApplicationCookie");
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, leaderDetails.LeaderId.ToString()));
                    // get owin context
                    var ctx = Request.GetOwinContext();
                    // get authentication manager
                    var authManager = ctx.Authentication;
                    //sign in as claimed identity- in this case the admin
                    //A user is authenticated by calling AuthenticationManager.SignIn
                    authManager.SignIn(identity);
                    //User is authenticated and redirected
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    //User authentication failed
                }
                
            }
            return View(leaderModel); //Should always be declared on the end of an action method

        }

        // log the user out.
        public ActionResult LogOut()
        {
            // get owin context
            var ctx = Request.GetOwinContext();
            // get authentication manager
            var authManager = ctx.Authentication;
            //Calling SignOut passing the authentication type (so the manager knows exactly what cookie to remove).
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //method to hash the password using SHA256 encryption
        public static string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }
}