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

        // GET: PriceController/Details/5
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

        // GET: PriceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PriceController/Edit/5
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

        // GET: PriceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PriceController/Delete/5
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
