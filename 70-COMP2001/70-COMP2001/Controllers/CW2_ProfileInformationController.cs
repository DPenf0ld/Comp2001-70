using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _70_COMP2001.Controllers
{
    public class CW2_ProfileInformationController : Controller
    {
        // GET: CW2_ProfileInformationController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CW2_ProfileInformationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CW2_ProfileInformationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CW2_ProfileInformationController/Create
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

        // GET: CW2_ProfileInformationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CW2_ProfileInformationController/Edit/5
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

        // GET: CW2_ProfileInformationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CW2_ProfileInformationController/Delete/5
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
