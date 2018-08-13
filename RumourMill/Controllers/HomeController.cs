using System;
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
using Microsoft.AspNet.Identity;

namespace RumourMill.Controllers
{
    public class HomeController : Controller
    {
        private RumourMillEntities db = new RumourMillEntities();

        /*
         * Method to display the Index View. 
         * Before the View is loaded, the Questions table is joined to the Replies table to show all questions
         * and the replies associated with those questions. 
         * For every question that has a reply, this method loops through the replies and appends them below the
         * question.
         * @return The Index View.
         */

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
                                          qER = x.EditReason,
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
                    EditReason = joinedItem.qER,
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

            QRModel.Reverse();
            qCount.Reverse();
            model.Add(new QContainer()
            {
                qrvmodel = QRModel,
                questionCount = qCount
            });

            return View(model);
        }

        /*
         * Method to allow anyone to ask a question.
         * @param questionText - The text of the user's question.
         * @return Redirect the user to the index page.
         */

        [AllowAnonymous]
        public ActionResult Save(string questionText)
        {

            using (db)
            {
                DateTime currentTime;

                //FOR BST
                currentTime = DateTime.Now;
                currentTime.AddHours(1);
                var question = db.Set<Question>();

                question.Add(new Question
                {
                    QuestionText = questionText,
                    Status = "Submitted",
                    IsAnswered = false,
                    TimeAsked = currentTime
                });

                if (string.IsNullOrEmpty(questionText))
                {
                    ModelState.AddModelError("Question", "Name Required");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SaveChanges();
                        TempData["UserMessage"] = "Thanks for your response. Your question has been submitted for moderation.";

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception e)
                    {
                        TempData["UserMessage"] = "There has been an error. Please contact Stephan or Daniel.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["UserMessage"] = "There has been an error. Please contact Stephan or Daniel.";
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        /*
         * Method to allow a Leader to reply to a question
         * @param replyText - The text of the leaders response.
         * @param fk_QuestionId - The question ID of the question the leader is replying to.
         * @return Redirect the user to the index page.
         */

        [Authorize(Roles = "Leader, SuperAdmin")]
        public ActionResult SaveReply(string replyText, int fk_QuestionId)
        {
            using (db)
            {
                var reply = db.Set<Reply>();
                string id = User.Identity.GetUserId();
                int idInt = Convert.ToInt32(id);
                reply.Add(new Reply
                {
                    ReplyText = replyText,
                    fk_QuestionId = fk_QuestionId,
                    fk_LeaderId = idInt,
                    TimeReplied = DateTime.Now
                });

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


        /*
         * Method to allow moderators to accept a question.
         * @param fk_QuestionId - The question ID of the question to be accepted.
         * @return Redirect the user to the index page.
         */

        [Authorize(Roles = "Moderator, SuperAdmin")]
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

        /*
         * Method to allow moderators to reject a question.
         * @param fk_QuestionId - The question ID of the question to be rejected.
         * @return Redirect the user to the index page.
         */
        
        [Authorize(Roles = "Moderator, SuperAdmin")]
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

        /*
         * Method to edit a question. This method can only be called as a SuperAdmin or a Moderator.
         * @param questionId - The question Id of the question that should be edited.
         * @param editText - The edited version of the question that should be edited.
         * @param reason - The reason why the question was edited.
         * @param user - The user who made the edit (for the log)
         * @param oldText - The old question text (for the log)
         * @param reasonOther - If the moderator selected Other as their reason, the free text box is stored here.
         * @return Redirect the user to the index page.
         * 
         */

        [Authorize(Roles = "Moderator, SuperAdmin")]
        public ActionResult EditQuestion(int questionId, string editText, string reason, string user, string oldText, string reasonOther)
        {

            using (db)
            {
                string reasonToWrite;
                if (reasonOther != "")
                {
                    reasonToWrite = reasonOther;
                }
                else
                {
                    reasonToWrite = reason;
                }

                db.Set<Question>().SingleOrDefault(o => o.QuestionId == questionId).EditReason = reasonToWrite;
                db.SaveChanges();
                // load reference to Log DB table.
                var log = db.Set<Log>();
                // add new row to Log DB table.
                log.Add(new Log
                {
                    QuestionId = questionId,
                    Reason = reason,
                    User = user,
                    TimeEdited = DateTime.Now,
                    OldText = oldText,
                    NewText = editText,
                    ReasonOther = reasonOther
                });

                if (ModelState.IsValid)
                {
                    // edit the question text for the question ID that was passed through, and mark as approved.
                    db.Set<Question>().SingleOrDefault(o => o.QuestionId == questionId).QuestionText = editText;
                    db.Set<Question>().SingleOrDefault(o => o.QuestionId == questionId).Status = "Approved";
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        /*
         * Method to display the Login View. Can be called by all users. 
         * @return The Login View.
         */

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        /*
         * Method to display the ChangePassword View. Can only be called when logged in.
         * @return The ChangePassword View.
         */

        [Authorize(Roles = "Moderator, SuperAdmin, Leader")]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /*
         * Method to change the password. Can only be called when logged in as Moderator, SuperAdmin or Leader.
         * @param currentPassword - The current password in unhashed form
         * @param newPassword - The new password in unhashed form
         * @param leaderModel - The customised version of the Leader model with error messages.
         * @return The View if authentication fails, else redirect the user to the index page.
         */

        [HttpPost]
        [Authorize(Roles = "Moderator, SuperAdmin, Leader")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, LeaderErrors leaderModel)
        {
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                return View(); //Returns the view with the input values so that the user doesn't have to retype again
            }
            using (RumourMillEntities db = new RumourMillEntities())
            {
                int id = Convert.ToInt32(User.Identity.GetUserId());
                // hash the password and compare against database
                if (!(id == null || currentPassword == null))
                {
                    var hashedPassword = Sha256encrypt(currentPassword);
                    var leaderDetails = db.Leaders.Where(x => x.LeaderId == id && x.Password == hashedPassword).FirstOrDefault();

                    if (leaderDetails != null)
                    {
                        var newHashedPassword = Sha256encrypt(newPassword);
                        db.Set<Leader>().SingleOrDefault(o => o.LeaderId == id).Password = newHashedPassword;
                        db.SaveChanges();

                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        //User authentication failed
                        leaderModel.ErrorMessage = "The current password you've entered is incorrect. Please try again.";
                        return View(leaderModel);
                    }
                } else
                {
                    leaderModel.ErrorMessage = "Please enter your current password and your new password.";
                    //User authentication failed - blank 
                }

            }
            return View(leaderModel); //Should always be declared on the end of an action method

        }

        /*
         * Method to log the user in
         * @param The customised version of the leader model that includes an error message (for display on login)
         * @return Redirects the user to the index if logged in, else returns the View again.
         * 
         */

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LeaderErrors leaderModel)
        {
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {
                return View(leaderModel); //Returns the view with the input values so that the user doesn't have to retype again
            }
            using (RumourMillEntities db = new RumourMillEntities())
            {
                // hash the password and compare against database
                if (!(leaderModel.UserName == null || leaderModel.Password == null))
                {
                    var hashedPassword = Sha256encrypt(leaderModel.Password);
                    var leaderDetails = db.Leaders.Where(x => x.UserName == leaderModel.UserName && x.Password == hashedPassword).FirstOrDefault();

                    if (leaderDetails != null)
                    {

                        var identity = new ClaimsIdentity(new[] {
                             new Claim(ClaimTypes.Role, leaderDetails.Role),
                             new Claim(ClaimTypes.Name, leaderDetails.LeaderName),
                             new Claim(ClaimTypes.NameIdentifier, leaderDetails.LeaderId.ToString())
                        },
                            "ApplicationCookie");

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
                        leaderModel.ErrorMessage = "The username or password entered is incorrected. Please try again.";
                        //User authentication failed
                    }
                }
                else
                {
                    leaderModel.ErrorMessage = "The username or password entered is incorrected. Please try again.";
                    //User authentication failed - blank 
                }

            }
            return View(leaderModel); //Should always be declared on the end of an action method

        }

        /*
         * Method to log the user out.
         * @return Redirect the user to the index page.
         */

        [AllowAnonymous]
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

        /**
         * Method to hash the passwords using SHA-256 Encryption to ensure they're stored securely.
         * @param phrase - the password to be encrypted.
         * @return the hashed password
         **/

        [AllowAnonymous]
        public static string Sha256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }


    }
}