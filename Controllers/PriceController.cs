using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Visitka.Controllers
{
    public class PriceController : Controller
    {
        // GET: PriceController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PriceController/Details/{id}
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PriceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PriceController/Create
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

        // GET: PriceController/Edit/{id}
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PriceController/Edit/{id}
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

        // GET: PriceController/Delete/{id}
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PriceController/Delete/{id}
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
