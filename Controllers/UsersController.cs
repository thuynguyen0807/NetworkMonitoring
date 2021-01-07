using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetworkMonitor.Data;
using NetworkMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace NetworkMonitor.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("Email") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET - REGISTER
        public IActionResult Register()
        {
            return View();
        }

        //POST - REGISTER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Users user)
        {
            if (ModelState.IsValid)
            {
                var check = _db.Users.FirstOrDefault(s => s.Email == user.Email);
                if (check == null)
                {
                    user.Password = GetMD5(user.Password);
                    _db.Users.Add(user);
                    _db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists!";
                }
            }
            return View(user);
        }

        // GET - LOGIN
        public IActionResult Login()
        {
            return View();
        }

        // POST - LOGIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel userModel)
        {
            if (ModelState.IsValid)
            {               
                var f_password = GetMD5(userModel.Password);

                // 1 user la 1 nguoi duy nhat nen cu dung FirstOrDefault()
                // 2 Khong nen truyen ca 1 entity la User vao request len, O day request chi co Email va Password nen em chi can tao 
                // model chua 2 field la Email va Password thoi

                // E nen chia ra, Tang data, tang request , va xu ly, Users nay em dung chung ko nen
                // cai ni chua co data ma
                // ko phai, cai loi nay la ko tim thay trang index nha
                // em biet cai loi nay roi, nhung ma cai model ni anh moi tao ra da co du lieu dau sao ma no dang nhap vao duoc, no lay du lieu cua cai table kia ha
                /// cai model nay trong ko co j, no la data em truyen len de check thoi ma, con data cua em la no lay table Users tu Db ra dong nay ne

                var userLoggedIn = _db.Users.FirstOrDefault(x => x.Email == userModel.Email && x.Password == f_password); 
                if (userLoggedIn != null)
                {
                    HttpContext.Session.SetString("Email", userModel.Email);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "Login failed!";
                    return RedirectToAction("Login");
                }
            }
            return View(userModel);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

    }
}
