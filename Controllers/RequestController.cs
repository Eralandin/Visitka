using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Visitka.Controllers
{
    public class RequestController : Controller
    {
        // GET: RequestController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RequestController/Details/{id}
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RequestController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RequestController/Create
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

        // GET: RequestController/Edit/{id}
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RequestController/Edit/{id}
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

        // GET: RequestController/Delete/{id}
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RequestController/Delete/{id}
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
