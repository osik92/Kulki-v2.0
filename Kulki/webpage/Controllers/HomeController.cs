using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using demo.ServiceReference1;

namespace demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string boardSize, string colors)
        {
            try
            {
                GlobalScoreBoardClient client = new GlobalScoreBoardClient();
                ViewBag.Scores = client.GetAllScores();
                ViewBag.BoardSize = boardSize;
                ViewBag.Colors = colors;
            }
            catch (Exception)
            {
                
            }
            
            return View();
        }
    }
}