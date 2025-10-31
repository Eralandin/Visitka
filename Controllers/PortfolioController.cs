using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Visitka.Controllers
{
    public class PortfolioController : Controller
    {
        // GET: PortfolioController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PortfolioController/Details/{id}
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PortfolioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PortfolioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PortfolioController/Edit/{id}
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PortfolioController/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PortfolioController/Delete/{id}
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PortfolioController/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
