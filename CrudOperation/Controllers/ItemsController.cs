using CrudOperation.Models;
using CrudOperation.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudOperation.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ItemsController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var items = context.Items.OrderByDescending(i =>i.Id).ToList();

            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ItemDto itemDto)
        {
            if (itemDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The Image is Required");
            }
            if(!ModelState.IsValid)
            {
                return View(itemDto);
            }

            //Save Image File

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(itemDto.ImageFile!.FileName);

            string imageFullPath=environment.WebRootPath +"/Items/" +newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                itemDto.ImageFile.CopyTo(stream);
            }

            Item item = new Item()
            {
                Name = itemDto.Name,
                Unit=itemDto.Unit,
                Quantity = itemDto.Quantity,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Items.Add(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Items");
        }

        public IActionResult Edit(int id) 
        {
            var item=context.Items.Find(id);
            if (item == null)
            {
                return RedirectToAction("Index", "Items");
            }

            //create ItemDto from Item
            var itemDto = new ItemDto()
            {
                Name=item.Name,
                Unit=item.Unit,
                Quantity = item.Quantity,
                
            };

            ViewData["ItemID"] = item.Id;
            ViewData["ImageFileName"]=item.ImageFileName;
            ViewData["CreatedAt"] = item.CreatedAt.ToString("dd/MM/yyyy");

            return View(itemDto);   
        }

        [HttpPost]
        public IActionResult Edit(int id,ItemDto itemDto)
        {
            var item=context.Items.Find(id);
            if (item == null)
            {
                return RedirectToAction("Index", "Items");
            }

            if(!ModelState.IsValid)
            {
                ViewData["ItemID"] = item.Id;
                ViewData["ImageFileName"] = item.ImageFileName;
                ViewData["CreatedAt"] = item.CreatedAt.ToString("dd/MM/yyyy");
                return View(itemDto);
            }

            //update the image file if we have a new image File 
            string newFileName=item.ImageFileName;
            if(itemDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(itemDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/Items/" + newFileName;
                using(var stream=System.IO.File.Create(imageFullPath))
                {
                    itemDto.ImageFile.CopyTo(stream);
                }

                //delete old image
                string oldImageFullPath = environment.WebRootPath + "/Items/" + item.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);
            }

            //update the item in the database
            item.Name=itemDto.Name;
            item.Unit=itemDto.Unit;
            item.Quantity=itemDto.Quantity;
            item.ImageFileName=newFileName;

            context.SaveChanges();

            return RedirectToAction("Index", "Items");

        }


        public IActionResult Delete(int id)
        {
            var item=context.Items.Find(id);
            if (item == null)
            {
                return RedirectToAction("Index", "Items");
            }

            string imageFullPath=environment.WebRootPath +"/Items"+item.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Items.Remove(item);
            context.SaveChanges();

            return RedirectToAction("Index", "Items");
        }


    }
}
